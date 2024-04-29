using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraController : MonoBehaviour
{
    public Camera gameplayCamera;
    public Camera skillTreeCamera;

    private void Start()
    {
        gameplayCamera.enabled = false;
        skillTreeCamera.enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            gameplayCamera.enabled = !gameplayCamera.enabled;
            skillTreeCamera.enabled = !skillTreeCamera.enabled;
        }
    }
}
