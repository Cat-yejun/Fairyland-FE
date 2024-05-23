using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;



public class SpeakingScript : MonoBehaviour
{

    public GameObject SpeakStartCanvas;
    public GameObject SpeakStopCanvas;
    public GameObject NextButtonCanvas;
    public Button speakStopButton; // speakStop 버튼
    public Button speakStartButton; // speakStart 버튼
    private float initialDelay = 2f; // 초기 지연 시간
    private float displayDuration = 3f; // 텍스트와 버튼 표시 시간

    private AudioSource audioSource;
    private bool isRecording = false;

    public string serverUrl = "http://a249-125-132-126-243.ngrok-free.app/predict-emotion/"; // Replace with your FastAPI server URL


    // Start is called before the first frame update
    void Start()
    {
        SpeakStartCanvas.SetActive(false);
        SpeakStopCanvas.SetActive(false);
        NextButtonCanvas.SetActive(false);

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

        // 음성을 받아오는 로직 추가
    }

    public void OnSpeakStopButtonClick()
    {
        SpeakStopCanvas.SetActive(false);
        SpeakStartCanvas.SetActive(true);
        speakStartButton.interactable = false;

        StopRecording();
        SaveRecording();

        string filePath = Path.Combine(Application.persistentDataPath, "MyRecording.wav");

        if (File.Exists(filePath))
        {
            StartCoroutine(UploadAudio(filePath));
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

        NextButtonCanvas.SetActive(true);

        //StartCoroutine(SpeakStopSequence());
    }

    IEnumerator UploadAudio(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, "MyRecording.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error uploading file: " + www.error);
            }
            else
            {
                Debug.Log("File uploaded successfully.");
            }
        }
    }

    IEnumerator SpeakStopSequence()
    {

        yield return new WaitForSeconds(3f);
        // 음성 처리 시간 / 음성 처리 로직 추가

        //StartCoroutine(Sequence());
        NextButtonCanvas.SetActive(true);
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

        Directory.CreateDirectory(Path.GetDirectoryName(filepath));

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