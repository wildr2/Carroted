using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeScreenPage : UIMenuPage 
{
    private Image overlay;
    private Color default_color;
    private bool initialized = false;


    new private void Start()
    {
        if (!initialized) Initialize();
        base.Start();
    }
    private void Initialize()
    {
        overlay = GetComponent<Image>();
        default_color = overlay.color;

        // enable the colored overlay here so that it need not be on in the editor
        overlay.enabled = true;
        
        initialized = true;
    }

    public void TransitionIn(float seconds, float delay_seconds, Color color)
    {
        if (!initialized) Initialize();
        overlay.color = color;

        base.TransitionIn(seconds, delay_seconds);
    }
    public override void TransitionIn(float seconds, float delay_seconds)
    {
        TransitionIn(seconds, delay_seconds, default_color);
    }
    public void TransitionOut(float seconds, float delay_seconds, Color color)
    {
        if (!initialized) Initialize();
        overlay.color = color;

        base.TransitionOut(seconds, delay_seconds);
    }
    public override void TransitionOut(float seconds, float delay_seconds)
    {
        TransitionOut(seconds, delay_seconds, default_color);
    }
}
