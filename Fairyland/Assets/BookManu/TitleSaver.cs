using UnityEngine;
using TMPro;

public class TitleSaver : MonoBehaviour
{
    public TMP_InputField titleInputField;

    public void SaveTitle()
    {
        string title = titleInputField.text;
        PlayerPrefs.SetString("title", title);
        PlayerPrefs.Save();
        Debug.Log("Title saved: " + title);
    }
}
