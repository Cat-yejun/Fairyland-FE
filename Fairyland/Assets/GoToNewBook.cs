using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class GoToNewBook : MonoBehaviour
{
    public TMP_Text titleText;

    void Start()
    {
        string BGMPath = Path.Combine(Application.persistentDataPath, "BGM.mp3");
        StartCoroutine(BackGroundMusic.Instance.LoadBackgroundMusic(BGMPath));
    }

    public void SceneChange()
    {
        BackGroundMusic.Instance.StopMusic();
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
