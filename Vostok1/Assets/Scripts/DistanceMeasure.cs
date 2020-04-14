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

    private Vector3 lastPos;

    private void Start()
    {
        lastPos = _rocket.transform.position;
    }
    
    private void Update()
    {
        MeasureDistanceAndSpeed();
        Debug.Log(Distance + " " + Speed);
    }

    private void MeasureDistanceAndSpeed()
    {
        Ray ray = new Ray(_rocket.transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out var hit))
        {
            Distance = (decimal) (hit.distance / 100);
            Distance = TruncateDecimal(Distance, 2);
        }

        Speed = (decimal) ((_rocket.transform.position - lastPos).magnitude * 1000);
        Speed = Math.Truncate(Speed);
        lastPos = _rocket.transform.position;

    }

    private decimal TruncateDecimal(decimal value, int precision)
    {
        decimal step = (decimal) Math.Pow(10, precision);
        decimal tmp = Math.Truncate(step * value);
        return tmp / step;
    }
}
