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
    private string WhaleTalking = "빈칸에 들어갈 주인공의 대사는 무엇일까?";

    private string[] texts;
    private string title;

    void LoadTextsToPages()
    {
        string[] filePaths = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "SaveFile", title, "interaction"), "*.txt");
        Debug.Log("Total files found: " + filePaths.Length);

        int fileLength = filePaths.Length;

        texts = new string[fileLength];

        for (int pageIndex = 0; pageIndex < fileLength; pageIndex++)
        {
            Debug.Log("Loading text from: " + filePaths[pageIndex]);

            string textContent = File.ReadAllText(filePaths[pageIndex]);
            texts[pageIndex] = textContent;
        }

        Debug.Log("texts Length : " + texts.Length);
    }



    void Start()
    {
        title = PlayerPrefs.GetString("title", "defaultTitle");

        LoadTextsToPages();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        //GetAndPlaySpeech("빈칸에 들어갈 대사는 무엇일까?");
        GetAndPlaySpeech(WhaleTalking, "tts");

        Debug.Log(texts[0]);
        Debug.Log(texts[1]);
        GetAndPlaySpeech(texts[0], "firstLine");
        GetAndPlaySpeech(texts[1], "secondLine");


    }

    public void GetAndPlaySpeech(string text, string filename)
    {
        GetSpeech(text, filename);
    }


    private void GetSpeech(string text, string filename)
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
            string filePath = Path.Combine(Application.persistentDataPath, filename+".mp3");
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