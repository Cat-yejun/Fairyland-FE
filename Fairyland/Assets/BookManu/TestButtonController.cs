using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestButtonController : MonoBehaviour
{
    public Button buttonToControl;
    public TextMeshProUGUI buttonText;

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
            // Change the button's text
            buttonText.text = newTitle;
        }
        else
        {
            // Set a default text when title is empty
            buttonText.text = "Default Text";
        }
    }
}
