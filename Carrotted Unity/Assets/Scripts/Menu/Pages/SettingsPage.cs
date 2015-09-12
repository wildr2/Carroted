using UnityEngine;
using System.Collections;

public class SettingsPage : UIMenuPage
{
    public Transform camguide;
    public MainMenuPage page_mainmenu;


    protected override void OnStartTransitionIn()
    {
        Camera.main.transform.position = camguide.position;
        Camera.main.transform.rotation = camguide.rotation;
        base.OnStartTransitionIn();
    }

    public void OnButtonBack()
    {
        TransitionOut();
        page_mainmenu.TransitionIn();
    }
}
