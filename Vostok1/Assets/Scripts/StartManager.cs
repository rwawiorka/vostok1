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
    [SerializeField] private GameObject rocket;

    public bool RocketCanStart
    {
        get => _rocketCanStart;
        private set => _rocketCanStart = value;
    }
    private bool _rocketCanStart;
    private bool _isRocketStringSetTo100 = true;

    private float timerStart;
    private float timerStop;

    private async void Update()
    {
        // First stage - Release the rocket holders

        if (Input.GetKeyDown(KeyCode.Space) && !RocketCanStart)
        {
            Debug.LogError("zmienic ciag");
            if (!_isRocketStringSetTo100)
            {
                return;
            }
            timerStart = Time.deltaTime;
            timerStop = timerStart + 10f;
            ReleaseTheHolders();
            RocketCanStart = true;
            await HoldRocketRotationAsync();
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
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.S))
                break;
            rocket.transform.rotation = Quaternion.identity;
            await Task.Delay(10);
        }
    }
}