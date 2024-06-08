using UnityEngine;
using TMPro;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class ApiCaller : MonoBehaviour
{
    public TMP_Text responseText;

    // Static instance to make this class a Singleton
    private static ApiCaller instance;

    // Flag to track whether the API call has been made
    private bool hasCalledApi = false;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Check if the instance already exists
        if (instance == null)
        {
            // If not, set the instance to this object
            instance = this;

            // Make sure this GameObject persists between scene changes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this object
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        Debug.Log("hasCalledApi: " + hasCalledApi);

        // Call CheckServerStatus only if the API hasn't been called yet
        if (!hasCalledApi)
        {
            SceneManager.LoadScene("new_Book_manu");
            await CheckServerStatus();
            hasCalledApi = true;
        }

        //SceneManager.LoadScene("new_Book_manu");
        

        await CheckServerStatus2();
    }

    public async Task CheckServerStatus()
    {
        string url = "http://43.201.252.166:8000/hello";
        Debug.Log("Sending request...");

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(100); // Adjust timeout as needed
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response from server: " + jsonResponse);

                    // Display server response in TextMeshPro text element
                    responseText.text = jsonResponse;
                    string title = responseText.text;
                    PlayerPrefs.SetString("title", title);
                    PlayerPrefs.Save();
                    Debug.Log("Title saved: " + title);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Request error: " + response.StatusCode + " - " + errorResponse);
                }
            }
        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
        }
        catch (TaskCanceledException e)
        {
            Debug.LogError("Request timeout: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Unexpected error: " + e.Message);
        }
    }

    public async Task CheckServerStatus2()
    {
        Debug.Log("CheckServerStatus2 실행");
        string url = "http://43.201.252.166:8000/hello";
        Debug.Log("22Sending request...");

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(100); // Adjust timeout as needed
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.Log("22Response from server: " + jsonResponse);

                    // Display server response in TextMeshPro text element
                    responseText.text = jsonResponse;
                    string title = responseText.text;
                    title = title + "2";
                    PlayerPrefs.SetString("title", title);
                    PlayerPrefs.Save();
                    Debug.Log("Title saved: " + title);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Request error: " + response.StatusCode + " - " + errorResponse);
                }
            }
        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
        }
        catch (TaskCanceledException e)
        {
            Debug.LogError("Request timeout: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Unexpected error: " + e.Message);
        }
    }
}
