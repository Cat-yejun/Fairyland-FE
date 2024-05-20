using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DisplayData : MonoBehaviour
{
    public TMP_Text displayText; // Assign this in the inspector

    void Start()
    {
        Dictionary<string, object> data = DataLoader.LoadJsonFromFile();
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
