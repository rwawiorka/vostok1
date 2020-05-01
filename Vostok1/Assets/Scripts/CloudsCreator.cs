using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloudsCreator : MonoBehaviour
{
    [SerializeField] private ParticleSystem _cloud;
    [SerializeField] private GameObject _camera;
    [SerializeField] private DistanceMeasure _distanceMeasure;

    private float _timeLeft = 5f;

    public bool IsCloudToggled { get; private set; }

    public void ToggleClouds(bool toggle)
    {
        IsCloudToggled = toggle;
        _cloud.gameObject.SetActive(toggle);
        if(toggle)
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
            var emmissionModule = _cloud.emission;
            emmissionModule.rateOverDistance = increase;
            increase++;
            yield return new WaitForSeconds(1f);
        }
    }

    public void DecreaseCloudEmission()
    {
        StartCoroutine(DecreaseClouds());
    }

    private IEnumerator DecreaseClouds()
    {

        _cloud.gameObject.SetActive(true);
        IsCloudToggled = true;
        while (_timeLeft > 0)
        {
            var emissionModule = _cloud.emission;
            emissionModule.rateOverDistance = _timeLeft;
            _timeLeft -= 1;
            yield return new WaitForSeconds(1f);
        }
    }
}
