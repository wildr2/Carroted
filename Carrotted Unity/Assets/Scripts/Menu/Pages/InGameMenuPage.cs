using UnityEngine;
using System.Collections;

public class InGameMenuPage : UIMenuPage 
{
    public FadeScreenPage fade_page;
    public Transform hideable;

    private bool hidden = false;


    public void ButtonHideMenu()
    {
        if (hidden) return;

        hidden = true;
        hideable.gameObject.SetActive(false);
        StartCoroutine("CheckForUnHideInput");
    }
    public void ButtonRestart()
    {
        fade_page.TransitionIn();
        fade_page.on_transitioned_in = () => Application.LoadLevel("Game");
    }
    public void ButtonQuit()
    {
        //TransitionOut();
        fade_page.TransitionIn();
        fade_page.on_transitioned_in = () => Quit();
    }


    private void Quit()
    {
        Application.LoadLevel("Main Menu");
    }
    private IEnumerator CheckForUnHideInput()
    {
        while (true)
        {
            if (Input.anyKey)
            {
                hideable.gameObject.SetActive(true);
                SetStateOut();
                on_transitioned_in = new System.Action(()=>hidden=false);
                TransitionIn();
                break;
            }

            yield return null;
        }
    }

    public bool Hidden()
    {
        return hidden;
    }
}
