using UnityEngine;
using System.Collections;

public class ReaderAudio : MonoBehaviour 
{
    public BlockManager blocks;
    public AudioSource read_word;
    public AudioSource reading_loop;

    private float reading_loop_volume_mult = 0.5f;


    private void Update()
    {
        if (reading_loop.isPlaying)
        {
            // reading loop volume
            float t = (float)blocks.GetReadLetterIndex() / (float)blocks.GetReadPhrase().Length;
            reading_loop.volume = GameSettings.Instance.volume_fx * reading_loop_volume_mult * (1 - t);
        }
    }

    public void PlayReading()
    {
        reading_loop.pitch = Random.Range(0.8f, 1.2f);
        reading_loop.volume = GameSettings.Instance.volume_fx * reading_loop_volume_mult;
       // reading_loop.Play();
    }
    public void StopReading()
    {
        reading_loop.Stop();
    }
    public void PlayReadWord(int word_len)
    {
        //read_word.pitch = 2 - (Mathf.Min(2, word_len / 5f));
        read_word.volume = GameSettings.Instance.volume_fx;
        read_word.Play();
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float v = source.volume;
        while (v > 0)
        {
            v -= Time.deltaTime * 0.5f;
            source.volume = v;
            yield return null;
        }
        source.Stop();
    }
}
