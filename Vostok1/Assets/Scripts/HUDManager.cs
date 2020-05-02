using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private StartManager _startManager;

    [SerializeField] private DistanceMeasure _distance;
    
    [SerializeField] private Text _speedText;
    [SerializeField] private Text _distanceText;
    [SerializeField] private Text _fuelText;

    private void Start()
    {
        _speedText.text = "Speed: 0km/h";
        _distanceText.text = "Distance: 0km";
        _fuelText.text = "Boosters Fuel: " + _startManager.BoostersFuel + "l";
    }

    private void Update()
    {
        if (!_startManager.RocketCanStart) return;

        if (_startManager.StartStage || _startManager.LandingStage)
        {
            _distanceText.color = _speedText.color = _fuelText.color = Color.white;
        }

        else
        {
            _distanceText.color = _speedText.color = _fuelText.color = Color.white;
        }


        if (_distance.Speed != 0 && _distance.Speed != 5000)
        {
            _speedText.text = "Speed: " + _distance.Speed + "km/h";
        }

        switch (_distance.Distance.ToString().Length)
        {
            case 1:
                _distanceText.text = "Distance: " + _distance.Distance + ",00km";
                break;
            case 3:
                _distanceText.text = "Distance: " + _distance.Distance + "0km";
                break;
            default:
                _distanceText.text = "Distance: " + _distance.Distance + "km";
                break;
        }

        if (_startManager.BoostersFuel > 0)
        {
            _fuelText.text = "Boosters Fuel: " + Math.Truncate(_startManager.BoostersFuel) + "l";
        }
        else if (_startManager.RocketFuel > 0)
        {
            _fuelText.text = "Rocket Fuel: " + Math.Truncate(_startManager.RocketFuel) + "l";
        }
        else if (_startManager.CapsuleBoosterFuel > 0)
        {
            _fuelText.text = "Capsule Booster Fuel: " + Math.Truncate(_startManager.CapsuleBoosterFuel) + "l";
        }
        else
        {
            _fuelText.text = "No fuel";
        }
        
    }
}
