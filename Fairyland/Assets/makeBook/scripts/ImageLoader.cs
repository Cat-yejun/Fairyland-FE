using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using UnityEngine.SceneManagement;

public class ImageLoader : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button leftButton;
    public Button rightButton;
    public Button selectButton; // 추가된 버튼
    public Button goToCreateInteractionButton; // Added button
    public RawImage rawImage;
    public TMP_Text tmpText;
    public GameObject loadingScreen;
    private const string PATH = "/SaveFile/";
    private int currentScene = 1; // 현재 씬 번호
    private Dictionary<string, string> novelNumDict = new Dictionary<string, string>();
    private Dictionary<string, string> splitedNovelDict = new Dictionary<string, string>();
    private string[] selectedImages; // 씬마다의 이미지를 저장하는 배열
    private int current_img_num;


    void Start()
    {
        button1.onClick.AddListener(() => LoadImage(1));
        button2.onClick.AddListener(() => LoadImage(2));
        button3.onClick.AddListener(() => LoadImage(3));

        leftButton.onClick.AddListener(() =>
        {
            ChangeScene(-1);
            button1.onClick.Invoke(); // button1 클릭 이벤트 트리거
        });

        rightButton.onClick.AddListener(() =>
        {
            ChangeScene(1);
            button1.onClick.Invoke(); // button1 클릭 이벤트 트리거
        });
        
        selectButton.onClick.AddListener(SelectImage); // '선택하기' 버튼 클릭 이벤트 추가

        // JSON 파일 읽기 및 딕셔너리 변환
        LoadJson();

        // 씬이 변경될 때마다 텍스트 업데이트
        ChangeScene(0);

        // selectedImages 배열 초기화
        int maxScene = PlayerPrefs.GetInt("split", 1) - 1;
        maxScene = 3; // TEST!!!!!!!
        selectedImages = new string[maxScene + 1]; // 씬 번호는 1부터 시작하므로 +1
        goToCreateInteractionButton.gameObject.SetActive(false);
        goToCreateInteractionButton.onClick.AddListener(SendInteractionRequest);
    }


    private void LoadImage(int buttonNumber)
    {
        string title = PlayerPrefs.GetString("title", "default_title");
   
        title = "펭펭펭펭날아가"; //TEST!!!!!!!
        current_img_num = buttonNumber - 1;

        string path = $"{Application.persistentDataPath}{PATH}{title}/img/{currentScene}-{buttonNumber - 1}.png";

        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            rawImage.texture = texture;
            rawImage.SetNativeSize();
        }
        else
        {
            Debug.LogError("Image file not found: " + path);
        }
    }

    private void ChangeScene(int change)
    {
       
        int maxScene = PlayerPrefs.GetInt("split", 1) - 1;
        maxScene = 3; //TEST!!!!!!!
        currentScene += change;
        if (currentScene < 1)
        {
            currentScene = 1;
        }
        else if (currentScene > maxScene)
        {
            currentScene = maxScene;
        }
        //Debug.Log("현재 title:" + PlayerPrefs.GetString("title", "default_title"));

        // 현재 씬에 해당하는 텍스트를 TMP_Text에 삽입
        if (splitedNovelDict.TryGetValue(currentScene.ToString(), out string sceneText))
        {
            tmpText.text = sceneText;
            
        }
        else
        {
            tmpText.text = "Scene text not found.";
        }
    }

    private void LoadJson()
    {
        string title = PlayerPrefs.GetString("title", "default_title");
        title = "펭펭펭펭날아가"; //TEST!!!!!!!
        string jsonPath = $"{Application.persistentDataPath}{PATH}{title}/{title}.json";

        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

            if (jsonData != null)
            {
                novelNumDict = jsonData.ContainsKey("novel_num_dict") ? jsonData["novel_num_dict"] : new Dictionary<string, string>();
                splitedNovelDict = jsonData.ContainsKey("splited_novel_dict") ? jsonData["splited_novel_dict"] : new Dictionary<string, string>();

                Debug.Log("JSON loaded and deserialized.");
            }
            else
            {
                Debug.LogError("Failed to deserialize JSON.");
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }
    }

    private void SelectImage()
    {
        
        string imageName = $"{currentScene}-{current_img_num}.png";

        if (selectedImages[currentScene] == imageName)
        {
            Debug.Log($"Image {imageName} is already selected for scene {currentScene}.");
        }
        else
        {
            selectedImages[currentScene] = imageName;
            Debug.Log($"Image {imageName} selected for scene {currentScene}.");
            // Check if the array is full
            bool arrayIsFull = true;
            for (int i = 1; i < selectedImages.Length; i++)
            {
                if (string.IsNullOrEmpty(selectedImages[i]))
                {
                    arrayIsFull = false;
                    break;
                }
            }

            // Activate "Go to Create Interaction" button if the array is full
            if (arrayIsFull)
            {
                goToCreateInteractionButton.gameObject.SetActive(true);
            }
        }
    }
    private async void SendInteractionRequest()
    {
        
        string title = PlayerPrefs.GetString("title", "default_title");
        title = "펭펭펭펭날아가"; //TEST!!!!!!!
        string jsonPath = $"{Application.persistentDataPath}{PATH}{title}/{title}.json";
        Debug.Log("데이터를 보냅니..");
        loadingScreen.SetActive(true);

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
                        Debug.Log("보내는...");
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        Debug.Log("Response from server: " + jsonResponse);

                        // 받아온 JSON 데이터를 파일로 저장
                        SaveJsonToFile(jsonResponse, "interaction.json");
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
                catch (Exception e)
                {
                    Debug.LogError("Unexpected error: " + e.Message);
                }
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }

        DeleteNonSelectedImages();
        SceneManager.LoadScene("Book_manu_replace");
    }

   

    private void SaveJsonToFile(string json, string fileName)
    {
        
        string title = PlayerPrefs.GetString("title", "default_title");
        title = "펭펭펭펭날아가";
        string path = $"{Application.persistentDataPath}{PATH}{title}/{fileName}";

        try
        {
            File.WriteAllText(path, json);
            Debug.Log("JSON response saved to: " + path);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save JSON file: " + e.Message);
        }
    }

    private void DeleteNonSelectedImages()
    {
        string title = PlayerPrefs.GetString("title", "default_title");
        title = "펭펭펭펭날아가";
        string imgDirectoryPath = $"{Application.persistentDataPath}{PATH}{title}/img/";

        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(imgDirectoryPath);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                string fileName = file.Name;
                string sceneNumber = fileName.Split('-')[0];
                int sceneIndex = int.Parse(sceneNumber);

                if (selectedImages[sceneIndex] == null || selectedImages[sceneIndex] != fileName)
                {
                    // Delete the image file
                    File.Delete(file.FullName);
                    Debug.Log($"Deleted image: {file.Name}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete non-selected images: " + e.Message);
        }
    }
}
