using UnityEngine;
using System.Collections;

public class StartPopupPage : UIMenuPage
{
    public UIMenuPage fade_page;

	public void StartMatch()
    {
        fade_page.on_transitioned_in = new System.Action(() => Application.LoadLevel("Game"));
        fade_page.TransitionIn();
    }
}
