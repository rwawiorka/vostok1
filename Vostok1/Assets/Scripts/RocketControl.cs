using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private StartManager _firstStageManager;


    public bool IsRocketOn { get; private set; } = true;

    private void Update()
    {
        if (!_firstStageManager.RocketCanStart) return;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            IsRocketOn = !IsRocketOn;
        }
        if (IsRocketOn)
        {
            GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * Time.deltaTime * _firstStageManager.RocketForce);
        }

        if (Input.GetAxis("Horizontal") == 1 || Input.GetAxis("Horizontal") == -1)
        {
            GetComponent<Rigidbody>().AddTorque(transform.right * Time.deltaTime * Input.GetAxis("Horizontal") * 300);
        }

        if (Input.GetAxis("Rotation") == 1 || Input.GetAxis("Rotation") == -1)
        {
            GetComponent<Rigidbody>().AddTorque(transform.up * Time.deltaTime * Input.GetAxis("Rotation") * 300);
        }

        if (Input.GetAxis("Vertical") == 1 || Input.GetAxis("Vertical") == -1)
        {
            GetComponent<Rigidbody>().AddTorque(transform.forward * Time.deltaTime * Input.GetAxis("Vertical") * 300);
        }
    }   

    public bool IsControlKeyDown()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) ||
               Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E);
    }
}
