using UnityEngine;
using System.Collections;

public class BlockStump : MonoBehaviour 
{
    private MeshRenderer mesh;
    private static float pos_y_intial = -0.5f, pos_y_final = 1f;
    private static float rise_time = 2;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }
    private IEnumerator RiseFromGround()
    {
        float t = 0;
        Vector3 pos = transform.position;

        while (t < 1)
        {
            t += Time.deltaTime / rise_time;
            pos.y = Mathf.Lerp(pos_y_intial, pos_y_final, t);
            transform.position = pos;
            yield return null;
        }
        pos.y = pos_y_final;
        transform.position = pos;
    }

    public void Initialize(Vector3 pos, Color color)
    {
        mesh.material.color = color;
        transform.position = new Vector3(pos.x, pos_y_intial, pos.z);
        StartCoroutine(RiseFromGround());
    }
    
}
