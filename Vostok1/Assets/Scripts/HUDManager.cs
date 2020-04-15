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

    private void Start()
    {
        _speedText.text = "Speed: 0km/h";
        _distanceText.text = "Distance: 0km";
    }

    private void Update()
    {
        if (!_startManager.RocketCanStart) return;
        if (_distance.Speed != 0)
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

    }
}
