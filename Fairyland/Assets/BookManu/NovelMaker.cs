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


public class NovelMaker : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TMP_InputField titleInputField;
    public GameObject elementaryObject; // New object for elementary
    public GameObject toddlerObject; // New object for toddler
    public GameObject customObject; // New object for custom settings
    private const string PATH = "/SaveFile/";



    public TMP_InputField splitInputField; // New TMP_InputField for custom split
    public TMP_InputField lengthLimitInputField; // New TMP_InputField for custom length limit

    private string novel;
    private string title;

    

    // Singleton instance
    private static NovelMaker instance;
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Check if the instance already exists
        if (instance == null)
        {
            // If not, set the instance to this object
            instance = this;

            // Make sure this GameObject persists between scene changes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this object
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        // Initially activate toddlerObject and deactivate others
        toddlerObject.SetActive(true);
        elementaryObject.SetActive(false);
        customObject.SetActive(false);
        
    }

    public void Next()
    {
        if (toddlerObject.activeSelf)
        {
            toddlerObject.SetActive(false);
            elementaryObject.SetActive(true);
        }
        else if (elementaryObject.activeSelf)
        {
            elementaryObject.SetActive(false);
            customObject.SetActive(true);
        }
    }

    public void Previous()
    {
        if (customObject.activeSelf)
        {
            customObject.SetActive(false);
            elementaryObject.SetActive(true);
        }
        else if (elementaryObject.activeSelf)
        {
            elementaryObject.SetActive(false);
            toddlerObject.SetActive(true);
        }
    }


    // Function to save PlayerPrefs based on the active object
    public void SavePlayerPrefsForActiveObject()
    {
        novel = userInputField.text;
        title = titleInputField.text;
        if (elementaryObject.activeSelf)
        {
            int split = 36;
            int lengthLimit = 130;
            PlayerPrefs.SetInt("split", split);
            PlayerPrefs.SetInt("lengthLimit", lengthLimit);
            PlayerPrefs.Save();
            Debug.Log("split, lengthLimit saved: " + split + "-" + lengthLimit);
            
        }
        else if (toddlerObject.activeSelf)
        {
            int split = 16;
            int lengthLimit = 70;
            PlayerPrefs.SetInt("split", split);
            PlayerPrefs.SetInt("lengthLimit", lengthLimit);
            PlayerPrefs.Save();
            Debug.Log("split, lengthLimit saved: " + split + "-" + lengthLimit);
            
        }
        else if (customObject.activeSelf)
        {
            int split = int.Parse(splitInputField.text);
            int lengthLimit = int.Parse(lengthLimitInputField.text);
            PlayerPrefs.SetInt("split", split);
            PlayerPrefs.SetInt("lengthLimit", lengthLimit);
            PlayerPrefs.Save();
            Debug.Log("split, lengthLimit saved: " + split + "-" + lengthLimit);
            
        }
        
        Debug.Log("Data sent to server: " + title + "\n" + novel);



        SendNovelToServer();
       
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
                    // 5초 동안 딜레이
                    await Task.Delay(5000);
                    Debug.Log("5초 딜레이 되는 ");
                    
                    SendImageRequestsToServer(); //  바로 이미지 만드는 요청





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
                        Debug.Log("딕셔너리 잘 만들어");

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
            Debug.Log("두번째 보내는 historyPrompt: " + historyPrompt);
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
        Debug.Log("이제 이미지 다 만들고 표지 만드는중 " );
        SendCoverRequestToServer(historyPrompt, agePrompt, characterDescriptionDict);
        

    }

    public async void SendCoverRequestToServer(string historyPrompt,string agePrompt, Dictionary<string, string> characterDescriptionDict)
    {
        string url = "http://43.201.252.166:8000/make-cover";

        // 요청 데이터 설정
        var requestData = new
        {
            source = novel,
            image_try = 1,
            history_prompt = historyPrompt,
            age_prompt = agePrompt,
            char_des_dict = characterDescriptionDict
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

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
                        // scene_links 배열 처리
                        foreach (var sceneLink in sceneLinks)
                        {
                            string sceneUrl = sceneLink.Value<string>();
                            Debug.Log("Scene Link: " + sceneUrl);
                            // 여기서 sceneUrl을 사용하여 필요한 작업 수행
                            string coverFolderPath = Application.persistentDataPath + "/SaveFile/temp/";

                            // temp 폴더가 존재하지 않으면 생성
                            if (!System.IO.Directory.Exists(coverFolderPath))
                            {
                                System.IO.Directory.CreateDirectory(coverFolderPath);
                            }

                            StartCoroutine(DownloadAndSaveImage(sceneUrl, coverFolderPath + title + ".png"));

                        }
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
        Debug.Log("표지 만들기 끝 ");
        SendInteractionRequest(); // 이미지 끝나면 바로 인터렉션 만드는 요청
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

    private async void SendInteractionRequest()
    {
        string jsonPath = $"{Application.persistentDataPath}{PATH}{title}/{title}.json";
        Debug.Log("인터렉션 데이터를 보냅니다..");

        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);

            using (HttpClient client = new HttpClient())
            {
                string url = "http://43.201.252.166:8000/make-interaction";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Log("보내는 중...");
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        Debug.Log("서버로부터의 응답: " + jsonResponse);

                        // 받아온 JSON 데이터를 파일로 저장
                        SaveJsonToSpecificPath(jsonResponse, $"{Application.persistentDataPath}{PATH}{title}", "interaction.json");
                        PlayerPrefs.SetInt("isNew", 1);
                        PlayerPrefs.Save();
                        Debug.Log("new Title saved: " + title);
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Debug.LogError("요청 오류: " + response.StatusCode + " - " + errorResponse);
                    }
                }
                catch (HttpRequestException e)
                {
                    Debug.LogError("요청 오류: " + e.Message);
                }
                catch (Exception e)
                {
                    Debug.LogError("예상치 못한 오류: " + e.Message);
                }
            }
        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + jsonPath);
        }
    }

    private void SaveJsonToSpecificPath(string jsonResponse, string directoryPath, string fileName)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(filePath, jsonResponse);
        Debug.Log("JSON 파일이 저장되었습니다: " + filePath);
    }


}
