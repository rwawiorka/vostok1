using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure : MonoBehaviour
{
    public decimal Distance;
    public decimal Speed;

    [SerializeField] private GameObject _floor;
    [SerializeField] private GameObject _rocket;
    [SerializeField] private StartManager _startManager;

    private Vector3 lastPos;

    private void Start()
    {
        lastPos = _rocket.transform.position;
    }
    
    private void Update()
    {
        if (_startManager.IsRocketInSpace)
        {
            MeasureDistanceAndSpeedInSpace();
        }
        else
        {
            MeasureDistanceAndSpeedOnEarth();
        }
        
    }

    private void MeasureDistanceAndSpeedOnEarth()
    {
        Distance = (decimal) Vector3.Distance(_rocket.transform.position, _floor.transform.position);
        Distance -= 69.23332M; // start distance from floor to rocket
        Distance /= 100; // estimate real distance
        Distance = TruncateDecimal(Distance, 2);
        Speed = (decimal) ((_rocket.transform.position - lastPos).magnitude * 900); // estimate real speed
        Speed = Math.Truncate(Speed);
        lastPos = _rocket.transform.position;
    }

    private void MeasureDistanceAndSpeedInSpace()
    {
        if (_floor == null)
        {
            _floor = GameObject.Find("Earth");
        }
        // Distance = TruncateDecimal((decimal)_floor.GetComponent<EarthGravity>().Distance, 2);
        Distance = (decimal)_floor.GetComponent<EarthGravity>().Distance;
        Distance = TruncateDecimal(Distance, 1);
        Debug.Log(_floor + " " + Distance);
    }

    private decimal TruncateDecimal(decimal value, int precision)
    {
        decimal step = (decimal) Math.Pow(10, precision);
        decimal tmp = Math.Truncate(step * value);
        return tmp / step;
    }
}
