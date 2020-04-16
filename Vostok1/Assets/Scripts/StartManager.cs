using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
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

    public const decimal DISTANCETOSPACE = 6.5M;
    public const decimal DISTANCETOCREATECLOUDS = 3.5M;
    public const decimal DISTANCETOINCREASEEMISSION = 5.5M;

    public bool IsRocketInSpace { get; private set; }

    public bool RocketCanStart { get; private set; }

    public float RocketForce { get; private set; }

    public float BoostersFuel { get; private set; }

    public float RocketFuel { get; private set; }

    private bool _droppedBoosters;
    private bool _droppedMainEngine;
    private bool _droppedCapsuleBooster;

    private bool _spaceVariablesInitialized;

    private List<Transform> _boosters;

    private void Start()
    {
        _boosters = new List<Transform>();

        foreach (var flame in _flames)
        {
            flame.GetComponent<Transform>().localScale = new Vector3(.2f, .2f, .2f);
        }

        RocketForce = 4500;
        RocketFuel = 1500;
        BoostersFuel = 2000;
    }

    private async void Update()
    {
        // First stage - Release the rocket holders
        if (!IsRocketInSpace)
        {
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

            if (Input.GetKeyDown(KeyCode.F3))
            {
                FlyToSpace();
            } 

            if (!_droppedBoosters && (Input.GetKeyDown(KeyCode.Space) || _distanceMeasure.Distance >= DISTANCETOCREATECLOUDS || BoostersFuel <= 0))
            {
                DropBoosters();
                BoostersFuel = 0;
                RocketForce -= 1125;
                _droppedBoosters = true;
            }

            if (RocketCanStart)
            {
                ToggleFlames(_rocketControl.IsRocketOn);
            }
            
            if (_distanceMeasure.Distance >= DISTANCETOSPACE)
            {
                if (SceneManager.GetActiveScene().name == "SpaceScene") return;
                foreach (var booster in _boosters)
                {
                    DestroyImmediate(booster.gameObject);
                }
                FlyToSpace();

            }
            
            if (_distanceMeasure.Distance >= DISTANCETOCREATECLOUDS)
            {
                if(!_cloudsCreator.IsCloudToggled)
                    _cloudsCreator.ToggleClouds(true);
            }
        }

        else
        { 
            await Task.Delay(5000);
            _cloudsCreator.ToggleClouds(false);
        }

        if (_rocketControl.IsRocketOn)
        {
            if (BoostersFuel > 0 && !_droppedBoosters) // stage 1;
            {
                BoostersFuel -= Time.deltaTime * 50;
            }
            else if (RocketFuel > 0 && !_droppedMainEngine) // stage 2;
            {
                RocketFuel -= Time.deltaTime * 20;
            }
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

    private void DropBoosters()
    {
        if (_droppedBoosters) return;
        var sequence = DOTween.Sequence();
        for (var i = 0; i <= 3; i++) //boosters are on 0-3 position
        {
            var booster = _rocket.transform.GetChild(i);
            _boosters.Add(booster);

            switch (i)
            {
                case 0:
                    sequence.Join(booster.DORotate(new Vector3(150, 0, 0), 2f));
                    break;
                case 1:
                    sequence.Join(booster.DORotate(new Vector3(0, 0, -150), 2f));
                    break;
                case 2:
                    sequence.Join(booster.DORotate(new Vector3(-150, 0, 0), 2f));
                    break;
                case 3:
                    sequence.Join(booster.DORotate(new Vector3(0, 0, 150), 2f));
                    break;
            }
        }
        sequence.OnComplete(EndDropBoosters);
    }

    private void EndDropBoosters()
    {
        for (var i = 0; i <= 3; i++)
        {
            var booster = _rocket.transform.GetChild(0);
            booster.parent = null;
        }
        _rocket.GetComponent<Rigidbody>().mass -= 2;
    }

    private void FlyToSpace()
    {
        IsRocketInSpace = true;
        SceneManager.LoadScene("SpaceScene");
        InitializeSpaceVariables();
    }

    private void InitializeSpaceVariables()
    {
        if (_spaceVariablesInitialized) return;
        _rocket.transform.position = new Vector3(-5604.4f, 8123.8f, -1635.0f);
        _rocket.GetComponent<Rigidbody>().useGravity = false;
    }
}