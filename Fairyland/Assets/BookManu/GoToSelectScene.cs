using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSelectScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("new_selectScene");
    }
}
