using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _rocketHolders;

    public bool RocketCanStart
    {
        get => _rocketCanStart;
        private set => _rocketCanStart = value;
    }
    private bool _rocketCanStart;
    private bool _rocketStarted;
    private bool _stopAnimation;
    private bool _isRocketStringSetTo100 = true;
    private int _holdersIterator;

    private void Update()
    {
        // First stage - Release the rocket holders
        if (Input.GetKeyDown(KeyCode.Space) && !RocketCanStart)
        {
            Debug.LogError("zmienic ciag");
            if (!_isRocketStringSetTo100)
            {
                //TODO: komunikat o ciągu 100%
                
                return;
            }
            ReleaseTheHolders();
            RocketCanStart = true;
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
}