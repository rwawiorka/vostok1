using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

public class EarthManager : MonoBehaviour
{
    [SerializeField] private GameObject _line;
    [SerializeField] private float _radius;
    [SerializeField] private float _lineWidth;
    [SerializeField] private Material _material;
    private async void Start()
    {
        DrawCircle(_line, _radius, _lineWidth);
        await Task.Delay(5000);
        gameObject.AddComponent<SphereCollider>().radius = 10;
        
    }

    private void Update()
    {
        DrawCircle(_line, _radius, _lineWidth);
    }

    public void DrawCircle(GameObject container, float radius, float lineWidth)
    {
        var segments = 720;
        var line = container.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = container.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = segments + 1;
            line.material = _material;
        }
        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
    }
}
