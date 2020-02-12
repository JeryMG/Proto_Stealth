using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action FinAtteinte;
    
    [SerializeField] private float moveSpeed = 7f;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8f;

    private float angle;
    private float smoothInputMagnitude;
    private float smoothMoveVelocity;
    private Vector3 velocity;
    
    private Rigidbody _rigidbody;

    private bool choppe;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Guard.GardSpotPlayer += Choppe;
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;

        if (!choppe)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    private void FixedUpdate()
    {
        _rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        _rigidbody.MovePosition(_rigidbody.position + velocity * Time.deltaTime);
    }

    void Choppe()
    {
        choppe = true;
    }

    private void OnDestroy()
    {
        Guard.GardSpotPlayer -= Choppe;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Choppe();
            if (FinAtteinte != null)
            {
                FinAtteinte();
            }
        }
    }
}
