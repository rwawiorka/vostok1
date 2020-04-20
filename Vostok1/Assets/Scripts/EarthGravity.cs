using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGravity : MonoBehaviour
{
    [SerializeField] private GameObject _rocket;

    private const float G = 6.7f;

    private Rigidbody _rocketRb;
    private Rigidbody _thisRb;
    private void Start()
    {
        _rocket = GameObject.Find("Rocket");
        if (_rocket == null) return;
        _rocketRb = _rocket.GetComponent<Rigidbody>();
        _thisRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_thisRb == null) return; 
        Gravity();
    }

    private void Gravity()
    {
        if (_rocket == null) return;
        float distance = Vector3.Distance(transform.position, _rocket.transform.position);
        float force = (_thisRb.mass * _rocketRb.mass * 1000) / (distance * distance);
        _rocketRb.AddForce((transform.position - _rocket.transform.position).normalized * force);
    }


}
