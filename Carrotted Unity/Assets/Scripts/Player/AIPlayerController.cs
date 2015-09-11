using UnityEngine;
using System.Collections;

//private enum AIState { Chasing,  }

public class AIPlayerController : PlayerController
{
    private Player player;
    private Player opponent;
    private GameManager manager;

    private float inaccuracy = 0.8f;
    private float swing_dist_min = 3f, swing_dist_max = 8f;
    private float reaction_time_min = 0.5f, reaction_time_max = 2f;

    private Vector3 move_target;
    private float swing_dist;
    private bool reacting = false;
    private bool need_reset = true;


    private void Start()
    {
    }
    private void Update()
    {
        if (manager.GetGameState() == GameState.Reading && !reacting && manager.block_manager.IsWritingReadWord())
        {
            StartCoroutine(StartMoveAfterRT());
        }
        else if (manager.GetGameState() == GameState.PreRead)
        {
            if (need_reset)
            {
                Reset();
            }
        }
    }

    private IEnumerator StartMoveAfterRT()
    {
        reacting = true;
        float rt = Random.Range(reaction_time_min, reaction_time_max);
        yield return new WaitForSeconds(rt);
        StartMove();
    }
    private void StartMove()
    {
        move_target = manager.block_manager.GetReadBlock().transform.position;
        swing_dist = Random.Range(swing_dist_min, swing_dist_max);

        need_reset = true;
        StartCoroutine(UpdateSwing());
    }
    private IEnumerator UpdateSwing()
    {
        while (!player.HasGone())
        {
            Vector3 dif = move_target - transform.position;
            float angle = (Vector3.Angle(Vector3.right, dif)) * Mathf.Deg2Rad;
            angle += (Random.value * 45f - 10f) * inaccuracy * Mathf.Deg2Rad;
            Vector3 inaccurate_dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));


            if (!manager.block_manager.GetReadBlock().IsHit())
            {
                InputMove = new Vector2(inaccurate_dir.x, inaccurate_dir.z).normalized;
            }

            if (dif.magnitude < swing_dist && !player.IsGoingOrGone())
                InputSwing();



            Debug.DrawLine(transform.position, move_target, Color.red, 0.05f);
            Debug.DrawLine(transform.position, transform.position + new Vector3(InputMove.x, 0, InputMove.y) * dif.magnitude, Color.white, 0.05f);

            yield return new WaitForSeconds(0.05f);
        }
    }
    private void Reset()
    {
        StopAllCoroutines();
        reacting = false;
        need_reset = false;

        InputMove = Vector2.zero;
    }



    public void Initialize(Player player, GameManager manager)
    {
        this.player = player;
        this.opponent = manager.GetOpponent(player.GetPlayerNumber());
        this.manager = manager;
    }
}
