using UnityEngine;
using System.Collections;

public class MatchAudio : MonoBehaviour
{
    public AudioSource memorize_begin;
    public AudioSource memorize_interval;
    public AudioSource memorize_over;

    public AudioSource preread_begin;
    public AudioSource game_over;

    public AudioSource read_block_hit;
    public AudioSource fault;


    public void Update()
    {
        // turn off looping sounds when paused
        /*
        if (Time.timeScale == 0)
        {
            source_alert_loop.volume = 0;
        }
        else
        {
            source_alert_loop.volume = GameSettings.Instance.volume_fx;
        }
        */
    }

    public void PlayMemorizeBegin()
    {
        memorize_begin.volume = GameSettings.Instance.volume_fx;
        memorize_begin.Play();
    }
    public void PlayMemorizeInterval()
    {
        memorize_interval.volume = GameSettings.Instance.volume_fx;
        memorize_interval.Play();
    }
    public void PlayMemorizeOver()
    {
        memorize_over.volume = GameSettings.Instance.volume_fx;
        memorize_over.Play();
    }

    public void PlayPreturnBegin()
    {
        preread_begin.volume = GameSettings.Instance.volume_fx;
        preread_begin.Play();
    }
    public void PlayGameOver()
    {
        game_over.volume = GameSettings.Instance.volume_fx;
        game_over.Play();
    }

    public void PlayReadBlockHit()
    {
        read_block_hit.volume = GameSettings.Instance.volume_fx * 2f;
        read_block_hit.Play();
    }
    public void PlayFault()
    {
        fault.volume = GameSettings.Instance.volume_fx * 1f;
        fault.Play();
    }
}
