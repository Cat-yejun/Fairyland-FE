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
    public TMP_InputField titleInputField;
    public GameObject loadingScreen;
    public GameObject textLookScreen;
    public GameObject imgSelectScreen;
    public GameObject mainScreen;
    public Button elementaryButton; // New button for elementary
    public Button toddlerButton; // New button for toddler
    public Button customButton; // New button for custom settings
    
    public TMP_InputField splitInputField; // New TMP_InputField for custom split
    public TMP_InputField lengthLimitInputField; // New TMP_InputField for custom length limit

    private string novel;
    private string title;

    private void Start()
    {
        // Add click listeners to the buttons
        elementaryButton.onClick.AddListener(OnElementaryButtonClick);
        toddlerButton.onClick.AddListener(OnToddlerButtonClick);
        customButton.onClick.AddListener(OnCustomButtonClick);
    }

    // Elementary button click event handler
    public void OnElementaryButtonClick()
    {
        int split = 36;
        int lengthLimit = 130;
        PlayerPrefs.SetInt("split", split);
        PlayerPrefs.SetInt("lengthLimit", lengthLimit);

        PlayerPrefs.Save();
        Debug.Log("split, lengthLmit saved: " + split + "-" + lengthLimit);
        //SendNovelToServer(36, 130);
    }

    // Toddler button click event handler
    public void OnToddlerButtonClick()
    {
        int split = 16;
        int lengthLimit = 70;
        PlayerPrefs.SetInt("split", split);
        PlayerPrefs.SetInt("lengthLimit", lengthLimit);

        PlayerPrefs.Save();
        Debug.Log("split, lengthLmit saved: " + split + "-" + lengthLimit);
        //SendNovelToServer(16, 70);
    }

    // Custom button click event handler
    public void OnCustomButtonClick()
    {
        int split = int.Parse(splitInputField.text);
        int lengthLimit = int.Parse(lengthLimitInputField.text);
        PlayerPrefs.SetInt("split", split);
        PlayerPrefs.SetInt("lengthLimit", lengthLimit);

        PlayerPrefs.Save();
        Debug.Log("split, lengthLmit saved: " + split + "-" + lengthLimit);
        //SendNovelToServer(split, lengthLimit);
    }

    public async void SendNovelToServer()
    {
        novel = userInputField.text;
        title = titleInputField.text;
        string url = "http://43.201.252.166:8000/make-novel";
        int split = PlayerPrefs.GetInt("split", 1);
        int lengthLimit = PlayerPrefs.GetInt("lengthLimit", 1);
        


        var json = new JObject
        {
            { "source", novel },
            { "split", split }, // Use split parameter
            { "length_limit", lengthLimit } // Use lengthLimit parameter
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

                    loadingScreen.SetActive(false);
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
                loadingScreen.SetActive(false);
            }
        }
    }

    private void SaveJsonToFile(string json, string title)
    {
        string path = Application.persistentDataPath + "/SaveFile/" + title + "/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        File.WriteAllText(path + title + ".json", json);
        Debug.Log("JSON response saved to: " + path + title + ".json");
    }


    public async void SendImageRequestsToServer()
    {
        string url = "http://43.201.252.166:8000/make-image";
        int initialSceneNum = 1;
        int split = PlayerPrefs.GetInt("split", 1);
        
   
        string historyPrompt = "";
        string agePrompt = "";
        Debug.Log("start to sending");
        Dictionary<string, string> characterDescriptionDict = new Dictionary<string, string>();
        //////
        {
            int currentSceneNum = initialSceneNum;
            var requestData = new
            {
                source = novel,
                split = split,
                scene = currentSceneNum,
                image_try = 3,
                history_prompt = historyPrompt,
                age_prompt = agePrompt,
                char_des_dict = new Dictionary<string, string>()
            };

            string jsonData = JsonConvert.SerializeObject(requestData);

            Debug.Log("Data sent to server: " + jsonData);
            loadingScreen.SetActive(true);

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
                        historyPrompt = responseData["history_prompt"]?.ToString();
                        agePrompt = responseData["age_prompt"]?.ToString();
                        string charDesDict = responseData["char_des_dict"]?.ToString();
                        characterDescriptionDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(charDesDict);
                        Debug.Log("딕셔너리 잘 만들어");

                        if (sceneLinks != null)
                        {
                            // Create directory if not exists
                            string imgFolderPath = Application.persistentDataPath + "/SaveFile/" + title + "/img/";
                            if (!Directory.Exists(imgFolderPath))
                            {
                                Directory.CreateDirectory(imgFolderPath);
                            }

                            // Download and save images
                            for (int j = 0; j < sceneLinks.Count; j++)
                            {
                                string imageUrl = sceneLinks[j].Value<string>();
                                StartCoroutine(DownloadAndSaveImage(imageUrl, imgFolderPath + currentSceneNum + "-" + j + ".png"));
                            }
                            loadingScreen.SetActive(false);
                            imgSelectScreen.SetActive(true);
                            textLookScreen.SetActive(false);

                        }
                        else
                        {
                            Debug.LogError("No scene links found in response.");
                        }

                        // 각 변수 값 출력 (디버그용)
                        Debug.Log("History Prompt: " + historyPrompt);
                        Debug.Log("Age Prompt: " + agePrompt);
                        Debug.Log("Character Description Dictionary: " + charDesDict);
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





     
            for (int i = 1; i < split - 1; i++)
            {
                int currentSceneNum = initialSceneNum + i;
                Debug.Log("두번째 보내는 historyPrompt: " +historyPrompt);
                var requestData = new
                {
                    source = novel,
                    split = split,
                    scene = currentSceneNum,
                    image_try = 3,
                    history_prompt = historyPrompt,
                    age_prompt = agePrompt,
                    char_des_dict = characterDescriptionDict
                };

                string jsonData = JsonConvert.SerializeObject(requestData);

                Debug.Log("Data sent to server: " + jsonData);
                loadingScreen.SetActive(true);

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
                            historyPrompt = responseData["history_prompt"]?.ToString();
                            agePrompt = responseData["age_prompt"]?.ToString();
                            string charDesDict = responseData["char_des_dict"]?.ToString();

                            if (sceneLinks != null)
                            {
                                // Create directory if not exists
                                string imgFolderPath = Application.persistentDataPath + "/SaveFile/" + title + "/img/";
                                if (!Directory.Exists(imgFolderPath))
                                {
                                    Directory.CreateDirectory(imgFolderPath);
                                }

                                // Download and save images
                                for (int j = 0; j < sceneLinks.Count; j++)
                                {
                                    string imageUrl = sceneLinks[j].Value<string>();
                                    StartCoroutine(DownloadAndSaveImage(imageUrl, imgFolderPath + currentSceneNum + "-" + j + ".png"));
                                }
                                loadingScreen.SetActive(false);
                                imgSelectScreen.SetActive(true);
                                textLookScreen.SetActive(false);

                            }
                            else
                            {
                                Debug.LogError("No scene links found in response.");
                            }

                            // 각 변수 값 출력 (디버그용)
                            Debug.Log("History Prompt: " + historyPrompt);
                            Debug.Log("Age Prompt: " + agePrompt);
                            Debug.Log("Character Description Dictionary: " + charDesDict);
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
