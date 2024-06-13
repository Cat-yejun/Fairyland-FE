using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestButtonController : MonoBehaviour
{
    //public Button buttonToControl;
    public TextMeshProUGUI LaLoText;
    //public Sprite valueOneSprite; // Sprite for when the value is 1
    //public Sprite defaultSprite;  // Default sprite for other values

    void Start()
    {
        // Find the PlayerPrefsMonitor instance in the scene
        Alart monitorInstance = FindObjectOfType<Alart>();
        Debug.Log("테스트 버튼 컨트롤러");
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

            LaLoText.text = "동화가 도착했어!";
            // Change the button's image
            //buttonToControl.image.sprite = valueOneSprite;
        }
        else if (newValue == -1)
        {
            LaLoText.text = "동화 만드는 중...";
        }
        else
        {
            LaLoText.text = "새로운 동화를 만들어볼까?";
            // Change the button's image to the default sprite
            //buttonToControl.image.sprite = defaultSprite;
        }
    }
}
