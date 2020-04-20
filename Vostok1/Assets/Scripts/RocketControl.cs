﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private StartManager _firstStageManager;
    [SerializeField] private GameObject _rocket;
    [SerializeField] private Transform camera;

    private Rigidbody _rocketRigidbody;

    public bool IsRocketOn { get; private set; } = true;

    private void Start()
    {
        _rocketRigidbody = _rocket.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_firstStageManager.RocketCanStart) return;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            IsRocketOn = !IsRocketOn;
        }
        if (IsRocketOn)
        {
            _rocketRigidbody.AddRelativeForce(transform.up * Time.deltaTime * _firstStageManager.RocketForce);
        }

        if (Input.GetAxis("Horizontal") == 1 || Input.GetAxis("Horizontal") == -1)
        {
            _rocketRigidbody.AddTorque(Vector3.forward * Time.deltaTime * 50 * Input.GetAxis("Horizontal"));  
        }
        
        if (Input.GetAxis("Vertical") == 1 || Input.GetAxis("Vertical") == -1)
        {
            _rocketRigidbody.AddTorque(Vector3.right * Time.deltaTime * 50 * Input.GetAxis("Vertical"));
        }
        
        if (Input.GetAxis("Rotation") == 1 || Input.GetAxis("Rotation") == -1)
        {
            _rocketRigidbody.AddTorque(Vector3.down * Time.deltaTime * 50 * Input.GetAxis("Rotation"));
        }
    }   

    public bool IsControlKeyDown()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) ||
               Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E);
    }
}
