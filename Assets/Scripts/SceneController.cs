using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        SceneManager.LoadScene("Gameplay");
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        SceneManager.LoadScene("SkillTree");
    //    }
    //}
    public void OnSkillTreeButtonPressed()
    {
        GameManager.instance.ShowSkillTree();
    }

    public void OnReturnToGameButtonPressed()
    {
        GameManager.instance.ShowGameplay();
    }
}
