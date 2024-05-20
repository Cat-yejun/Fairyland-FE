using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO; // 파일 작업을 위한 네임스페이스

public class NovelSender : MonoBehaviour
{
    public TMP_InputField userInputField;
    private const string PATH = "/SaveFile/"; // 저장할 폴더 경로
    private const string FILE_NAME = "ServerResponse.json"; // 저장할 파일 이름

    public async void SendNovelToServer()
    {
        string novel = userInputField.text;
        string url = "http://43.201.252.166:8000/make-novel";

        // Create JSON object
        var json = new JObject
        {
            { "source", novel },
            { "split", 16 },
            { "length_limit", 70 }
        };

        string jsonData = json.ToString();

        Debug.Log("Data sent to server: " + jsonData); // Logging the data sent to the server

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(100); // Set a reasonable timeout
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response from server: " + jsonResponse);

                    // Deserialize JSON response to dictionary
                    var dictionaryResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                    Debug.Log("Dictionary response: " + dictionaryResponse);

                    // Example of how to use the dictionary
                    foreach (var kvp in dictionaryResponse)
                    {
                        Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
                    }

                    // Save the JSON response to a file
                    SaveJsonToFile(jsonResponse);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Request error: " + response.StatusCode + " - " + errorResponse);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("Request error: " + e.Message);
            }
            catch (TaskCanceledException e)
            {
                Debug.LogError("Request timeout: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected error: " + e.Message);
            }
        }
    }

    private void SaveJsonToFile(string json)
    {
        string path = Application.dataPath + PATH;

        if (!Directory.Exists(path)) // 해당 경로가 존재하지 않는다면
        {
            Directory.CreateDirectory(path); // 경로(폴더) 생성
        }

        // 파일 생성 및 저장
        File.WriteAllText(path + FILE_NAME, json);
        Debug.Log("JSON response saved to: " + path + FILE_NAME);
    }
}
