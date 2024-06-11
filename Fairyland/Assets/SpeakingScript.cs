using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine.Windows;
using TMPro;

[Serializable]
public class TranscriptData
{
    public string transcript;
    public float similarity;
}

[Serializable]
public class EmotionData
{
    public string emotion;
}


public class SpeakingScript : MonoBehaviour
{

    public GameObject SpeakStartCanvas;
    public GameObject SpeakStopCanvas;
    public GameObject NextButtonCanvas;
    public GameObject AskLineGuessCanvas;
    public Button speakStopButton; // speakStop 버튼
    public Button speakStartButton; // speakStart 버튼
    private float initialDelay = 2f; // 초기 지연 시간
    private float displayDuration = 3f; // 텍스트와 버튼 표시 시간

    private AudioSource audioSource;
    private bool isRecording = false;

    public GameObject CorrectCanvas;
    public GameObject WrongCanvas;

    private string[] EmotionAnswer = { "calm", "neutral"};
    private float AccuracyPass = 0.5f;
    private float currentAccuracy = 1.0f;
    private string currentEmotion = "";

    private readonly string serverUrlEmotion = "http://a249-125-132-126-243.ngrok-free.app/predict-emotion/";
    private readonly string serverUrlSimilarity = "http://a249-125-132-126-243.ngrok-free.app/asr-similarity/";
    private readonly string groundTruth = "";

    private Book BookClass;

    public TextMeshProUGUI CorrectText;

    private NaverTTSManager TTSManager;

    private string[] emotionKoreanArray = { "평온함", "기쁨", "슬", "화남", "무서움", "놀라움", "Main" };
    string[] emotionArray = { "calm", "happy", "sad", "anger", "fear", "surprise", "main" };



    public void StartUpload(string answer)
    {
        StartCoroutine(UploadAudioAndAnalyze(answer));
    }

