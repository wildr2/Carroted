using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum GameState { MemorizationTime, PreRead, Reading, PostRead, PostTurn, PostGame }

public class GameManager : MonoBehaviour
{
    public bool debug = false;

    public MatchAudio match_audio;
    public BlockManager block_manager;
    public Player[] players;

    // State and score
    private GameState state = GameState.MemorizationTime;
    private int[] scores;
    private int points_to_win = 16;
    private int winner_player_num = -1;
    private bool turn_resolved; // a point was won by either player


    // Time
    private float first_preread_delay = 8f, pre_read_time_min = 1.5f, pre_read_time_max = 3.5f, pre_read_time = 0;
    private float post_turn_time = 5f;

    private float memorize_time = 120;
    private float memtime_message_interval = 10f;
    private float memtime_sound_interval = 30f;

    private float timer = 0;
    private float match_time = 0;

    



    // UI
    public TextMesh ui_score, ui_messenger;
    private bool display_info = false;

    private float message_timer = 2f;
    private float min_message_time = 2f;
    private Queue<string> message_queue = new Queue<string>();


    private Vector3 cam_intitial_pos;


    // PRIVATE MODIFIERS

    private void Start()
    {
        block_manager.Initialize();

        // players
        players[0].Inititalize(1, this);
        players[1].Inititalize(2, this);

        // events
        for (int i = 0; i < players.Length; ++i)
        {
            int p = i + 1;
            players[i].event_go += () => OnPlayerGo(p);
            players[i].event_gone += () => OnPlayerGone(p);
        }
        block_manager.event_read_done += OnReadDone;

        // scores
        scores = new int[players.Length];
        UpdateUIScore();

        // camera
        cam_intitial_pos = Camera.main.transform.position;

        // begin
        BeginMemorization();
    }
    private void Update()
    {
        // match time
        if (state != GameState.PostGame)
            match_time += Time.deltaTime;

        // Messenger ui
        
        UpdateMessenger();


        if (state == GameState.PreRead)
        {
            timer += Time.deltaTime;
            if (timer > pre_read_time) ReadNext();
        }
        else if (state == GameState.PostTurn)
        {
            ZoomOutCam();
            timer += Time.deltaTime;
            if (timer > post_turn_time)
            {
                if (winner_player_num == -1)
                    ResetToPreRead();
                else
                    ResetToPostGame();
            }
        }
        else if (state == GameState.PostGame)
        {
        }
    }
    
    private void BeginMemorization()
    {
        memorize_time = GameSettings.Instance.GetMemorizeTimeMins() * 60f;
        if (debug)
        {
            memorize_time = 0;
        }

        
        state = GameState.MemorizationTime;
        match_audio.PlayMemorizeBegin();
        StartCoroutine(UpdateMemorizationTime());
    }
    private IEnumerator UpdateMemorizationTime()
    {
        timer = 0;
        UpdateUIMemorizationTime();

        float sound_timestamp = Time.timeSinceLevelLoad;
        float message_timestamp = Time.timeSinceLevelLoad;

        while (timer <= memorize_time)
        {
            timer += Time.deltaTime;
            if (timer <= 0) break;


            // time reminder sound
            if (Time.timeSinceLevelLoad - sound_timestamp > memtime_sound_interval)
            {
                sound_timestamp = Time.timeSinceLevelLoad;
                match_audio.PlayMemorizeInterval();
            }

            // time left messege
            if (Time.timeSinceLevelLoad - message_timestamp > memtime_message_interval)
            {
                message_timestamp = Time.timeSinceLevelLoad;
                UpdateUIMemorizationTime();
            }

            yield return null;
        }

        BeginMatch();
    }

    private void BeginMatch()
    {
        match_audio.PlayMemorizeOver();

        timer = 0;
        pre_read_time = first_preread_delay + Random.Range(pre_read_time_min, pre_read_time_max);
        state = GameState.PreRead;
        display_info = true;
        for (int i = 0; i < players.Length; ++i)
        {
            players[i].SetHasControl(true);
        }
    }
    private void ResetToPreRead()
    {
        match_audio.PlayPreturnBegin();

        display_info = true;

        block_manager.ResetBlocks();
        block_manager.ResetReader();
        players[0].Reset();
        players[1].Reset();
        
        timer = 0;
        pre_read_time = Random.Range(pre_read_time_min, pre_read_time_max);

        Camera.main.transform.position = cam_intitial_pos;

        state = GameState.PreRead;
        turn_resolved = false;
    }
    private void ResetToPostGame()
    {
        timer = 0;

        block_manager.ResetBlocks();

        players[0].Reset();
        players[1].Reset();
        players[0].SetHasControl(false);
        players[1].SetHasControl(false);

        state = GameState.PostGame;
    }
    private void ToPostTurn()
    {
        timer = 0;
        block_manager.GetReadBlock().Remove();
        state = GameState.PostTurn;

        if (block_manager.UnreadBlocks() == 0)
        {
            GG(GeneralHelpers.IndexOfMax(scores) + 1);
        }
    }

