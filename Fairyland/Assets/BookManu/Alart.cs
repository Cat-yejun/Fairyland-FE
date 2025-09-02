using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Alart : MonoBehaviour
{
    private static Alart instance;

    public event Action<int> OnValueChange;

    //private string key = "value";
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
        PlayerPrefs.SetInt("isNew", 0);
        PlayerPrefs.Save();
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
            int currentValue = PlayerPrefs.GetInt("isNew", 0);

            // Check if the value has changed
            if (currentValue != previousValue)
            {
                // The value has changed, perform necessary actions
                Debug.Log("Value of 'value' has changed from " + previousValue + " to " + currentValue);

                // Trigger the event
                OnValueChange?.Invoke(currentValue);

                // Update the previous value
                previousValue = currentValue;
            }
        }
    }
}
