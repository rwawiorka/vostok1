using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloudsCreator : MonoBehaviour
{
    [SerializeField] private ParticleSystem _cloud;
    [SerializeField] private GameObject _camera;

    private const float SPEED = 10f;

    public void ToggleClouds(bool toggle)
    {
        _cloud.gameObject.SetActive(toggle);
    }

    private void Update()
    {
        // var pos = _cloud.transform.position;
        // pos.x = _camera.transform.position.x;
        // pos.x = _camera.transform.position.z;
        // _cloud.transform.position = pos;
        
        _cloud.transform.position = _camera.transform.position + new Vector3(0, 2, 0);
        _cloud.transform.rotation = _camera.transform.rotation;
    }

    public void IncreaseCloudEmission()
    {
        var emissionModule = _cloud.emission;
        emissionModule.rateOverDistance = 6;
    }
}
