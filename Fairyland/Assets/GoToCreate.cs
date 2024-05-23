using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToCreate : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("mktextScene");
    }
}
