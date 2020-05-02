using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGravity : MonoBehaviour
{
    [SerializeField] private GameObject _rocket;

    public float Distance { get; private set; }

    private Rigidbody _rocketRb;
    private Rigidbody _thisRb;
    private void Start()
    {
        _rocket = GameObject.Find("Rocket");
        _rocketRb = _rocket.GetComponent<Rigidbody>();
        _thisRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    private void Gravity()
    {
        Distance = Vector3.Distance(transform.position, _rocket.transform.position);
        float force = (_thisRb.mass * _rocketRb.mass * 10) / (Distance * Distance);
        _rocketRb.AddForce((transform.position - _rocket.transform.position).normalized * force);
    }


}
