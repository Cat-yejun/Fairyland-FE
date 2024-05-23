using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageLoader : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button leftButton;
    public Button rightButton;
    public RawImage rawImage;
    private const string PATH = "/SaveFile/";

    private int currentScene = 1; // 현재 씬 번호
    //public int maxScene = 3; // 최대 씬 번호

    void Start()
    {
        
        button1.onClick.AddListener(() => LoadImage(1));
        button2.onClick.AddListener(() => LoadImage(2));
        button3.onClick.AddListener(() => LoadImage(3));
        leftButton.onClick.AddListener(() => ChangeScene(-1));
        rightButton.onClick.AddListener(() => ChangeScene(1));
    }

    private void LoadImage(int buttonNumber)
    {
        string title = PlayerPrefs.GetString("title", "default_title"); // "default_title"은 기본값
        
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
        currentScene += change;
        if (currentScene < 1)
        {
            currentScene = 1;
        }
        else if (currentScene > maxScene)
        {
            currentScene = maxScene;
        }
        Debug.Log("Current Scene: " + currentScene);
    }
}
