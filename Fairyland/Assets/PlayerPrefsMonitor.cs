using UnityEngine;
using System.Collections;

public class PlayerPrefsMonitor : MonoBehaviour
{
    private static PlayerPrefsMonitor instance;

    private string key = "title";
    private int previousValue;

    // Singleton pattern
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize the previous value from PlayerPrefs
        previousValue = PlayerPrefs.GetInt(key, 0);
        // Start the coroutine to check the value periodically
        StartCoroutine(CheckPlayerPrefs());
    }

    IEnumerator CheckPlayerPrefs()
    {
        while (true)
        {
            // Wait for a specified amount of time before checking again
            yield return new WaitForSeconds(1.0f);
            Debug.Log("title 찾기 계속 동작");

            // Get the current value from PlayerPrefs
            int currentValue = PlayerPrefs.GetInt(key, 0);

            // Check if the value has changed
            if (currentValue != previousValue)
            {
                // The value has changed, perform necessary actions
                Debug.Log("Value of 'title' has changed from " + previousValue + " to " + currentValue);

                // Update the previous value
                previousValue = currentValue;
            }
        }
    }
}
