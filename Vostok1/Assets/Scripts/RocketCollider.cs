using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketCollider : MonoBehaviour
{
    private Transform[] children;

    private void Start()
    {
        children = transform.GetComponentsInChildren<Transform>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.Contains("Terrain"))
        {
            //TODO: Koniec gry!
            foreach (var child in children)
            {
                if (child.name == "Main Camera")
                    continue;

                if (child.GetComponent<Rigidbody>() == null)
                {
                    child.gameObject.AddComponent<Rigidbody>();

                }
            }
        }

        if (col.gameObject.name.Contains("Russia"))
        {
            SceneManager.LoadScene("LandingScene");
        }
    }
}
