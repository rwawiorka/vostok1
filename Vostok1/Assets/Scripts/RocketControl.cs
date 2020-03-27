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
        }
    }
}
