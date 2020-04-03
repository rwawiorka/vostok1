using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure : MonoBehaviour
{
    [SerializeField] private GameObject _floor;
    [SerializeField] private GameObject _rocket;

    private Vector3 lastPos;

    private float _distance;
    private float _speed;

    private void Start()
    {
        lastPos = _rocket.transform.position;
    }
    
    private void Update()
    {
        MeasureDistanceAndSpeed();
        // Debug.Log("Distance: " + _distance + "||| Speed: " + _speed);
    }

    private void MeasureDistanceAndSpeed()
    {
        Ray ray = new Ray(_rocket.transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out var hit))
        {
            _distance = hit.distance / 100;
        }

        _speed = (_rocket.transform.position - lastPos).magnitude * 1000;
        lastPos = _rocket.transform.position;

    }
}
