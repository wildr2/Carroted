using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UITransitionFlicker : UITransition
{
    private CanvasRenderer canvas_r;
    private CanvasGroup canvas_group;
    private float alpha_initial;
    public float offset = 0;
    public float flicker_intensity = 0.2f;
    public bool in_only = true;

    private bool on = false;


    public void Awake()
    {
        canvas_r = GetComponent<CanvasRenderer>();
        canvas_group = GetComponent<CanvasGroup>();

        if (canvas_group != null) alpha_initial = canvas_group.alpha;
        else alpha_initial = canvas_r.GetAlpha();
    }
    public override void UpdateTransition(float transition, bool going_in)
    {
        float t = transition;

        // offset
        if (going_in)
        {
            t = Mathf.Max(0, (t - offset) / (1 - offset));
        }
        else
        {
            if (in_only) t = 0;
            else
                t = 1 - Mathf.Max(0, ((1 - t) - offset) / (1 - offset));
        }

        // set alpha
        //float flicker_t = t % (1-flicker_intensity) < 0.15f ? 0 : 1;
        //if (t >= 1) flicker_t = 1;

        
        if (t >= 1) on = true;
        else if (t <= 0) on = false;
        else if (Random.value < flicker_intensity) on = !on;

        float flicker_t = on ? 1 : 0;

        if (canvas_group != null) canvas_group.alpha = alpha_initial * flicker_t;
        else canvas_r.SetAlpha(alpha_initial * flicker_t);
    }

}
