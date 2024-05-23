using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ImageLoader : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button leftButton;
    public Button rightButton;
    public RawImage rawImage;
    public TMP_Text tmpText;
    private const string PATH = "/SaveFile/";
    private int currentScene = 1; // 현재 씬 번호
    private Dictionary<string, string> novelNumDict = new Dictionary<string, string>();
    private Dictionary<string, string> splitedNovelDict = new Dictionary<string, string>();

    void Start()
    {
        button1.onClick.AddListener(() => LoadImage(1));
        button2.onClick.AddListener(() => LoadImage(2));
        button3.onClick.AddListener(() => LoadImage(3));
        leftButton.onClick.AddListener(() => ChangeScene(-1));
        rightButton.onClick.AddListener(() => ChangeScene(1));

        // JSON 파일 읽기 및 딕셔너리 변환
        LoadJson();

        // 씬이 변경될 때마다 텍스트 업데이트
        ChangeScene(0);
    }

    private void LoadImage(int buttonNumber)
    {
        string title = PlayerPrefs.GetString("title", "default_title");
        title = "소나기123"; //TEST!!!!!!!

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
        string title = "소나기123"; //TEST!!!!!!!
        int maxScene = PlayerPrefs.GetInt("split", 1) - 1;
        maxScene = 2; //TEST!!!!!!!
        currentScene += change;
        if (currentScene < 1)
        {
            currentScene = 1;
        }
        else if (currentScene > maxScene)
        {
            currentScene = maxScene;
        }

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
        string title = "소나기123"; //TEST!!!!!!!
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
}
