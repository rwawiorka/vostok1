﻿using System;
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
        MeasureDistanceAndSpeed();
    }

    private void MeasureDistanceAndSpeed()
    {
        if (_startManager.IsRocketInSpace)
        {
            _floor = GameObject.Find("Earth");
        }
        Distance = (decimal) Vector3.Distance(_rocket.transform.position, _floor.transform.position);
        Distance -= 69.23332M;
        Distance /= 100;
        Distance = TruncateDecimal(Distance, 2);
        Speed = (decimal) ((_rocket.transform.position - lastPos).magnitude * 900);
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
