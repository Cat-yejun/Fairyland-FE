using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using System.Collections.Generic;

public class DisplayData : MonoBehaviour
{
    public TMP_Text displayText; // Assign this in the inspector
    private const string PATH = "/SaveFile/";
    public TMP_InputField titleInputField;

    void Start()
    {
        Dictionary<string, object> data = null; // Initialize data to null

        string title = titleInputField.text;
        string path = Application.persistentDataPath + PATH + title + "/" + title + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }

        if (data != null)
        {
            foreach (var kvp in data)
            {
                displayText.text += $"Key: {kvp.Key}, Value: {kvp.Value}\n";
            }
        }
        else
        {
            displayText.text = "Failed to load data.";
        }
    }
}
