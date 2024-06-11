
using UnityEngine;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using static Unity.VisualScripting.Member;

public class NaverTTSforRolePlaying : MonoBehaviour
{

    private AudioSource audioSource;

    private string clientId = "t053d93bde";
    private string clientSecret = "Os71Z2Xm9LPAjAu5PbbHHQCeHqEk7Hzl8OsCLFpw";

    private string[] texts;
    private string title;

    public Book BookClass;

    private static List<AudioSource> activeAudioSources = new List<AudioSource>();

    private Coroutine currentCoroutine;
    private RolePlayingScript RolePlayingClass;


    //void LoadTextsToPages()
    //{
    //    string[] filePaths = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "SaveFile", title, "interaction"), "*.txt");
    //    Debug.Log("Total files found: " + filePaths.Length);

    //    int fileLength = filePaths.Length;

    //    texts = new string[fileLength];

    //    for (int pageIndex = 0; pageIndex < fileLength; pageIndex++)
    //    {
    //        Debug.Log("Loading text from: " + filePaths[pageIndex]);

    //        string textContent = File.ReadAllText(filePaths[pageIndex]);
    //        texts[pageIndex] = textContent;
    //    }

    //    Debug.Log("texts Length : " + texts.Length);
    //}



    void Start()
    {

        //title = PlayerPrefs.GetString("title", "defaultTitle");
        title = "소나기";
        RolePlayingClass = GetComponent<RolePlayingScript>();

        //LoadTextsToPages();

        //audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.Log("audioSource added in NaverTTS");
            audioSource = gameObject.AddComponent<AudioSource>();
        }


        //GetAndPlaySpeech("빈칸에 들어갈 대사는 무엇일까?");
        //GetAndPlaySpeech("ndain", "Angry", WhaleTalking, "tts");

        //Debug.Log(texts[0]);
        //Debug.Log(texts[1]);
        //GetAndPlaySpeech("ndain", "Angry", texts[0], "firstLine");
        //GetAndPlaySpeech("ndain", "Angry", texts[1], "secondLine");


    }


    public void GetAndPlaySpeech(string speaker, string emotion, string text, string filename)
    {
        GetSpeech(speaker, emotion, text, filename);
    }

    public IEnumerator LoadAndPlayAudio(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip != null && RolePlayingClass.audioSourceRolePlaying != null)
                {
                    if (audioSource == null)
                    {
                        RolePlayingClass.audioSourceRolePlaying.clip = clip;
                        RolePlayingClass.audioSourceRolePlaying.Play();
                        //yield return new WaitForSeconds(RolePlayingClass.audioSourceRolePlaying.clip.length + 1.0f);

                    }

                    else
                    {
                        audioSource.clip = clip;
                        audioSource.Play();
                    }
                }
                else
                {
                    if (clip == null)
                    {
                        Debug.LogError("AudioClip is null.");
                    }

               
                }
            }
        }
    }

    private void StopAllActiveAudio()
    {
        foreach (AudioSource source in activeAudioSources)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
        activeAudioSources.Clear();
    }

    private void RegisterActiveAudioSource(AudioSource source)
    {
        activeAudioSources.Add(source);
    }

    private void GetSpeech(string speaker, string emotion, string text, string filename)
    {
        if (speaker == null)
        {
            speaker = "vdain";
        }

        int emotionInteger = 0;

        if (emotion == "Neutral")
        {
            emotionInteger = 0;
        }
        else if (emotion == "Sad")
        {
            emotionInteger = 1;
        }
        else if (emotion == "Happy")
        {
            emotionInteger = 2;
        }
        else if (emotion == "Angry")
        {
            emotionInteger = 3;
        }

        if (currentCoroutine != null)
        {
            if (audioSource == null)
            {
                RolePlayingClass.audioSourceRolePlaying.Stop();
                StopCoroutine(currentCoroutine);
                Debug.Log("current coroutine stopping...");
            }
            else
            {
                audioSource.Stop();
                StopCoroutine(currentCoroutine);
                Debug.Log("current coroutine stopping...");

            }

        }

        string url = "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("X-NCP-APIGW-API-KEY-ID", clientId);
        request.Headers.Add("X-NCP-APIGW-API-KEY", clientSecret);
        request.Method = "POST";
        string postData = $"speaker={speaker}&volume=0&speed=0&pitch=0&format=mp3&text={text}&emotion={emotionInteger}&emotion-strength=2";
        byte[] byteDataParams = Encoding.UTF8.GetBytes(postData);
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
            string filePath = Path.Combine(Application.persistentDataPath, filename + ".mp3");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (Stream output = File.OpenWrite(filePath))
            using (Stream input = response.GetResponseStream())
            {
                input.CopyTo(output);
            }
            Debug.Log(filePath + " was created");

            // 파일 저장 후 재생


            currentCoroutine = StartCoroutine(LoadAndPlayAudio(filePath));
        }
    }


}