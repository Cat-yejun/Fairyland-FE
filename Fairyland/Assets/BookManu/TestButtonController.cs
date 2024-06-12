using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestButtonController : MonoBehaviour
{
    public Button buttonToControl;
    public TextMeshProUGUI buttonText;
    public Sprite valueOneSprite; // Sprite for when the value is 1
    public Sprite defaultSprite;  // Default sprite for other values

    void Start()
    {
        // Find the PlayerPrefsMonitor instance in the scene
        Alart monitorInstance = FindObjectOfType<Alart>();

        // Check if instance exists before subscribing to the event
        if (monitorInstance != null)
        {
            // Subscribe to the event from PlayerPrefsMonitor
            monitorInstance.OnValueChange += HandleValueChange;
        }
        else
        {
            Debug.LogError("Alart instance not found in the scene!");
        }
    }

    void HandleValueChange(int newValue)
    {
        // Check if the value meets a certain condition (for example, if it's 1)
        if (newValue == 1)
        {
            // Change the button's text
            buttonText.text = "Value is 1";
            // Change the button's image
            buttonToControl.image.sprite = valueOneSprite;
        }
        else
        {
            // Set a default text when value is not 1
            buttonText.text = "Default Text";
            // Change the button's image to the default sprite
            buttonToControl.image.sprite = defaultSprite;
        }
    }
}
