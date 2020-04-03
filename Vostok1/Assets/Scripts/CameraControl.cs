using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _rocket;

    private const float SPEED = 2f;

    public float zoomSensitivity = 15.0f;
    public float zoomSpeed = 5.0f;
    public float zoomMin = 5.0f;
    public float zoomMax = 80.0f;

    private float zoom;

    private void Start()
    {
        zoom = _camera.fieldOfView;
    }

    private void Update()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
    }

    private void LateUpdate()
    {
        _camera.transform.RotateAround(_rocket.transform.position, Vector3.up, Input.GetAxis("Mouse X") * SPEED);
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, zoom, Time.deltaTime * zoomSpeed);
    }

    
}
