using UnityEngine;
using UnityEngine.UI;

public class TestButtonController : MonoBehaviour
{
    public Button buttonToControl;

    void Start()
    {
        // Find the PlayerPrefsMonitor instance in the scene
        PlayerPrefsMonitor monitorInstance = FindObjectOfType<PlayerPrefsMonitor>();

        // Check if instance exists before subscribing to the event
        if (monitorInstance != null)
        {
            // Subscribe to the event from PlayerPrefsMonitor
            monitorInstance.OnTitleChange += HandleTitleChange;
        }
        else
        {
            Debug.LogError("PlayerPrefsMonitor instance not found in the scene!");
        }
    }

    void HandleTitleChange(string newTitle)
    {
        // Check if the title meets a certain condition (for example, if it's not empty)
        if (!string.IsNullOrEmpty(newTitle))
        {
            // Make the button visible
            buttonToControl.gameObject.SetActive(true);
        }
        else
        {
            // Hide the button
            buttonToControl.gameObject.SetActive(false);
        }
    }
}
