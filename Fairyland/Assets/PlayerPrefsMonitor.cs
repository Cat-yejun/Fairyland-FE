using UnityEngine;
using System.Collections;
using System;

public class PlayerPrefsMonitor : MonoBehaviour
{
    private static PlayerPrefsMonitor instance;

    public event Action<string> OnTitleChange;

    private string key = "title";
    private string previousValue;

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
        previousValue = PlayerPrefs.GetString(key, "default_title");
        // Start the coroutine to check the value periodically
        StartCoroutine(CheckPlayerPrefs());
    }

    IEnumerator CheckPlayerPrefs()
    {
        while (true)
        {
            // Wait for a specified amount of time before checking again
            yield return new WaitForSeconds(3.0f);

            // Get the current value from PlayerPrefs
            string currentValue = PlayerPrefs.GetString(key, "default_title");

            // Check if the value has changed
            if (currentValue != previousValue)
            {
                // The value has changed, perform necessary actions
                Debug.Log("Value of 'title' has changed from " + previousValue + " to " + currentValue);

                // Trigger the event
                OnTitleChange?.Invoke(currentValue);

                // Update the previous value
                previousValue = currentValue;
            }
        }
    }
}
