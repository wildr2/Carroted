using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MovingObjectAudio : MonoBehaviour 
{
    public float max_move_speed = 10;
    public float max_pitch = 2;
    public float pitch_offset = 0;

    public Rigidbody rbody;
    private AudioSource source;


    public void Awake()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0;
    }
    public void Update()
    {
        float speed_factor = Mathf.Clamp(rbody.velocity.magnitude / max_move_speed, 0, 1);

        // louder volume when moving faster
        if (Time.timeScale == 0) source.volume = 0;
        else source.volume = speed_factor * GameSettings.Instance.volume_fx;

        // faster playback when moving faster
        source.pitch = pitch_offset + speed_factor * Time.timeScale * max_pitch;
    }
}
