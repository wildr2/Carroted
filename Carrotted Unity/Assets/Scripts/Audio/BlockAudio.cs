using UnityEngine;
using System.Collections;

public class BlockAudio : MonoBehaviour
{
    public WorldSound block_collision;
    public WorldSound floor_collision;

    public void PlayBlockCollision(float force)
    {
        block_collision.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        block_collision.base_volume = 0.5f * force;
        block_collision.Play();
    }
    public void PlayFloorCollision(float force)
    {
        floor_collision.SetPitchOffset(Random.Range(-0.05f, 0.05f));
        floor_collision.base_volume = 0.4f * force;
        floor_collision.Play();
    }


}
