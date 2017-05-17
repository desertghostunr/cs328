using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalCharacterController : MonoBehaviour
{

    public string ForwardMotionInput;
    public string TurnMotionInput;

    public float moveSpeed = 14.0f;
    public float rotSpeed = 80.0f;

    public float reverseSpeedInhibitor = 0.25f;

    private bool m_canMove = true;

    private float m_forwardMultiplier;
    private float m_rotMultiplier;

    private float m_movementInhibitor;

    

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    
}
