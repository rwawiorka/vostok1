using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class BackgroundDimmer : MonoBehaviour
{
    [SerializeField] private StartManager _startManager;

    public float TimeRocketOn { get; private set; }

    private void Start()
    {
        RenderSettings.skybox.SetFloat("_Exposure", 1);
        TimeRocketOn = 1f;
    }

    public void FadeDown()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        while (!_startManager.SpaceStage && !_startManager.LandingStage)
        {
            TimeRocketOn -= Time.deltaTime / 8;
            RenderSettings.skybox.SetFloat("_Exposure", TimeRocketOn);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
