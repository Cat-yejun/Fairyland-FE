using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MenuScreenController : MonoBehaviour
{
    public RawImage rawImagePrefab; // Prefab of RawImage to instantiate
    public RectTransform container; // Container to hold the RawImage objects
    public string directoryPath; // Path to the directory containing images

    void Start()
    {
        // Get the file paths from the directory
        string[] filePaths = Directory.GetFiles(Path.Combine(Application.persistentDataPath, directoryPath));

        // Calculate the gap between images
        float gapBetweenImages = 10f; // You can adjust this value

        // Initial position for the first image (adjust as needed)
        float initialXPosition = -1200f;
        float currentXPosition = initialXPosition;

        // Loop through each file path
        foreach (string filePath in filePaths)
        {
            // Create a new raw image object
            RawImage rawImage = Instantiate(rawImagePrefab, container);

            // Load the texture from the file
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            // Set the texture of the raw image
            rawImage.texture = texture;

            // Set the size of the raw image (adjust width and height as needed)
            rawImage.rectTransform.sizeDelta = new Vector2(300, 300); // Adjust width and height as needed

            // Set the position of the raw image (adjust xPosition and yPosition as needed)
            rawImage.rectTransform.anchoredPosition = new Vector2(currentXPosition+40, 0); // Adjust xPosition and yPosition as needed

            // Update the current X position for the next image
            currentXPosition += rawImage.rectTransform.sizeDelta.x + gapBetweenImages;
        }
    }


}
