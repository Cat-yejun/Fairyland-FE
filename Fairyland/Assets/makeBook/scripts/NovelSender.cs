using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

public class NovelSender : MonoBehaviour
{
    public TMP_InputField userInputField;

    public async void SendNovelToServer()
    {
        string novel = userInputField.text;
        string url = "http://43.201.252.166:8000/make-novel";
        string jsonData = "{\"source\": \"" + novel + "\", \"split\": 16, \"length_limit\": 70}";

        Debug.Log("Data sent to server: " + jsonData); // Logging the data sent to the server

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(100); // Set a reasonable timeout
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Debug.Log("Response from server: " + jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("Request error: " + e.Message);
            }
            catch (TaskCanceledException e)
            {
                Debug.LogError("Request timeout: " + e.Message);
            }
        }
    }
}
