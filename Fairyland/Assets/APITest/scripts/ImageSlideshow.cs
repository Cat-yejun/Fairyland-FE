using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlideshow : MonoBehaviour
{
    public string imagesFolderPath = "Assets/savefile/소나기/img"; // Path to the folder containing images
    public RawImage displayImage; // UI RawImage to display the images
    public float interval = 5.0f; // Interval between images

    private List<Texture2D> images;
    private int currentImageIndex = 0;

    void Start()
    {
        // Load images from the folder
        LoadImages();

        // Start the coroutine to display images
        if (images.Count > 0)
        {
            StartCoroutine(DisplayImages());
        }
        else
        {
            Debug.LogError("No images found in the specified folder.");
        }
    }

    void LoadImages()
    {
        images = new List<Texture2D>();

        // Get all files in the specified folder
        string[] filePaths = Directory.GetFiles(imagesFolderPath);

        foreach (string filePath in filePaths)
        {
            if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg"))
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(fileData); // Load image data into the texture
                images.Add(texture);
            }
        }
    }

    IEnumerator DisplayImages()
    {
        while (true)
        {
            // Display the current image
            displayImage.texture = images[currentImageIndex];

            // Wait for the specified interval
            yield return new WaitForSeconds(interval);

            // Move to the next image
            currentImageIndex = (currentImageIndex + 1) % images.Count;
        }
    }
}
