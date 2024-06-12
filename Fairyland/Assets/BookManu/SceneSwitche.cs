using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitche : MonoBehaviour
{
    public void SceneChange()
    {
        int previousValue = PlayerPrefs.GetInt("isNew", 0);
        if (previousValue == 1)
        {
            SceneManager.LoadScene("new_selectScene");
        }
        else
        {
            SceneManager.LoadScene("txtInputScene");
        }
    }
}
