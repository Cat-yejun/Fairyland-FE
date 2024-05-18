using UnityEngine;
using UnityEngine.Networking;
using TMPro; // TextMeshPro를 사용하기 위해 추가합니다.
using System.Collections;

public class ApiCaller : MonoBehaviour
{
    public TextMeshProUGUI responseText; // API 응답을 표시할 텍스트 필드

    // 시작할 때 호출됩니다.
    void Start()
    {
        StartCoroutine(PostRequest("http://43.201.252.166:8000/doyouworking")); // Flask 서버의 주소로 변경해주세요.
    }

    // POST 요청을 보내고 응답을 받는 메소드
    IEnumerator PostRequest(string uri)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(uri, "POST"))
        {
            // 빈 데이터를 바디로 설정합니다.
            webRequest.uploadHandler = new UploadHandlerRaw(new byte[0]);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // 요청을 보냅니다.
            yield return webRequest.SendWebRequest();

            // 에러 체크
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                // 응답 텍스트를 표시합니다.
                responseText.text = webRequest.downloadHandler.text;
            }
        }
    }
}
