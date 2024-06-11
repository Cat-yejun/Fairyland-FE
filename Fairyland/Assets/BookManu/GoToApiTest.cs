using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GoToApiTest : MonoBehaviour
{

    public void SceneChange()
    {
        SceneManager.LoadScene("APITestScene");
    }


}
