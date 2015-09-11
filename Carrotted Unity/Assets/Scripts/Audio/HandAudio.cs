using UnityEngine;
using System.Collections;

public class HandAudio : MonoBehaviour 
{
    public AudioSource moving, swinging;

    public void PlaySwinging()
    {
        swinging.Play();
        moving.Stop();
    }
    public void Reset()
    {
        swinging.Stop();
        moving.Play();
    }
}
