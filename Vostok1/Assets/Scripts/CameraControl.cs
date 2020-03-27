using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _rocket;

    private const float SPEED = 2f;

    private void LateUpdate()
    {
        _camera.transform.RotateAround(_rocket.transform.position, Vector3.up, Input.GetAxis("Mouse X") * SPEED);
        _camera.transform.RotateAround(_rocket.transform.position, Vector3.right, Input.GetAxis("Mouse Y") * SPEED);
    }
}
