using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    [SerializeField] private Transform _gyroscopePivot;
    [SerializeField] private GameObject _rocket;


    private void Update()
    {
        _gyroscopePivot.rotation = _rocket.GetComponent<Rigidbody>().rotation;
    }
}
