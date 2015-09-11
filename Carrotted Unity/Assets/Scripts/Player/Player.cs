using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
    private int player_num = 1;
    private string player_name = "Player";
    private Color player_color = Color.red;

    private Rigidbody rb;
    public SpriteRenderer sprite_hand_inner, sprite_str_indicator;
    private LineTrailRenderer trail;
    private Collider hand_collider;
    private HandAudio hand_audio;

    private PlayerController pc;



    // movement
    private Vector3 start_pos, pos_last;
    private bool has_control = false;
    private static float kb_force = 2500f;
    private bool has_moved = false, going = false, gone = false;
    public Action event_go, event_gone;

    // strength (energy to move)
    private float strength;
    private static float strength_max = 20;
    private static float strength_rate_going_idle = 0.1f;
    private static float strength_rate_going_dist = 1;
    private static float strength_rate_idle = 0.05f;
    private static float strength_rate_dist = 0.35f;
    private static float strength_rate_per_block = 0.5f;

    
    // PRIVATE MODIFIERS

    private void Start()
    {
        // Physics
        rb = GetComponent<Rigidbody>();
        hand_collider = GetComponent<Collider>();
        hand_collider.isTrigger = false; // allow collision with player walls before swinging

        // Trail
        trail = GetComponent<LineTrailRenderer>();
        trail.SetEmisionEnabled(false);

        // Audio
        hand_audio = GetComponentInChildren<HandAudio>();

        // Colors
        sprite_hand_inner.color = player_color;
        Color c = player_color;
        c.a = 0;
        trail.GetLine().SetColors(player_color, c);

        // Position
        start_pos = transform.position;
        pos_last = start_pos;

        // Strengh
        SetStrength(strength_max);
    }
    private void Update()
    {
        if (!has_control) return;

        // Update Strength
        float dist_travelled = Vector3.Distance(transform.position, pos_last);

        if (going)
        {
            if (SetStrength(strength - Mathf.Max(dist_travelled * strength_rate_going_dist,
                strength_rate_going_idle)))
                return;
        }
        else if (has_moved)
        {
            if (SetStrength(strength - Mathf.Max(dist_travelled * strength_rate_dist,
                strength_rate_idle)))
                return;
        }
        else
        {
            if (dist_travelled > 0.01f) has_moved = true;
        }

        // Save last pos for distance travelled calculation
        pos_last = transform.position;
    }
    private void FixedUpdate()
    {
        if (!has_control) return;

        Vector3 axis = new Vector3(pc.InputMove.x, 0, pc.InputMove.y);
        //float target_rot_y = -(Mathf.Atan2(input_v, input_h) * Mathf.Rad2Deg) + 90;
        //rot_y = Mathf.LerpAngle(rot_y, target_rot_y, Time.deltaTime * 5f);
        //transform.rotation = Quaternion.Euler(0, target_rot_y, 0);

        rb.AddForce(axis * Time.deltaTime * 5f * 8500f * axis.magnitude);
    }
    private void StartSwing()
    {
        if (IsGoingOrGone()) return;

        if (event_go != null) event_go();
        going = true;

        hand_audio.PlaySwinging();

        trail.Clear();
        trail.SetEmisionEnabled(true);
        hand_collider.isTrigger = true; // allow collision with blocks

        rb.transform.position = new Vector3(rb.position.x, going ? start_pos.y - 2 : start_pos.y, rb.position.z);
    }
    private void OnTriggerEnter(Collider hand_collider)
    {
        //if (!has_control) return;

        Block block = hand_collider.GetComponent<Block>();
        if (block != null)
        {
            block.Hit(this);
            SetStrength(strength - strength_rate_per_block);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Player p = collision.collider.GetComponent<Player>();
        if (p != null)
        {
            p.rb.AddForceAtPosition(-collision.contacts[0].normal * kb_force, collision.contacts[0].point);
        }
    }
    private bool SetStrength(float value)
    {
        strength = Mathf.Max(value, 0);

        // Visual
        float s = 1 - (strength / strength_max);
        sprite_str_indicator.transform.localScale = new Vector3(s, s, s);

        // Depletion
        if (strength <= 0)
        {
            // End Going
            has_control = false;

            going = false;
            gone = true;

            if (event_gone != null) event_gone();
            return true;
        }

        return false;
    }


    // PUBLIC MODIFIERS

    public void Inititalize(int number, string name, Color color, bool ai, int control_scheme, GameManager manager)
    {
        player_num = number;
        player_name = name;
        player_color = color;

        // Player controller
        if (ai)
        {
            gameObject.AddComponent<AIPlayerController>();
            GetComponent<AIPlayerController>().Initialize(this, manager);
        }
        else
        {
            gameObject.AddComponent<HumanPlayerController>();
            GetComponent<HumanPlayerController>().Initialize(control_scheme, manager);
        }
        this.pc = GetComponent<PlayerController>();
        pc.InputSwing += StartSwing;
    }
    public void SetHasControl(bool has_control)
    {
        this.has_control = has_control;
    }
    public void Reset()
    {
        has_moved = false;
        going = false;
        gone = false;
        has_control = true;

        // Collision
        hand_collider.isTrigger = false;  // allow collision with player walls before swinging

        // Audio
        hand_audio.Reset();

        // Trail
        trail.Clear();
        trail.SetEmisionEnabled(false);

        // Position
        transform.position = start_pos;
        pos_last = start_pos;

        // Strength
        SetStrength(strength_max);
    }


    // PUBLIC ACCESSORS

    public bool HasGone()
    {
        return gone;
    }
    public bool IsGoingOrGone()
    {
        return going || gone;
    }
    public string GetPlayerName()
    {
        return player_name;
    }
    public int GetPlayerNumber()
    {
        return player_num;
    }
    public Color GetPlayerColor()
    {
        return player_color;
    }
}
