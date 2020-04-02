using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private StartManager _firstStageManager;
    [SerializeField] private GameObject _rocket;

    private Rigidbody _rocketRigidbody;

    private void Start()
    {
        _rocketRigidbody = _rocket.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_firstStageManager.RocketCanStart)
        {
            //TODO: Zmienić ciąg
            _rocketRigidbody.AddRelativeForce(Vector3.up * Time.deltaTime * 500);

            if (Input.GetAxis("Horizontal") == 1 || Input.GetAxis("Horizontal") == -1)
            {
                _rocketRigidbody.AddTorque(Vector3.back * Time.deltaTime * 20 * Input.GetAxis("Horizontal"));  
            }

            if (Input.GetAxis("Vertical") == 1 || Input.GetAxis("Vertical") == -1)
            {
                _rocketRigidbody.AddTorque(Vector3.right * Time.deltaTime * 20 * Input.GetAxis("Vertical"));
            }

            if (Input.GetAxis("Rotation") == 1 || Input.GetAxis("Rotation") == -1)
            {
                _rocketRigidbody.AddTorque(Vector3.down * Time.deltaTime * 10 * Input.GetAxis("Rotation"));
            }
        }
    }

    public bool IsControlKeyDown()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            return true;
        return false;
    }
}
