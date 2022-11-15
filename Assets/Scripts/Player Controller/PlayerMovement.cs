using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private Transform orientation;

    private float horiInput;
    private float vertInput;

    private Vector3 moveDir;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        horiInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        moveDir = orientation.forward * vertInput + orientation.right * horiInput;

        rb.AddForce(moveDir.normalized * speed, ForceMode.Force);
    }
}
