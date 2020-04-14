using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _rocketHolders;
    [SerializeField] private GameObject _rocket;
    [SerializeField] private RocketControl _rocketControl;
    [SerializeField] private ParticleSystem[] _flames;
    [SerializeField] private DistanceMeasure _distanceMeasure;
    [SerializeField] private BackgroundDimmer _background;
    [SerializeField] private CloudsCreator _cloudsCreator;

    private const decimal DISTANCETOSPACE = 5;
    private const decimal DISTANCETOCREATECLOUDS = 2;
    private const decimal DISTANCETOINCREASEEMISSION = 3.5M;

    public bool IsRocketInSpace { get; private set; }

    public bool RocketCanStart { get; private set; }

    private bool _droppedBoosters;


    private void Start()
    {
        foreach (var flame in _flames)
        {
            flame.GetComponent<Transform>().localScale = new Vector3(.2f, .2f, .2f);
        }
    }

    private async void Update()
    {
        // First stage - Release the rocket holders

        if (Input.GetKeyDown(KeyCode.Space) && !RocketCanStart)
        {
            ReleaseTheHolders();
            foreach (var flame in _flames)
            {
                flame.GetComponent<Transform>().DOScale(Vector3.one, 2f);
            }
            RocketCanStart = true;
            _background.FadeDown();
            await HoldRocketRotationAsync();
        }

        if (!IsRocketInSpace)
        {
            if (RocketCanStart)
            {
                ToggleFlames(_rocketControl.IsRocketOn);
            }

            if (_distanceMeasure.Distance >= DISTANCETOSPACE)
            {
                IsRocketInSpace = true;
            }

            if (_distanceMeasure.Distance >= DISTANCETOCREATECLOUDS)
            {
                _cloudsCreator.ToggleClouds(true);
                DropElement(0, 3); // drop boosters;
            }

            if (_distanceMeasure.Distance >= DISTANCETOINCREASEEMISSION)
            {
                _cloudsCreator.IncreaseCloudEmission();
            }
        }
        else
        {
            SceneManager.LoadScene("SpaceScene");
            _cloudsCreator.ToggleClouds(false);
        }
        
    }

    private void ReleaseTheHolders()
    {
        // Animation of holders to release the rocket.
        foreach (var rocketHolder in _rocketHolders)
        {
            rocketHolder.transform.DORotate(
                new Vector3(rocketHolder.transform.localRotation.x + 14, rocketHolder.transform.localRotation.y,
                    rocketHolder.transform.localRotation.z + 30), 5f, RotateMode.LocalAxisAdd);
        }
    }

    private async Task HoldRocketRotationAsync()
    {
        while (true)
        {
            if (_rocketControl.IsControlKeyDown())
                break;
            _rocket.transform.rotation = new Quaternion(0f, _rocket.transform.rotation.y, 0f, 1f);
            await Task.Delay(10);
        }
    }

    private void ToggleFlames(bool isRocketOn)
    {
        foreach (var flame in _flames)
        {
            if (isRocketOn)
            {
                // check only x axis to prevent multiplied checking
                if (flame.GetComponent<Transform>().localScale.x > 0f) return;
                flame.GetComponent<Transform>().DOScale(Vector3.one, 2f);
            }
            else
            {
                if (flame.GetComponent<Transform>().localScale.x < 1f) return;
                flame.GetComponent<Transform>().DOScale(Vector3.zero, 1f);
            }
        }
    }

    private void DropElement(int startIndex, int endIndex)
    {
        if (_droppedBoosters) return;
        for (var i = startIndex; i <= endIndex; i++)
        {
            _rocket.transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
            _rocket.transform.GetChild(0).parent = null;
        }

        _droppedBoosters = true;
    }
}