using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketDontDestroy : MonoBehaviour
{
    [SerializeField] private GameObject rocket;
    [SerializeField] private GameObject sceneManager;
    [SerializeField] private GameObject hud;

    private void Start()
    {
        DontDestroyOnLoad(rocket);
        DontDestroyOnLoad(sceneManager);
        DontDestroyOnLoad(hud);
    }
}