    private void ReadNext()
    {
        if (block_manager.GetReadBlock() != null) block_manager.GetReadBlock().event_hit = null;
        block_manager.ReadNext();
        block_manager.GetReadBlock().event_hit += OnReadBlockHit;
        
        state = GameState.Reading;
    }

    private void OnReadDone()
    {
        timer = 0;
        state = GameState.PostRead;

        // if bother players have gone or block has been hit, go to post turn
        if (players[0].HasGone() && players[1].HasGone() ||
            block_manager.GetReadBlock().IsHit()) ToPostTurn();
    }
    private void OnReadBlockHit(int player_num)
    {
        if (player_num != -1 && !turn_resolved) // not a neutral hit from a fault
        {
            GivePoint(player_num);
            match_audio.PlayReadBlockHit();
        }
            

        // if in post read and block hit, go to post turn
        if (state == GameState.PostRead)
            ToPostTurn();
    }
    private void OnPlayerGo(int player_num)
    {
        if (state == GameState.PreRead)
            FaultFalseStart(player_num);
    }
    private void OnPlayerGone(int player_num)
    {
        if (state == GameState.PreRead)
            FaultFalseStart(player_num);

        // if another fault has not happened and the block has not been hit, fault
        if (!turn_resolved)
        {
            FaultMiss(player_num);
        }
    }

    private void FaultFalseStart(int player_num)
    {
        match_audio.PlayFault();
        Message("FAULT FALSE START    " + players[player_num - 1].GetPlayerName());

        ReadNext();
        GivePointToOpponents(player_num);
        block_manager.GetReadBlock().Hit(GetOpponent(player_num), false);
    }
    private void FaultMiss(int player_num)
    {
        match_audio.PlayFault();
        Message("FAULT MISS    " + players[player_num - 1].GetPlayerName());
        GivePointToOpponents(player_num);

        block_manager.GetReadBlock().Hit(GetOpponent(player_num), false);

        // if in post read and fault given, go to post turn
        if (state == GameState.PostRead)
            ToPostTurn();
    }
    private void GivePoint(int player_num)
    {
        Message("POINT    " + players[player_num - 1].GetPlayerName() + "    (" + block_manager.GetReadWord() + ")");
        scores[player_num - 1]++;
        if (scores[player_num - 1] >= points_to_win)
        {
            GG(player_num);
        }
        UpdateUIScore();
        turn_resolved = true;
    }
    private void GG(int winning_player_num)
    {
        match_audio.PlayGameOver();

        Message("GAME    " + players[winning_player_num - 1].GetPlayerName());
        winner_player_num = winning_player_num;
    }
    private void GivePointToOpponents(int player_num)
    {
        for (int i = 0; i < players.Length; ++i)
        {
            if (i + 1 == player_num) continue;
            GivePoint(i + 1);
        }
    }

    private void ZoomOutCam()
    {
        Vector3 pos = Camera.main.transform.position;
        pos.y = Mathf.Lerp(pos.y, cam_intitial_pos.y + 3, Time.deltaTime);
        Camera.main.transform.position = pos;
    }

    private void UpdateUIScore()
    {
        ui_score.text = "";
        for (int i = 0; i < players.Length; ++i)
        {
            ui_score.text += scores[i] + " : " + players[i].GetPlayerName() + "\n";
        }
    }
    private void UpdateUIMemorizationTime()
    {
        int secs = (int)Mathf.Ceil((memorize_time - timer) / 10f) * 10;
        ui_messenger.text = "MEMORIZE    " + "< " + secs + " seconds";
    }
    private void UpdateMessenger()
    {
        // message timer
        if (message_timer < min_message_time)
        {
            message_timer += Time.deltaTime;

            // next message
            if (message_timer >= min_message_time && message_queue.Count > 0)
            {
                message_timer = 0;
                display_info = false;
                ui_messenger.text = message_queue.Dequeue();
            }
        } 
        else if (display_info)
        {
            ui_messenger.text = "FIRST TO " + points_to_win + "    " + FormatMinSecTimer(match_time);
        }
       
    }
    private void Message(string message)
    {
        if (message_timer < 1)
        {
            message_queue.Enqueue(message);
        }
        else
        {
            message_timer = 0;
            display_info = false;
            ui_messenger.text = message;
        }
            
    }


    // PRIVATE ACCESSORS and HELPERS

    private string FormatMinSecTimer(float seconds)
    {
        if (Mathf.Abs(seconds) < 0.1f) return "0:00";

        int min = (int)(seconds / 60);
        int sec = (int)(seconds % 60);

        return min + ":" + (sec < 10 ? "0" : "") + sec;
    }
    

    // PUBLIC ACCESSORS

    public Player GetOpponent(int player_num)
    {
        if (player_num == 1) return players[1];
        else return players[0];
    }
    public GameState GetGameState()
    {
        return state;
    }
    public int GetWinner()
    {
        return winner_player_num;
    }

}
