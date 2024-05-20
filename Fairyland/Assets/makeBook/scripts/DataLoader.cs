using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DataLoader
{
    private const string PATH = "/SaveFile/";
    private const string FILE_NAME = "ServerResponse.json";

    public static Dictionary<string, object> LoadJsonFromFile()
    {
        string path = Application.dataPath + PATH + FILE_NAME;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return data;
        }
        else
        {
            Debug.LogError("File not found: " + path);
            return null;
        }
    }
}
