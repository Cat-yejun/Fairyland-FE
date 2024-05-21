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
using System.IO;
using UnityEngine.SceneManagement;

public class NovelSender : MonoBehaviour
{
    public TMP_InputField userInputField;
    public GameObject loadingScreen; // Drag your loading panel here
    private const string PATH = "/SaveFile/";
    private const string FILE_NAME = "ServerResponse.json";

    public async void SendNovelToServer()
    {
        string novel = userInputField.text;
        string url = "http://43.201.252.166:8000/make-novel";

        var json = new JObject
        {
            { "source", novel },
            { "split", 16 },
            { "length_limit", 70 }
        };

        string jsonData = json.ToString();
        Debug.Log("Data sent to server: " + jsonData);

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(1000);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                // Activate loading screen
                loadingScreen.SetActive(true);

                HttpResponseMessage response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response from server: " + jsonResponse);

                    var dictionaryResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                    Debug.Log("Dictionary response: " + dictionaryResponse);

                    foreach (var kvp in dictionaryResponse)
                    {
                        Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
                    }

                    SaveJsonToFile(jsonResponse);

                    // Deactivate loading screen and load next scene
                    loadingScreen.SetActive(false);
                    SceneManager.LoadScene("textlookScene"); // Replace with your scene name
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
            finally
            {
                // Deactivate loading screen in case of error or completion
                loadingScreen.SetActive(false);
            }
        }
    }

    private void SaveJsonToFile(string json)
    {
        string path = Application.dataPath + PATH;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        File.WriteAllText(path + FILE_NAME, json);
        Debug.Log("JSON response saved to: " + path + FILE_NAME);
    }

    public async void SendImageRequestToServer()
    {
        string novel = userInputField.text;
        string url = "http://43.201.252.166:8000/make-image";

        var requestData = new
        {
            source = novel,
            split = 16,
            scene = 1,
            image_try = 3,
            history_prompt = "",
            age_prompt = "",
            char_des_dict = new Dictionary<string, string>()
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        Debug.Log("Data sent to server: " + jsonData);

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(1000);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response from server: " + jsonResponse);
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

}
