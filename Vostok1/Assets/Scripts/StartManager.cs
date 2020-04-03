using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _rocketHolders;
    [SerializeField] private GameObject _rocket;
    [SerializeField] private RocketControl _rocketControl;
    [SerializeField] private ParticleSystem[] _flames;

    public bool RocketCanStart
    {
        get => _rocketCanStart;
        private set => _rocketCanStart = value;
    }
    private bool _rocketCanStart;
    private bool _isRocketStringSetTo100 = true;

    private float timerStart;
    private float timerStop;

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
            timerStart = Time.deltaTime;
            timerStop = timerStart + 10f;
            ReleaseTheHolders();
            foreach (var flame in _flames)
            {
                flame.GetComponent<Transform>().DOScale(Vector3.one, 2f);
            }
            RocketCanStart = true;
            await HoldRocketRotationAsync();
        }

        if (RocketCanStart)
        {
            ToogleFlames(_rocketControl.IsRocketOn);
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

    private void ToogleFlames(bool isRocketOn)
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
}