using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    public Vector2 InputMove { get; protected set; }
    public Action InputSwing { get; set; }


    private void Start()
    {
        InputMove = Vector2.zero;
    }
    
}
