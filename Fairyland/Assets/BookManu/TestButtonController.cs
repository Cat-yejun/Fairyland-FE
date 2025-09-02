using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestButtonController : MonoBehaviour
{
    //public Button buttonToControl;
    public TextMeshProUGUI LaLoText;
    //public Sprite valueOneSprite; // Sprite for when the value is 1
    //public Sprite defaultSprite;  // Default sprite for other values
    public Transform whale;
    public GameObject WhaleObject;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Animator whaleAnimator;
    public Material[] expressions;

    void Start()
    {
        // Get the current value from PlayerPrefs
        int currentValue = PlayerPrefs.GetInt("isNew", 0);

        HandleValueChange(currentValue);
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

    public void changeExpression(int expression)
    {
        SkinnedMeshRenderer renderer = skinnedMeshRenderer;
        if (renderer != null)
        {
            renderer.material = expressions[expression];
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer not found on the game object.");
        }
    }

    void HandleValueChange(int newValue)
    {
        // Check if the value meets a certain condition (for example, if it's 1)
        if (newValue == 1)
        {

            LaLoText.text = "동화가 도착했어!";
            whaleAnimator.SetInteger("NextInt", 2);
            changeExpression(2);
            // Change the button's image
            //buttonToControl.image.sprite = valueOneSprite;
        }
        else if (newValue == -1)
        {
            whaleAnimator.SetInteger("NextInt", 1);
            changeExpression(0);
            LaLoText.text = "동화 만드는 중...";
        }
        else
        {

            LaLoText.text = "새로운 동화를 만들어볼까?";
            whaleAnimator.SetInteger("NextInt", 0);
            changeExpression(0);
            // Change the button's image to the default sprite
            //buttonToControl.image.sprite = defaultSprite;
        }
    }
}
