using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillTreeManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SceneManager.LoadScene("Gameplay");
        }
    }
}
