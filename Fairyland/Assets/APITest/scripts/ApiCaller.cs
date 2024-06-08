using UnityEngine;
using TMPro;
using System.Net.Http;
using System;
using System.Threading.Tasks;

public class ApiCaller : MonoBehaviour
{
    public TMP_Text responseText;

    void Start()
    {
        CheckServerStatus();
    }


    public async void CheckServerStatus()
    {
        string url = "http://43.201.252.166:8000/hello";
        Debug.Log("요청을 보냈");

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(100); // 타임아웃 시간을 조정할 수 있습니다.
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response from server: " + jsonResponse);

                    // 서버 응답을 TextMeshPro 텍스트 요소에 표시
                    responseText.text = jsonResponse;
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
