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

public class ImgCanvas : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button leftButton;
    public Button rightButton;
    public Button selectButton; // 추가된 버튼
    public Button goToCreateInteractionButton; // Added button
    public GameObject LALO;
    public RawImage rawImage;
    public TMP_Text tmpText;
    private const string PATH = "/SaveFile/";
    private int currentScene = 1; // 현재 씬 번호
    private Dictionary<string, string> novelNumDict = new Dictionary<string, string>();
    private Dictionary<string, string> splitedNovelDict = new Dictionary<string, string>();
    private string[] selectedImages; // 씬마다의 이미지를 저장하는 배열
    private int current_img_num;

    private string title = PlayerPrefs.GetString("newTitle", "default_title");


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
        //goToCreateInteractionButton.onClick.AddListener(SendInteractionRequest);
    }


    private void LoadImage(int buttonNumber)
    {
        //string title = PlayerPrefs.GetString("newTitle", "default_title");

        //title = "날아가는 펭귄"; //TEST!!!!!!!
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
        //maxScene = 3; //TEST!!!!!!!
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
        //string title = PlayerPrefs.GetString("newTitle", "default_title");
        //title = "날아가는 펭귄"; //TEST!!!!!!!
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
                LALO.SetActive(true);
            }
        }
    }


    public void DeleteNonSelectedImages()
    {
        //string title = PlayerPrefs.GetString("newTitle", "default_title");
        //title = "펭펭펭펭날아가";///TEST!!!!
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

    public void MoveFile()
    {
        //string title = PlayerPrefs.GetString("newTitle", "default_title");
        string tempFolderPath = Application.persistentDataPath + "/SaveFile/temp/";
        string destinationFolderPath = Application.persistentDataPath + "/SaveFile/";

        // Ensure the destination directory exists
        if (!Directory.Exists(destinationFolderPath))
        {
            Directory.CreateDirectory(destinationFolderPath);
        }

        // Get the next available number for the filename
        int nextNumber = GetNextAvailableNumber(destinationFolderPath);
        string destinationFileName = $"{nextNumber}-{title}.png";
        string sourceFilePath = tempFolderPath + title + ".png";
        string destinationFilePath = destinationFolderPath + destinationFileName;

        // Check if the file exists in the source directory
        if (File.Exists(sourceFilePath))
        {
            // Move the file to the destination directory
            File.Move(sourceFilePath, destinationFilePath);
            Debug.Log($"File moved successfully to {destinationFilePath}");
        }
        else
        {
            Debug.LogWarning($"File not found: {sourceFilePath}");
        }
    }

    private int GetNextAvailableNumber(string folderPath)
    {
        int maxNumber = -1;
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        FileInfo[] files = directoryInfo.GetFiles("*.png");

        foreach (FileInfo file in files)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
            string[] parts = fileNameWithoutExtension.Split('-');

            if (parts.Length > 1 && int.TryParse(parts[0], out int number))
            {
                if (number > maxNumber)
                {
                    maxNumber = number;
                }
            }
        }

        return maxNumber + 1;
    }
}
