using UnityEngine;
using System.Collections;

public class MainMenuPage : UIMenuPage 
{
    public Transform camguide;
    public MatchSetupPage page_match_setup;
    public SettingsPage page_settings;

    public Player[] players;


    new public void Start()
    {
        base.Start();
    }

    protected override void OnStartTransitionIn()
    {
        Camera.main.transform.position = camguide.position;
        Camera.main.transform.rotation = camguide.rotation;
        base.OnStartTransitionIn();
    }

    public void OnButtonMatch()
    {
        TransitionOut();
        page_match_setup.TransitionIn();
    }
    public void OnButtonSettings()
    {
        TransitionOut();
        page_settings.TransitionIn();
    }
}
