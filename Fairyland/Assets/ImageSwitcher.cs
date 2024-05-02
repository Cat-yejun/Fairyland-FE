using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Transform panel; // Reference to the panel containing the images
    private int currentIndex = 0; // Index of the currently active image

    void Start()
    {
        // Ensure there are child images in the panel
        if (panel == null || panel.childCount == 0)
        {
            Debug.LogError("Panel is not assigned or does not contain any child images.");
            return;
        }

        // Deactivate all images except the first one
        for (int i = 1; i < panel.childCount; i++)
        {
            panel.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Function to switch to the next image
    public void NextImage()
    {
        // Deactivate the current image
        panel.GetChild(currentIndex).gameObject.SetActive(false);

        // Increment the index
        currentIndex++;

        // If we have reached the last image, reset to the first image
        if (currentIndex >= panel.childCount)
        {
            currentIndex = 0;
        }

        // Activate the next image
        panel.GetChild(currentIndex).gameObject.SetActive(true);
    }

    // Function to switch to the previous image
    public void PreviousImage()
    {
        // Deactivate the current image
        panel.GetChild(currentIndex).gameObject.SetActive(false);

        // Decrement the index
        currentIndex--;

        // If we have reached the first image, loop back to the last image
        if (currentIndex < 0)
        {
            currentIndex = panel.childCount - 1;
        }

        // Activate the previous image
        panel.GetChild(currentIndex).gameObject.SetActive(true);
    }
}
