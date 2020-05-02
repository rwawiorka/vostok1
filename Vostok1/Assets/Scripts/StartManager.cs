using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    [SerializeField] private GameObject _capsuleBoosterFlame;
    [SerializeField] private GameObject[] _capsuleAnthennas;
    [SerializeField] private DistanceMeasure _distanceMeasure;
    [SerializeField] private BackgroundDimmer _background;
    [SerializeField] private CloudsCreator _cloudsCreator;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _capsuleBooster;

    public const decimal DISTANCETOSPACE = 6.5M;
    public const decimal DISTANCETOCREATECLOUDS = 4M;
    public const decimal DISTANCETOINCREASEEMISSION = 5.5M;

    public bool StartStage { get; set; }

    public bool SpaceStage { get; set; }

    public bool LandingStage { get; set; }

    public bool RocketCanStart { get; private set; }

    public float RocketForce { get; private set; }

    public float BoostersFuel { get; private set; }

    public float RocketFuel { get; private set; }

    public float CapsuleBoosterFuel { get; private set; }

    public float ControlForce { get; private set; }

    private bool _droppedBoosters;
    private bool _droppedCapsuleCover;
    private bool _droppedMainEngine;
    private bool _droppedCapsuleBooster;
    private bool _anthennasExtended;
    private bool _anthennasHidden;

    private bool _spaceVariablesInitialized;
    private bool _landingVariablesInitialized;

    private List<Transform> _boosters;
    private List<Transform> _capsuleCovers;
    

    private void Start()
    {
        _boosters = new List<Transform>();
        _capsuleCovers = new List<Transform>();

        foreach (var flame in _flames)
        {
            flame.GetComponent<Transform>().localScale = new Vector3(.2f, .2f, .2f);
        }

        RocketForce = 4500;
        RocketFuel = 2500;
        BoostersFuel = 1800;
        CapsuleBoosterFuel = 1000;
        ControlForce = 300;
        SpaceStage = false;
        LandingStage = false;
        StartStage = true;
    }

    private async void Update()
    {
        // First stage - Release the rocket holders
        if (StartStage)
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
                DropBoosters();
                await Task.Delay(2500);
                FlyToSpace();
            }

            

            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (!_droppedBoosters)
                {
                    DropBoosters();
                    await Task.Delay(2500);
                } 
                DropMainEngine();
                await Task.Delay(500);
                DropCapsuleCover();
                await Task.Delay(500);
                DropCapsuleBooster();
                await Task.Delay(500);
                BackToEarth();
            }

            if (!_droppedBoosters && (Input.GetKeyDown(KeyCode.Space) || _distanceMeasure.Distance >= DISTANCETOCREATECLOUDS || BoostersFuel <= 0))
            {
                DropBoosters();
            }

            if (_distanceMeasure.Distance >= DISTANCETOSPACE)
            {
                if (SceneManager.GetActiveScene().name == "SpaceScene") return;
                FlyToSpace();

            }
            
            if (_distanceMeasure.Distance >= DISTANCETOCREATECLOUDS)
            {
                if(!_cloudsCreator.IsCloudToggled)
                    _cloudsCreator.ToggleClouds(true);
            }
        }

        else if(SpaceStage)
        {
            _cloudsCreator.ToggleClouds(false);

            if (!_droppedCapsuleCover && (RocketFuel <= 700) || Input.GetKeyDown(KeyCode.Space))
            {
                DropCapsuleCover();
            }

            if (!_droppedMainEngine && (RocketFuel <= 0 || Input.GetKeyDown(KeyCode.Space)))
            {
                DropMainEngine();
                _capsuleBoosterFlame.SetActive(true);
                RocketForce /= 5;
                ControlForce -= 250;
            }

            if (!_droppedCapsuleBooster && (CapsuleBoosterFuel <= 0) || Input.GetKeyDown(KeyCode.Space))
            {
                DropCapsuleBooster();
                ControlForce -= 40;
                await Task.Delay(5000);
                ExtendAnthennas(); 
            }
        }

        else if(LandingStage)
        {
            if (!_cloudsCreator.IsCloudToggled)
            {
                _cloudsCreator.DecreaseCloudEmission();
            }
        }

        if (_rocketControl.IsRocketOn)
        {
            
            if (BoostersFuel > 0 && !_droppedBoosters) // stage 1;
            {
                BoostersFuel -= Time.deltaTime * 80;
            }
            else if (RocketFuel > 0 && !_droppedMainEngine) // stage 2;
            {
                RocketFuel -= Time.deltaTime * 50;
            }
            else if (CapsuleBoosterFuel > 0 && !_droppedCapsuleBooster) // stage 3;
            {
                CapsuleBoosterFuel -= Time.deltaTime * 20;
            }
        }
        ToggleFlames(_rocketControl.IsRocketOn);
        if (Input.GetKeyDown(KeyCode.F5))
        {
            FixCapsuleCoordinates();
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

        if (_droppedMainEngine) return;

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
        BoostersFuel = 0;
        RocketForce -= 1125;
        _droppedBoosters = true;
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

    private async void EndDropBoosters()
    {
        _rocket.GetComponent<Rigidbody>().mass -= 2;
        
        for (var i = 0; i <= 3; i++)
        {
            var booster = _rocket.transform.GetChild(0);
            booster.parent = null;
        }

        await Task.Delay(2000);

        foreach (var booster in _boosters)
        {
            DestroyImmediate(booster.gameObject);
        }
    }

    private void DropCapsuleCover()
    {
        if (_droppedCapsuleCover) return;
        _droppedCapsuleCover = true;
        var sequence = DOTween.Sequence();
        for (var i = 0; i < 2; i++) //covers are on 0-2 position now
        {
            var cover = _rocket.transform.GetChild(i);
            _capsuleCovers.Add(cover);
            switch (i)
            {
                case 0:
                    sequence.Join(cover.DORotate(new Vector3(179, 0, 0), 3f));
                    break;
                case 1:
                    sequence.Join(cover.DORotate(new Vector3(-179, 0, 0), 3f));
                    break;
            }
        }

        sequence.OnComplete(EndDropCapsuleCover);
    }

    private async void EndDropCapsuleCover()
    {
        await Task.Delay(2000);

        for (var i = 0; i < 2; i++)
        {
            var cover = _rocket.transform.GetChild(0);
            cover.parent = null;
        }

        foreach (var capsuleCover in _capsuleCovers)
        {
            DestroyImmediate(capsuleCover.gameObject);
        }
    }

    private async void DropMainEngine()
    {
        if (_droppedMainEngine) return;
        _droppedMainEngine = true;
        var mainEngine = _rocket.transform.GetChild(0);

        _rocket.transform.GetChild(0).parent = null;
        _rocket.GetComponent<Rigidbody>().mass -= 2;
        // await Task.Delay(500);
        // FixCapsuleCoordinates();
        await Task.Delay(2000);
        DestroyImmediate(mainEngine.gameObject);
    }

    private void DropCapsuleBooster()
    {
        if (_droppedCapsuleBooster) return;
        _droppedCapsuleBooster = true;
        _capsuleBooster.transform.parent = null;
        _rocket.GetComponent<Rigidbody>().mass -= 2;
        RocketForce = 0;
    }

    private void ExtendAnthennas()
    {
        if (_anthennasExtended) return;
        _anthennasExtended = true;
        foreach (var anthenna in _capsuleAnthennas)
        {
            anthenna.transform.DOScale(new Vector3(1, 1, 1), 5f);
        }
    }

    private void HideAnthennas()
    {
        if (_anthennasHidden) return;
        _anthennasHidden = true;
        foreach (var anthenna in _capsuleAnthennas)
        {
            anthenna.transform.DOScale(Vector3.zero, 0f);
        }
    }

    private void FlyToSpace()
    {
        StartStage = false;
        LandingStage = false;
        SpaceStage = true;
        SceneManager.LoadSceneAsync("SpaceScene");
        InitializeSpaceVariables();
        _cloudsCreator.ToggleClouds(false);
    }

    private void InitializeSpaceVariables()
    {
        if (_spaceVariablesInitialized) return;
        _spaceVariablesInitialized = true;
        _rocket.transform.position = new Vector3(-255.8f, 784.07f, -576.16f);
        RocketForce = 50;
        var rocketFlame = GameObject.Find("Flames (3)").GetComponent<ParticleSystem>();
        var rocketFlameMain = rocketFlame.main;
        rocketFlameMain.simulationSpace = ParticleSystemSimulationSpace.Local;
        _rocket.GetComponent<Rigidbody>().velocity = _rocket.GetComponent<Rigidbody>().velocity / 100;
        _rocket.GetComponent<Rigidbody>().useGravity = false;
        _rocket.GetComponent<Rigidbody>().rotation = Quaternion.identity;
    }

    private void InitializeLandingVariables()
    {
        if (_landingVariablesInitialized) return;
        _landingVariablesInitialized = true;
        _rocket.transform.position = new Vector3(0, 1000, 0);
        RenderSettings.skybox.SetFloat("_Exposure", 1);

    }

    private void BackToEarth()
    {
        StartStage = false;
        SpaceStage = false;
        LandingStage = true;
        SceneManager.LoadScene("LandingScene");
        InitializeLandingVariables();
        HideAnthennas();
    }

    private void FixCapsuleCoordinates()
    {
        Debug.Log(_rocket.transform.GetChild(0).gameObject.name);
        _rocket.transform.GetChild(0).position = Vector3.zero;
    }
}