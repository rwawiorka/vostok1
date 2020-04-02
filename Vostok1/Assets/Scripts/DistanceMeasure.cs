using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure : MonoBehaviour
{
    [SerializeField] private GameObject _floor;
    [SerializeField] private GameObject _rocket;

    private Vector3 _rocketPos;
    private Vector3 _floorPos;

    private float _distance;

    private void Start()
    {
        _rocketPos = _rocket.transform.position;
        _floorPos = _floor.transform.position;
    }

    private void Update()
    {
        MeasureDistance();
        Debug.Log(_distance);
    }

    private void MeasureDistance()
    {
        _distance = Vector3.Distance(_rocketPos, _floorPos);
    }
}
