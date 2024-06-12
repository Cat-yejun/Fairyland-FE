using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Alart : MonoBehaviour
{
    private static Alart instance;

    public event Action<int> OnValueChange;

    private int previousValue;

    public static object Instance { get; internal set; }

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
        previousValue = PlayerPrefs.GetInt("isNew", 0);
        // Start the coroutine to check the value periodically
        StartCoroutine(CheckPlayerPrefs());

        // Start the coroutine to toggle the value periodically
        //StartCoroutine(TogglePlayerPrefs());
    }

    IEnumerator CheckPlayerPrefs()
    {
        while (true)
        {
            // Wait for a specified amount of time before checking again
            yield return new WaitForSeconds(3.0f);

            // Get the current value from PlayerPrefs
            int currentValue = PlayerPrefs.GetInt("isNew", 0);

            // Check if the value has changed
            if (currentValue != previousValue)
            {
                // The value has changed, perform necessary actions
                Debug.Log("Value of 'isNew' has changed from " + previousValue + " to " + currentValue);

                // Trigger the event
                OnValueChange?.Invoke(currentValue);

                // Update the previous value
                previousValue = currentValue;
            }
        }
    }

    IEnumerator TogglePlayerPrefs()
    {
        while (true)
        {
            // Wait for 5 seconds before toggling the value
            yield return new WaitForSeconds(5.0f);

            // Get the current value from PlayerPrefs
            int currentValue = PlayerPrefs.GetInt("isNew", 0);

            // Toggle the value between 0 and 1
            int newValue = currentValue == 0 ? 1 : 0;
            PlayerPrefs.SetInt("isNew", newValue);
            PlayerPrefs.Save(); // Ensure the value is saved

            Debug.Log("Toggled 'isNew' value to " + newValue);
        }
    }
}
