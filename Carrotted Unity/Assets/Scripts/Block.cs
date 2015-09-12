using UnityEngine;
using System.Collections;
using System;

public class Block : MonoBehaviour 
{
    private Rigidbody rb;
    private MeshRenderer mesh;
    public TextMesh text;
    public BlockStump stump_prefab;
    private BlockAudio block_audio;
    
    private Color color;
    private Color color_initial;
    private Vector3 pos_initial;

    private bool hit = false;
    private bool removing = false;
    public Action<int> event_hit;


    private void Start()
    {
        block_audio = GetComponentInChildren<BlockAudio>();
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();

        color_initial = mesh.material.color;
        color = color_initial;
    }
    private void Update()
    {
    }
    private void OnTriggerEnter(Collider collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null)
        {
            //block_audio.PlayBlockCollision();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Block b = collision.collider.GetComponent<Block>();
        if (b != null)
        {
            block_audio.PlayBlockCollision(Mathf.Min(1, collision.relativeVelocity.magnitude / 20f));
        }
        else if (collision.collider.CompareTag("Floor"))
        {
            block_audio.PlayFloorCollision(Mathf.Min(1, collision.relativeVelocity.magnitude / 10f));
        }
    }
    private void SetColor(Color color)
    {
        this.color = color;
        mesh.material.color = color;
    }
    private IEnumerator UpdateRemove()
    {
        // Create placemarker to show who won the block
        CreateStump();

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 12; ++i)
        {
            mesh.enabled = !mesh.enabled;
            yield return new WaitForSeconds(0.075f);
        }

        float t = 0;
        float scale = 1;
        while (true)
        {
            t += Time.deltaTime;
            scale = Mathf.Lerp(1, 0, t*t);

            if (t >= 1)
            {
                Destroy(gameObject);
                break;
            }
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
    private void CreateStump()
    {
        BlockStump bs = Instantiate(stump_prefab);
        bs.Initialize(pos_initial, color);
    }

    public void Initialize(string word, Vector3 pos_initial)
    {
        text.text = word;

        this.pos_initial = pos_initial;
        transform.position = pos_initial;
    }
    public void Reset()
    {
        if (removing) return;
        transform.position = pos_initial;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        hit = false;
        SetColor(color_initial);

        rb.useGravity = false;
        rb.mass = 1.5f;
        rb.angularDrag = 2f;
        rb.drag = 0.3f;
    }
    public void Hit(Player hitter, bool apply_force)
    {
        if (hit) return;

        hit = true;
        
        rb.useGravity = true;
        rb.mass = 0.1f;
        rb.angularDrag = 0.1f;
        rb.drag = 0.1f;

        if (apply_force)
        {
            Vector3 dir = transform.position - hitter.transform.position;
            dir.y = 0.2f;
            rb.AddForce(dir * 150f);
        }
        
        SetColor(hitter.GetPlayerColor());
        if (event_hit != null) event_hit(hitter.GetPlayerNumber());
    }
    public void Hit(Player hitter)
    {
        Hit(hitter, true);
    }
    public void HitNeutral()
    {
        if (hit) return;
        hit = true;
        rb.useGravity = true;
        if (event_hit != null) event_hit(-1);
    }
    public void Remove()
    {
        removing = true;
        StartCoroutine(UpdateRemove());
    }
    public void SetTextVisible(bool visible)
    {
        text.gameObject.SetActive(visible);
    }

    public Color GetColor()
    {
        return color;
    }
    public bool IsHit()
    {
        return hit;
    }
}
