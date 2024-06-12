using UnityEngine;
using TMPro;

public class TitleSaver : MonoBehaviour
{
    public TMP_InputField titleInputField;

    public void SaveTitle()
    {
        string title = titleInputField.text;
        PlayerPrefs.SetString("newTitle", title);
        PlayerPrefs.Save();
        Debug.Log("new Title saved: " + title);
    }
}
