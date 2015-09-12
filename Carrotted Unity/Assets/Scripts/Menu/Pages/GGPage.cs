using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GGPage : InGameMenuPage
{
    private GameManager game_manager;
    public Text heading;


    public void Awake()
    {
        game_manager = Object.FindObjectOfType<GameManager>();
        if (game_manager == null) Debug.LogError("GameManager missing.");
    }
    public void OnEnable()
    {
        //UIAudio.Instance.PlayPause();

        
        int result = game_manager.GetWinner();
        if (result == 1)
        {
            heading.text = GameSettings.Instance.player_name[0] + " Wins";
        }
        else if (result == 2)
        {
            heading.text = GameSettings.Instance.player_name[1] + " Wins";
        }
        else
            heading.text = "Draw";
        
    }
}
