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
        StartCoroutine(GetRequest("http://127.0.0.1:5000/api")); // Flask 서버의 주소로 변경해주세요.
    }

    // GET 요청을 보내고 응답을 받는 메소드
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
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
