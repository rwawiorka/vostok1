using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketDontDestroy : MonoBehaviour
{
    [SerializeField] private GameObject rocket;

    private void Start()
    {
        DontDestroyOnLoad(rocket);
    }
}
