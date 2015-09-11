using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LineTrailRenderer : MonoBehaviour
{
    private LineRenderer line;

    public float y_offset = 0;
    public float vert_distance = 0.5f;
    public float life_time = 1;

    private Queue<Vector3> vertices = new Queue<Vector3>();
    private Queue<float> vertex_timestamps = new Queue<float>();
    private Vector3? last_vertex;
 
    private bool emitting = true;


    private void Start()
    {
        //if (num_vertices <= 0) Debug.LogError("must set a positive number of line vertices");

        line = GetComponent<LineRenderer>();
        line.SetVertexCount(0);
    }
    private void Update()
    {
        bool need_rerender = false;

        if (emitting)
        {
            if (last_vertex == null || Vector3.Distance(transform.position, (Vector3)last_vertex) >= vert_distance)
            {
                PlaceVertex();
                need_rerender = true;
            }
        }

        // remove oldest vertex if its lifetime is up
        if (FirstVertexDone())
        {
            vertices.Dequeue();
            vertex_timestamps.Dequeue();
            need_rerender = true;
        }

        if (need_rerender) UpdateRenderer();
    }
    private void PlaceVertex()
    {
        last_vertex = transform.position + new Vector3(0, y_offset, 0);
        vertices.Enqueue((Vector3)last_vertex);
        vertex_timestamps.Enqueue(Time.timeSinceLevelLoad);
    }
    private void UpdateRenderer()
    {
        Queue<Vector3>.Enumerator v_itr = vertices.GetEnumerator();

        int n = vertices.Count;
        line.SetVertexCount(n);

        for (int i = n-1; i >= 0; --i)
        {
            if (v_itr.MoveNext())
            {
                line.SetPosition(i, v_itr.Current);
            }
        }
    }

    public void SetEmisionEnabled(bool enable)
    {
        if (!enable)
        {
            last_vertex = null;
        }

        emitting = enable;
    }
    public void Clear()
    {
        vertices = new Queue<Vector3>();
        vertex_timestamps = new Queue<float>();
        last_vertex = null;

        line.SetVertexCount(0);
    }

    public LineRenderer GetLine()
    {
        return line;
    }

    private bool FirstVertexDone()
    {
        if (vertex_timestamps.Count == 0) return false;
        return Time.timeSinceLevelLoad - vertex_timestamps.Peek() > life_time;
    }
}
