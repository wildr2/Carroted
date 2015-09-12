using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour 
{
    public PauseMenuPage pause_menu;
    public GameManager game_manager;
    public bool Paused { get; private set; }


    public void Update()
    {
        // only allow pausing and unpausing when during game
        if (Input.GetKeyDown(KeyCode.Escape) &&
            (game_manager.GetGameState() == GameState.PostTurn || game_manager.GetGameState() == GameState.PostGame
             || game_manager.GetGameState() == GameState.MemorizationTime))
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
        Paused = true;
    }
    public void UnPause()
    {
        TimeScaleManager.Instance.RemoveMultiplier("pause");
        Paused = false;
    }

}
