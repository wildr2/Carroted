using UnityEngine;
using System.Collections;

public class HumanPlayerController : PlayerController
{
    private int control_scheme = 1;
    private GameManager match;

	private void Update()
    {
        // Locomotion
        float h = Input.GetAxis("H" + control_scheme);
        float v = Input.GetAxis("V" + control_scheme);
        InputMove = new Vector2(h, v);

        if (Input.GetButtonDown("Go" + control_scheme)
            && match.GetGameState() != GameState.MemorizationTime
            && InputSwing != null) InputSwing();
    }

    public void Initialize(int control_scheme, GameManager match)
    {
        this.control_scheme = control_scheme;
        this.match = match;
    }
}