    private IEnumerator UploadAudioAndAnalyze(string answer)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "MyRecording.wav");

        if (System.IO.File.Exists(filePath))
        {
            Debug.Log("File exists, starting upload...");
            yield return StartCoroutine(UploadFile(filePath, answer));
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    private IEnumerator UploadFile(string filePath, string answer)
    {
        // Emotion Recognition
        byte[] fileContent = System.IO.File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("audiofile", fileContent, Path.GetFileName(filePath), "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrlEmotion, form))
        {
            Debug.Log("http 직전 - Emotion");
            yield return www.SendWebRequest();
            Debug.Log("http 직후 - Emotion");

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Emotion Error: {www.error}");
            }
            else
            {
                Debug.Log($"Emotion Response: {www.downloadHandler.text}");
                string inputEmotion = www.downloadHandler.text;
                EmotionData emotionData = JsonUtility.FromJson<EmotionData>(inputEmotion);
                currentEmotion = emotionData.emotion;
                Debug.Log("Converted similarity value: " + currentEmotion);
            }
        }

        // Similarity Comparison
        form = new WWWForm();
        form.AddBinaryData("audio_file", fileContent, Path.GetFileName(filePath), "audio/wav");
        form.AddField("groundtruth", answer);

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrlSimilarity, form))
        {
            Debug.Log("http 직전 - Similarity");
            yield return www.SendWebRequest();
            Debug.Log("http 직후 - Similarity");

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Similarity Error: {www.error}");
            }
            else
            {
                string accuracy = www.downloadHandler.text;
                Debug.Log($"Similarity Response: {www.downloadHandler.text}");
                currentAccuracy = ConvertStringToFloat(accuracy);
                Debug.Log(currentAccuracy);
            }
        }
    }

    float ConvertStringToFloat(string input)
    {
        try
        {
            TranscriptData data = JsonUtility.FromJson<TranscriptData>(input);
            float similarityValue = data.similarity;
            Debug.Log("Converted similarity value: " + similarityValue);
            return similarityValue;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse JSON and convert similarity value: " + e.Message);
            return 0.0f;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        SpeakStartCanvas.SetActive(false);
        SpeakStopCanvas.SetActive(false);
        NextButtonCanvas.SetActive(false);
        CorrectCanvas.SetActive(false);
        WrongCanvas.SetActive(false);

        BookClass = GetComponent<Book>();
        TTSManager = GetComponent<NaverTTSManager>();


        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource component was missing and has been added.");
        }
    }


    public void OnSpeakStartButtonClick()
    {
        SpeakStopCanvas.SetActive(true);
        speakStopButton.interactable = true;
        SpeakStartCanvas.SetActive(false);

        StartRecording();
        //currentAccuracy = 0.9f;
        //currentEmotion = "calm";
        // 음성을 받아오는 로직 추가
    }

    public void OnSpeakStopButtonClick()
    {
        SpeakStopCanvas.SetActive(false);
        SpeakStartCanvas.SetActive(true);
        speakStartButton.interactable = false;

        
        //string filePath = Path.Combine(Application.persistentDataPath, "MyRecording.wav");

        //if (File.Exists(filePath))
        //{                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
        //    StartUpload();
        //}
        //else
        //{
        //    Debug.LogError("File not found: " + filePath);
        //}


        StartCoroutine(SpeakStopSequence());
    }


    IEnumerator SpeakStopSequence()
    {
        string answerText = BookClass.LineGuessAnswer;
        Debug.Log("answer is : " + answerText);
        StopRecording();
        SaveRecording();
        StartUpload(answerText);

        yield return new WaitForSeconds(2.0f);

        //bool exists = Array.Exists(EmotionAnswer, element => element == currentEmotion);
        string emotion = emotionArray[BookClass.emotionInteger];

        bool exists = (currentEmotion == emotion) ? true : false;

        Debug.Log("emotion : " + currentEmotion + ", accuracy : " + currentAccuracy);


        if (currentAccuracy > AccuracyPass)
        {
            if(exists)
            {
                TTSManager.GetAndPlaySpeech("vdain", "Happy", "맞았어요!", "CorrectAnswer");

                CorrectCanvas.SetActive(true);
                WrongCanvas.SetActive(false);

            }
            else
            {
                string tryAgain = $"{emotionKoreanArray[BookClass.emotionInteger]} 의 감정에 맞게 다시 말해봐.";
                CorrectText.text = tryAgain;

                Debug.Log(tryAgain);

                TTSManager.GetAndPlaySpeech("vdain", "Neutral", tryAgain, "WrongExplain");

                WrongCanvas.SetActive(true);
                CorrectCanvas.SetActive(false);
            }
        }
        else
        {
            TTSManager.GetAndPlaySpeech("vdain", "Neutral", "상황에 맞는 대사로 대답해봐!", "WrongExplain");
            CorrectText.text = "상황에 맞는 대사로 대답해봐.";

            WrongCanvas.SetActive(true);
            CorrectCanvas.SetActive(false);
        }

        yield return new WaitForSeconds(3.0f);

        CorrectCanvas.SetActive(false);
        WrongCanvas.SetActive(false);
        SpeakStartCanvas.SetActive(false);
        SpeakStopCanvas.SetActive(false);
        NextButtonCanvas.SetActive(false);
        AskLineGuessCanvas.SetActive(false);

        //StartCoroutine(Sequence());
        //NextButtonCanvas.SetActive(true);
        BookClass.StartGotoOriginalPos();
        BookClass.firstButtonPress = false;

        CorrectText.text = "틀렸어요.";

    }

    void StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            string microphone = Microphone.devices[0];
            audioSource.clip = Microphone.Start(microphone, false, 10, 44100);
            isRecording = true;
            Debug.Log("Recording started...");
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("Recording stopped.");
        }
    }

    void PlayRecording()
    {
        if (!isRecording && audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("Playing recording...");
        }
        else
        {
            Debug.LogError("No recording to play or still recording.");
        }
    }

    void SaveRecording()
    {
        if (!isRecording && audioSource.clip != null)
        {
            WAVUtility.Save("MyRecording.wav", audioSource.clip);
            Debug.Log("Recording saved as MyRecording.wav");
        }
        else
        {
            Debug.LogError("No recording to save or still recording.");
        }
    }
}




public static class WAVUtility
{
    const int HEADER_SIZE = 44;

    public static void Save(string filename, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        var filepath = Path.Combine(Application.persistentDataPath, filename);

        Debug.Log("Saving file to: " + filepath);

        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(filepath));

        using (var fileStream = CreateEmpty(filepath))
        {
            ConvertAndWrite(fileStream, clip);
            WriteHeader(fileStream, clip);
        }
    }

    private static FileStream CreateEmpty(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++)
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }

    private static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        Byte[] bytesData = new Byte[samples.Length * 2];

        const float rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    private static void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 one = 1;
        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
    }
}