using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainDirector : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Book_manu");
        }

    }
}
