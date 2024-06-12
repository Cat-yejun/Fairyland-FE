using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using System.Collections.Generic;

public class DisplayData : MonoBehaviour
{
    public TMP_Text displayText; // Assign this in the inspector
    public TMP_Text displayTitle; // Assign this in the inspector
    private const string PATH = "/SaveFile/";

    void Start()
    {
        Dictionary<string, object> data = null; // Initialize data to null

        
        string title = PlayerPrefs.GetString("newTitle", "default_title");
        
        displayTitle.text = title;

        string path = $"{Application.persistentDataPath}{PATH}{title}/{title}.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
        else
        {
            Debug.LogError("File not found: " + path);
            displayText.text = "File not found.";
            return;
        }

        if (data != null && data.TryGetValue("novel_num_dict", out var novelNumDictObj))
        {
            var novelNumDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(novelNumDictObj.ToString());
            if (novelNumDict != null)
            {
                List<string> sentences = new List<string>();
                foreach (var kvp in novelNumDict)
                {
                    sentences.Add(kvp.Value);
                }
                displayText.text = string.Join(" ", sentences);

            }
            else
            {
                displayText.text = "Failed to parse novel_num_dict.";
            }
        }
        else
        {
            displayText.text = "Data or novel_num_dict not found.";
        }
    }
}
