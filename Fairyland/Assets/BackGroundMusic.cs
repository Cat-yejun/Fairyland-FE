using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;


public class BackGroundMusic : MonoBehaviour
{
    public AudioSource backgroundMusicSource;

    private static BackGroundMusic instance = null;

    public static BackGroundMusic Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        string BGMPath = Path.Combine(Application.persistentDataPath, "BGM.mp3");
        StartCoroutine(LoadBackgroundMusic(BGMPath));

    }


    public IEnumerator LoadBackgroundMusic(string path)
    {
        if (File.Exists(path))
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
                    PlayBackgroundMusic(clip);
                }
            }
        }
        else
        {
            Debug.LogError("File not found at: " + path);
        }
    }



    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (backgroundMusicSource != null)
        {
            Debug.Log("backgroundmusic play");
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.Log("backgroundmusic is null!");
        }
    }

    public void StopMusic()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }
    }

}
