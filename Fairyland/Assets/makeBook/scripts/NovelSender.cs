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
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class NovelSender : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TMP_InputField titleInputField; // New TMP_InputField for title
    public GameObject loadingScreen; // Drag your loading panel here
    public GameObject textLookScreen;
    public GameObject imgSelectScreen;
    public GameObject mainScreen;

    public async void SendNovelToServer()
    {
        string novel = userInputField.text;
        string title = titleInputField.text;
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
                mainScreen.SetActive(false);
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

                    SaveJsonToFile(jsonResponse, title);

                    // Deactivate loading screen and load next scene
                    loadingScreen.SetActive(false);
                    //SceneManager.LoadScene("textlookScene"); // Replace with your scene name
                    textLookScreen.SetActive(true);

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

    private void SaveJsonToFile(string json, string title)
    {
        string path = Application.dataPath + "/SaveFile/" + title + "/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        File.WriteAllText(path + title + ".json", json);
        Debug.Log("JSON response saved to: " + path + title + ".json");
    }

    public async void SendImageRequestToServer()
    {
        string novel = userInputField.text;
        string title = titleInputField.text;
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

                    var responseData = JsonConvert.DeserializeObject<JObject>(jsonResponse);

                    JArray sceneLinks = responseData["scene_link"] as JArray;
                    if (sceneLinks != null)
                    {
                        // Create directory if not exists
                        string imgFolderPath = Application.dataPath + "/SaveFile/" + title + "/img/";
                        if (!Directory.Exists(imgFolderPath))
                        {
                            Directory.CreateDirectory(imgFolderPath);
                        }

                        // Download and save images
                        for (int i = 0; i < sceneLinks.Count; i++)
                        {
                            string imageUrl = sceneLinks[i].Value<string>();
                            StartCoroutine(DownloadAndSaveImage(imageUrl, imgFolderPath + "image_" + i + ".png"));
                        }
                        imgSelectScreen.SetActive(true);
                        textLookScreen.SetActive(false);
                    }
                    else
                    {
                        Debug.LogError("No scene links found in response.");
                    }
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

    private IEnumerator DownloadAndSaveImage(string imageUrl, string imagePath)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(imagePath, bytes);
                Debug.Log("Image saved to: " + imagePath);
            }
            else
            {
                Debug.LogError("Failed to download image: " + webRequest.error);
            }
        }
    }
}
