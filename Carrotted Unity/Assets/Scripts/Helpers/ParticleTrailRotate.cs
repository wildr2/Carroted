using UnityEngine;
using System.Collections;


[RequireComponent(typeof(ParticleSystem))]
public class ParticleTrailRotate : MonoBehaviour
{
    private ParticleSystem ps;
    public Rigidbody rb;


    public void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        //if (!ps) Debug.LogError("ParticleSystem not found");
    }

    public void Update()
    {
        ps.startRotation = -Mathf.Atan2(rb.velocity.z, rb.velocity.x);
    }
}
