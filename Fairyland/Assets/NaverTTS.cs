using UnityEngine;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class NaverTTSManager : MonoBehaviour
{
    
    private AudioSource audioSource;

    private string clientId = "t053d93bde";
    private string clientSecret = "Os71Z2Xm9LPAjAu5PbbHHQCeHqEk7Hzl8OsCLFpw";
    private string WhaleTalking = "페니는 하늘을 날고 싶은 꿈을 포기하지 않고 계속해서 노력하고 있어요. 페니가 어떤 마음가짐을 가지고 있는지 생각하면서, 그가 이 상황에서 할 법한 말은 무엇일까요? 말해보세요.";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        //GetAndPlaySpeech("빈칸에 들어갈 대사는 무엇일까?");
        GetAndPlaySpeech(WhaleTalking);
    }

    public void GetAndPlaySpeech(string text)
    {
        GetSpeech(text);
    }


    private void GetSpeech(string text)
    {
        string url = "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("X-NCP-APIGW-API-KEY-ID", clientId);
        request.Headers.Add("X-NCP-APIGW-API-KEY", clientSecret);
        request.Method = "POST";
        byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=nara&volume=0&speed=0&pitch=0&format=mp3&text=" + text);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteDataParams.Length;

        using (Stream st = request.GetRequestStream())
        {
            st.Write(byteDataParams, 0, byteDataParams.Length);
        }

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            string status = response.StatusCode.ToString();
            Debug.Log("status=" + status);
            string filePath = Path.Combine(Application.persistentDataPath, "tts.mp3");
            using (Stream output = File.OpenWrite(filePath))
            using (Stream input = response.GetResponseStream())
            {
                input.CopyTo(output);
            }
            Debug.Log(filePath + " was created");

            // 파일 저장 후 재생

            //StartCoroutine(LoadAndPlayAudio(filePath));
        }
    }

   
}