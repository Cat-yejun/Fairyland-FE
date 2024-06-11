using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GoToManuScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("new_Book_manu");
    }

}
