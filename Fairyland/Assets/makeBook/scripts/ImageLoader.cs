using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageLoader : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public RawImage rawImage;
    private const string PATH = "/SaveFile/";

    void Start()
    {
        button1.onClick.AddListener(() => LoadImage(1));
        button2.onClick.AddListener(() => LoadImage(2));
        button3.onClick.AddListener(() => LoadImage(3));
    }

    private void LoadImage(int buttonNumber)
    {
        string title = PlayerPrefs.GetString("title", "default_title"); // "default_title"은 기본값
        
        string path = $"{Application.persistentDataPath}{PATH}{title}/img/1-{buttonNumber - 1}.png";

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
}
