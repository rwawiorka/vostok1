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
            _rocketRigidbody.AddForce(Vector3.up * Time.deltaTime * 1000, ForceMode.Acceleration);
        }
    }
}
