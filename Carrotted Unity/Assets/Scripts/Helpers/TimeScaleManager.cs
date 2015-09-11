using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A singleton that manages multiple assignments to Time.timescale
/// </summary>
public class TimeScaleManager : MonoBehaviour 
{
    private static TimeScaleManager _instance;
    public static TimeScaleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TimeScaleManager>();

                if (_instance == null) Debug.LogError("Missing TimeScaleManager");
                else
                {
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    public float default_timescale = 1;
    public bool maintain_fixedtimestep_ratio = true;
    public float fixed_timestep = 0.016f;

    // time scale multipliers (id, multipliers on this layer (the top float is the used one))
    private Dictionary<string, Stack<float>> multipliers = new Dictionary<string, Stack<float>>();
    private float current_time_scale;


    // PUBLIC MODIFIERS

    public void AddMultiplier(string id, float multiplier, bool replace)
    {
        if (multipliers.ContainsKey(id))
        {
            if (replace)
            {
                // replace the multiplier at the top of the stack
                Stack<float> s = multipliers[id];
                s.Pop();
                s.Push(multiplier);
            }
            else
            {
                // add onto the top of the current stack of mults for this id
                // only the top mult of the stack multiplies the time scale
                multipliers[id].Push(multiplier);
            }
        }
        else
        {
            // add a new multiplier
            Stack<float> s = new Stack<float>();
            s.Push(multiplier);
            multipliers.Add(id, s);
        }

        UpdateCurrentTimeScale();
        UpdateFixedTimeStep();
    }
    public void AddMultiplier(string id, float multiplier)
    {
        AddMultiplier(id, multiplier, true);
    }
    public void RemoveMultiplier(string id, bool only_last_added)
    {
        if (!multipliers.ContainsKey(id)) return;

        if (only_last_added)
        {
            Stack<float> s = multipliers[id];
            if (s.Count > 0)
            {
                s.Pop();
                if (s.Count == 0) multipliers.Remove(id);
            }
        }
        else
        {
            multipliers.Remove(id);
        }

        UpdateCurrentTimeScale();
        UpdateFixedTimeStep();
    }
    public void RemoveMultiplier(string id)
    {
        RemoveMultiplier(id, false);
    }

    /// <summary>
    /// Remove all timescale multipliers but the original
    /// </summary>
    public void Reset()
    {
        multipliers = new Dictionary<string, Stack<float>>();
        AddMultiplier("default", default_timescale);
        UpdateFixedTimeStep();
    }


    // PRIVATE MODIFIERS

    private void Awake()
    {
        // if this is the first instance, make this the singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
            Reset();
        }
        else
        {
            // destroy other instances that are not the already existing singleton
            if (this != _instance)
            {
                // save new inspector parameters
                _instance.default_timescale = this.default_timescale;
                _instance.maintain_fixedtimestep_ratio = this.maintain_fixedtimestep_ratio;
                _instance.fixed_timestep = this.fixed_timestep;

                Destroy(this.gameObject);

                _instance.Reset();
            }
                
        }
    }
    private void Update()
    {
        if (Time.timeScale != current_time_scale)
        {
            Debug.LogWarning("Time.timeScale was set by something other than TimeScaleManager");

            // insure that time scale is controlled by this manager
            Time.timeScale = current_time_scale;
        }
        
    }
    private void UpdateCurrentTimeScale()
    {
        current_time_scale = 1;
        foreach (Stack<float> mults in multipliers.Values)
        {
            current_time_scale *= mults.Peek(); // note: there must be an element in mults
        }
        Time.timeScale = current_time_scale;
    }
    private void UpdateFixedTimeStep()
    {
        if (maintain_fixedtimestep_ratio)
        {
            Time.fixedDeltaTime = fixed_timestep * Time.timeScale;
        }
    }


    // PUBLIC ACCESSORS

    /// <summary>
    /// Returns the unscaled delta time, or 0 if timeScale is 0
    /// </summary>
    /// <returns></returns>
    public float UnscaledDTime()
    {
        return current_time_scale <= 0 ? 0 : Time.unscaledDeltaTime;
    }
}