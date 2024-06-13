using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using System.Collections.Generic;

public class DisplayData : MonoBehaviour
{
    public TMP_InputField displayText; // Assign this in the inspector
    public TMP_Text displayTitle; // Assign this in the inspector
    private const string PATH = "/SaveFile/";

    private string title;
    private string path;

    void Start()
    {
        Dictionary<string, object> data = null; // Initialize data to null
        Debug.Log("디스플레이 데이터 돌아가는 중?");

        title = PlayerPrefs.GetString("newTitle", "default_title");
        Debug.Log(title);
        displayTitle.text = title;

        path = $"{Application.persistentDataPath}{PATH}{title}/{title}.json";

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

    public void SaveText()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            if (data != null && data.TryGetValue("novel_num_dict", out var novelNumDictObj))
            {
                var novelNumDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(novelNumDictObj.ToString());
                if (novelNumDict != null)
                {
                    var sentences = displayText.text.Split(' ');
                    int index = 0;
                    foreach (var key in novelNumDict.Keys)
                    {
                        if (index < sentences.Length)
                        {
                            novelNumDict[key] = sentences[index];
                            index++;
                        }
                    }
                    data["novel_num_dict"] = novelNumDict;
                    string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                    File.WriteAllText(path, updatedJson);
                    Debug.Log("Text saved successfully.");
                }
                else
                {
                    Debug.LogError("Failed to parse novel_num_dict for saving.");
                }
            }
            else
            {
                Debug.LogError("Data or novel_num_dict not found for saving.");
            }
        }
        else
        {
            Debug.LogError("File not found for saving: " + path);
        }
    }
}
