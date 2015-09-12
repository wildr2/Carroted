using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour 
{
    public PauseMenuPage pause_menu;
    public GameManager game_manager;
    public BlockManager block_manager;
    public bool Paused { get; private set; }


    public void Update()
    {
        // only allow pausing and unpausing when during game
        if (Input.GetKeyDown(KeyCode.Escape) &&
            (game_manager.GetGameState() == GameState.PostTurn || game_manager.GetGameState() == GameState.PostGame
             || game_manager.GetGameState() == GameState.MemorizationTime || GameSettings.Instance.IsAIMatch()))
        {
            if (Paused && pause_menu.IsTopPage() && !pause_menu.Hidden()) pause_menu.ButtonResume();
            else
            {
                Pause();
                pause_menu.TransitionIn();
            }
        }
    }
    public void TogglePause()
    {
        if (Paused) UnPause();
        else Pause();
    }
    public void Pause()
    {
        TimeScaleManager.Instance.AddMultiplier("pause", 0, true);
        if (!GameSettings.Instance.IsAIMatch()) block_manager.SetWordsVisible(false);
        Paused = true;
    }
    public void UnPause()
    {
        TimeScaleManager.Instance.RemoveMultiplier("pause");
        if (!GameSettings.Instance.IsAIMatch()) block_manager.SetWordsVisible(true);
        Paused = false;
    }

}
