using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloudsCreator : MonoBehaviour
{
    [SerializeField] private ParticleSystem _cloud;
    [SerializeField] private GameObject _camera;
    [SerializeField] private DistanceMeasure _distanceMeasure;

    private const float SPEED = 10f;
    private bool _isCloudToggled;

    public void ToggleClouds(bool toggle)
    {
        if (_isCloudToggled) return;
        _cloud.gameObject.SetActive(toggle);
        _isCloudToggled = true;
        StartCoroutine(ToggleClouds());
    }

    private void Update()
    {
        _cloud.transform.position = _camera.transform.position + new Vector3(0, 2, 0);
        _cloud.transform.rotation = _camera.transform.rotation;
    }

    // public void IncreaseCloudEmission()
    // {
    //     var emissionModule = _cloud.emission;
    //     emissionModule.rateOverDistance = 6;
    // }

    private IEnumerator ToggleClouds()
    {
        int increase = 1;
        while (_distanceMeasure.Distance >= StartManager.DISTANCETOCREATECLOUDS &&
               _distanceMeasure.Distance <= StartManager.DISTANCETOINCREASEEMISSION)
        {
            increase++;
            var emmissionModule = _cloud.emission;
            emmissionModule.rateOverDistance = increase;
            yield return new WaitForSeconds(1f);
        }
    }
}
