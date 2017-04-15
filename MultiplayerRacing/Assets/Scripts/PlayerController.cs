using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    // UI Input
    public float speedMuliplier = 10.0f;
    public float rotateMulitpler = 10.0f;
    public float maxTurnAngle = 30.0f;

    // Input
    private float horiAxis, vertAxis;

    // Physics
    private Rigidbody rigid;

	void Start()
    {
        rigid = GetComponent<Rigidbody>();
	}
	
	void Update()
    {
        // Take input
        horiAxis = Input.GetAxis("Horizontal");
        vertAxis = Input.GetAxis("Vertical");
	}

    void FixedUpdate()
    {
        rigid.AddForce(transform.right * vertAxis * speedMuliplier);
        rigid.AddTorque(transform.up * horiAxis * rotateMulitpler);
    }
}
