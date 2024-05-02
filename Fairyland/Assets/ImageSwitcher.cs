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

    void Update()
    {
        // Check for screen touch or mouse click
        if (Input.GetButtonDown("Fire1")) // Change "Fire1" to your desired input axis
        {
            // Deactivate the current image
            panel.GetChild(currentIndex).gameObject.SetActive(false);

            // Increment the index
            currentIndex++;

            // If we have reached the last image, exit the function
            if (currentIndex >= panel.childCount)
            {
                // You can add any additional logic here if needed
                Debug.Log("Reached the end of images.");
                return;
            }

            // Activate the next image
            panel.GetChild(currentIndex).gameObject.SetActive(true);
        }
    }
}
