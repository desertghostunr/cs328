using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    // UI Input
    public string moveAxisName;
    public string rotateAxisName;
    public float speedMuliplier = 10.0f;
    public float rotateMulitpler = 10.0f;
    public float maxTurnAngle = 30.0f;

    // Input
    private float rotateAxis, moveAxis;

    // Physics
    private Rigidbody rigid;

	void Start()
    {
        rigid = GetComponent<Rigidbody>();
	}
	
	void Update()
    {
        // Take input
        moveAxis = Input.GetAxis(moveAxisName);
        rotateAxis = Input.GetAxis(rotateAxisName);
	}

    void FixedUpdate()
    {
        rigid.AddForce(transform.right * moveAxis * speedMuliplier);
        rigid.AddTorque(transform.up * rotateAxis * rotateMulitpler);
    }
}
