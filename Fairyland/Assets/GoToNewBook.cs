using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GoToNewBook : MonoBehaviour
{
    public TMP_Text titleText;

    public void SceneChange()
    {
        SceneManager.LoadScene("3D_book");
    }
    public void SaveTitle()
    {
        string title = titleText.text;
        PlayerPrefs.SetString("title", title);
        PlayerPrefs.Save();
        Debug.Log("Title saved: " + title);
    }
}
