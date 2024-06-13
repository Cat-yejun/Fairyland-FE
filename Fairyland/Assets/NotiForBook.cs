using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotiForBook : MonoBehaviour
{
    public Button buttonToControl;
    

    void Start()
    {
        // Get the current value from PlayerPrefs
        int currentValue = PlayerPrefs.GetInt("isNew", 0);

        HandleValueChange(currentValue);
        // Find the PlayerPrefsMonitor instance in the scene
        Alart monitorInstance = FindObjectOfType<Alart>();
        Debug.Log("북 알림 테스트 버튼 컨트롤러");
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

            
            buttonToControl.gameObject.SetActive(true);
        }
      
        else
        {
            buttonToControl.gameObject.SetActive(false);
        }
    }
}
