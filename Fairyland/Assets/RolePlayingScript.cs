using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Networking;
using System.Text.RegularExpressions;



public class RolePlayingScript : MonoBehaviour
{
    public GameObject whaleObject; // 고래 오브젝트
    public Animator whaleAnimator;
    public Material[] whaleExpressions; // 고래 표정 Material 배열
    public GameObject canvasLine1; // 첫 번째 라인
    public TextMeshProUGUI LineText;

    public GameObject MyLineBox;
    public TextMeshProUGUI MyLineText;

    //public GameObject canvasLine2; // 두 번째 라인
    public Button speakStopButton; // speakStop 버튼
    public Button speakStartButton; // speakStart 버튼
    public float initialDelay = 2f; // 초기 지연 시간
    public float displayDuration = 3f; // 텍스트와 버튼 표시 시간


    //public Button yourButton; // 버튼 참조
    public string sceneToLoad = "3D_book"; // 이동할 씬의 이름
    public string sceneToUnload = "Role_Playing_3d"; // 현재 씬의 이름

    private Dictionary<int, string> interactionDictionary;

    public string title;

    private Book bookClass;
    private NaverTTSforRolePlaying TTSManager;

    private string[] texts;

    public AudioSource audioSourceRolePlaying;

    private bool isFirst = true;

    public void SwitchScene()
    {
        SceneManager.LoadScene("3D_book");
    }

    private int currentPage =0;

    private List<string> youParts;
    private List<string> myParts;

    private int PartCount = 0;

    void LoadCurrentSceneData()
    {
        if (GlobalSceneData.Data.ContainsKey("currentPage"))
        {
            object exampleValue = GlobalSceneData.Data["currentPage"];
            // 저장된 데이터를 복원합니다.
            currentPage = ((int)exampleValue)/2 - 1;
            Debug.Log("Restored value: " + exampleValue);
        }

        youParts = ExtractYouParts(interactionDictionary[currentPage]);
        myParts = ExtractMyParts(interactionDictionary[currentPage]);

        foreach (string part in youParts)
        {
            Debug.Log("You part: " + part + "\n");
        }

        foreach (string part in myParts)
        {
            Debug.Log("My part: " + part + "\n");
        }

    }

