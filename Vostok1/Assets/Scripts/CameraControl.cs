using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] GameObject target;

    [Header("Speed")]
    [SerializeField] float moveSpeed = 300f;
    [SerializeField] float zoomSpeed = 100f;

    [Header("Zoom")]
    [SerializeField] float minDistance = 2f;
    [SerializeField] float maxDistance = 5f;

    private void Update()
    {
        CameraController();
    }

    private void CameraController()
    {
        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(target.transform.position, Vector3.up, ((Input.GetAxisRaw("Mouse X") * Time.deltaTime) * moveSpeed));
            transform.RotateAround(target.transform.position, transform.right, -((Input.GetAxisRaw("Mouse Y") * Time.deltaTime) * moveSpeed));
        }

        ZoomCamera();
    }

    private void ZoomCamera()
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= minDistance && Input.GetAxis("Mouse ScrollWheel") > 0f) { return; }
        if (Vector3.Distance(transform.position, target.transform.position) >= maxDistance && Input.GetAxis("Mouse ScrollWheel") < 0f) { return; }

        transform.Translate(
            0f,
            0f,
            (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomSpeed,
            Space.Self
        );
    }


}