    private void Awake()
    {
        // AudioSource가 null이면 새로 생성하고 DontDestroyOnLoad 설정
        if (audioSourceRolePlaying == null)
        {
            GameObject audioSourceObject = new GameObject("AudioSource");
            audioSourceRolePlaying = audioSourceObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(audioSourceObject);
            Debug.Log("audioSource added");
        }

        if (audioSourceRolePlaying == null)
        {
            //audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource component was missing and has been added.");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        PartCount = 0;

        title = PlayerPrefs.GetString("title", "defaultTitle");
        //LoadTextsToPages();


        if (GlobalSceneData.Data.TryGetValue("interactionDictionary", out object interactionTextObj))
        {
            interactionDictionary = interactionTextObj as Dictionary<int, string>;
            //Debug.Log("This is interaction Scene: " + interactionDictionary[currentPage]);
            // Parse and display the [너] parts
            //DisplayYouParts(interactionText);
        }

        LoadCurrentSceneData();


        bookClass = GetComponent<Book>();
        TTSManager = GetComponent<NaverTTSforRolePlaying>();


        //audioSource = GetComponent<AudioSource>();
        //if (audioSource == null)
        //{
        //    audioSource = gameObject.AddComponent<AudioSource>();
        //    Debug.Log("AudioSource component was missing and has been added.");
        //}

        // 초기 상태 설정
        changeExpression(0);
        canvasLine1.SetActive(false);
        //canvasLine2.SetActive(false);
       
        MyLineText.text = myParts[PartCount];
        MyLineBox.SetActive(true);

        speakStopButton.gameObject.SetActive(false);
        speakStartButton.gameObject.SetActive(true);


        // 초기 딜레이 후 첫 번째 상태로 전환
        //StartCoroutine(InitialSequence());


    }


    public static List<string> ExtractYouParts(string text)
    {
        List<string> youParts = new List<string>();

        // 정규 표현식을 사용하여 [너]: 로 시작하는 부분을 추출
        Regex regex = new Regex(@"\[너\]:.*?(?=\n|\r|$)");
        MatchCollection matches = regex.Matches(text);

        foreach (Match match in matches)
        {
            string youPart = match.Value.Replace("[너]:", "").Trim();
            youParts.Add(youPart);
        }

        return youParts;
    }

    public static List<string> ExtractMyParts(string text)
    {
        List<string> youParts = new List<string>();

        // 정규 표현식을 사용하여 [너]: 로 시작하는 부분을 추출
        Regex regex = new Regex(@"\[나\]:.*?(?=\n|\r|$)");
        MatchCollection matches = regex.Matches(text);

        foreach (Match match in matches)
        {
            string youPart = match.Value.Replace("[나]:", "").Trim();
            youParts.Add(youPart);
        }

        return youParts;
    }


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

    //public void PlaySpeech(string path)
    //{
    //    StartCoroutine(LoadAndPlayAudio(path));
    //}

    //public IEnumerator LoadAndPlayAudio(string path)
    //{
    //    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
    //    {
    //        yield return www.SendWebRequest();

    //        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            Debug.LogError(www.error);
    //        }
    //        else
    //        {
    //            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
    //            audioSource.clip = audioClip;
    //            audioSource.Play();
    //        }
    //    }
    //}


    IEnumerator InitialSequence()
    {
        yield return new WaitForSeconds(initialDelay);

        // 첫 번째 라인과 speakStop 버튼 활성화
        changeExpression(0);
        whaleAnimator.SetInteger("NextInt", 4);
        LineText.text = youParts[PartCount];
        canvasLine1.SetActive(true);
        speakStartButton.interactable = false;
        speakStartButton.gameObject.SetActive(true);

        TTSManager.GetAndPlaySpeech("vdain", "Neutral", youParts[PartCount], "RolePlaying");

        PartCount++;

        yield return new WaitForSeconds(displayDuration);

        whaleAnimator.SetInteger("NextInt", 0);
        speakStartButton.interactable = true;
    }

    public void OnSpeakStopButtonClick()
    {
        speakStopButton.gameObject.SetActive(false);
        speakStartButton.gameObject.SetActive(true);

        StartCoroutine(SpeakStopSequence());

    }

    public void OnSpeakStartButtonClick()
    {
        speakStartButton.gameObject.SetActive(false);
        speakStopButton.gameObject.SetActive(true);
        // 음성을 받아오는 로직 추가
        //StartCoroutine(SpeakStopSequence());

    }

    IEnumerator SpeakStartSequence()
    {
        yield return new WaitForSeconds(displayDuration);

        // 두 번째 라인과 두 번째 표정으로 전환
        changeExpression(2);
        canvasLine1.SetActive(false);
        //canvasLine2.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        // speakStop 버튼을 클릭 가능하도록 설정
        speakStopButton.interactable = true;
    }

    IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(displayDuration);
    }

    IEnumerator SpeakStopSequence()
    {
        //다음 표정과 라인 전환
        speakStartButton.interactable = false;

        yield return new WaitForSeconds(2f); // 음성 처리 시간

        canvasLine1.SetActive(true);
        MyLineBox.SetActive(false);
        speakStartButton.gameObject.SetActive(false);
        speakStopButton.gameObject.SetActive(false);

        changeExpression(0);
        whaleAnimator.SetInteger("NextInt", 4);
        
        LineText.text = youParts[PartCount];

        //speakStartButton.interactable = false;
        //speakStartButton.gameObject.SetActive(true);

        TTSManager.GetAndPlaySpeech("vdain", "Neutral", youParts[PartCount], "RolePlaying");

        PartCount++;

        yield return new WaitForSeconds(6.0f);

        whaleAnimator.SetInteger("NextInt", 0);

        if (PartCount >= youParts.Count)
        {
            SwitchScene();
            yield return null;
        }

       
        speakStartButton.interactable = true;
        canvasLine1.SetActive(false);

        MyLineText.text = myParts[PartCount];
        MyLineBox.SetActive(true);

        speakStartButton.gameObject.SetActive(true);
        speakStopButton.gameObject.SetActive(false);


    }

    IEnumerator SecondLineSequence()
    {

        speakStartButton.interactable = false;
        speakStartButton.gameObject.SetActive(true);


        yield return new WaitForSeconds(displayDuration);



    }

    public void changeExpression(int expression)
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            renderer.material = whaleExpressions[expression];
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer not found on the game object.");
        }
    }
}
