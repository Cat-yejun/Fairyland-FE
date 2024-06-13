//The implementation is based on this article:http://rbarraza.com/html5-canvas-pageflip/
//As the rbarraza.com website is not live anymore you can get an archived version from web archive 
//or check an archived version that I uploaded on my website: https://dandarawy.com/html5-canvas-pageflip/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.IO;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Networking;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System;
//using static System.Net.Mime.MediaTypeNames;

public enum FlipMode
{
    RightToLeft,
    LeftToRight
}


[System.Serializable]
public class NovelSceneData
{
    public int Id;
    public string Text;
    public List<string> Emotions = new List<string>();
    public List<string> Guides = new List<string>();
    public List<string> Interactions = new List<string>();
}

[System.Serializable]
public class InteractionData
{
    public List<List<string>> emotion;
    public List<List<string>> guide;
    public List<List<string>> interaction;
}


[ExecuteInEditMode]

public static class GlobalSceneData
{
    public static Dictionary<string, object> Data = new Dictionary<string, object>();
}

public class Book : MonoBehaviour {
    public Canvas canvas;
    [SerializeField]
    RectTransform BookPanel;
    public Sprite background;
    public Sprite[] bookPages;
    //public Sprite[] bookBigPages;
    public List<Sprite> bookBigPages = new List<Sprite>();
    public bool interactable=true;
    public bool enableShadowEffect=true;
    //represent the index of the sprite shown in the right page
    public int currentPage = 0;

    public int TotalPageCount
    {
        get { return bookPages.Length; }
    }
    public Vector3 EndBottomLeft
    {
        get { return ebl; }
    }
    public Vector3 EndBottomRight
    {
        get { return ebr; }
    }
    public float Height
    {
        get
        {
            return BookPanel.rect.height ; 
        }
    }
    public Image ClippingPlane;
    public Image NextPageClip;
    public Image Shadow;
    public Image ShadowLTR;
    public Image Left;
    public Image LeftNext;
    public Image Right;
    public Image RightNext;
    public UnityEvent OnFlip;
    float radius1, radius2;
    //Spine Bottom
    Vector3 sb;
    //Spine Top
    Vector3 st;
    //corner of the page
    Vector3 c;
    //Edge Bottom Right
    Vector3 ebr;
    //Edge Bottom Left
    Vector3 ebl;
    //follow point 
    Vector3 f;
    bool pageDragging = false;
    //current flip mode
    FlipMode mode;

    //public Transform[] pagePositions; // 페이지 위치를 정의하는 트랜스폼 배열
    //public TMP_FontAsset fontAsset; // 원하는 글꼴
    //public GameObject[] textGameObjects; // 각 페이지의 텍스트를 담는 GameObject 배열

    public TMP_FontAsset fontAsset; // 원하는 글꼴
    private string[] texts;

    public TextMeshProUGUI textObjectLeft;
    public TextMeshProUGUI textObjectRight;
    public TextMeshProUGUI LineGuessingText;

    public GameObject StoryCanvasLeft;
    public GameObject StoryCanvasRight;
    public GameObject LineButtonCanvas;
    public GameObject GotoInteractionCanvas;
    public GameObject GotoEmotionCanvas;

    public RectTransform LeftTextbox;
    public RectTransform RightTextbox;
    public RectTransform LineGuessing;
    public RectTransform buttonRectTransform;

    public UnityEngine.UI.Button lineButton;
    public bool firstButtonPress = false;
    private bool pressAllowed = true;
    private bool SpeakStartStopButton = true;

    public GameObject SpeakStartButton;
    public GameObject SpeakStopButton;
    public GameObject AskLineGuessCanvas;
    public GameObject AskEmotionCanvas;

    private int LineGuessingPage = 8;
    private int InteractionPage = 11;


    private Vector2 originalSize;
    private Vector2 originalPosition;
    private float originalFontSize;
    private Vector2 originalTextboxSize;
    private Vector2 originalButtonSize;
    private Vector2 originalButtonPosition;


    public AudioSource audioSource;

    public UnityEngine.UI.Button yourButton; // 버튼 참조
    public string sceneToLoad = "Role_Playing_3d"; // 이동할 씬의 이름
    public string sceneToUnload = "3D_book"; // 현재 씬의 이름


    private EmotionSelectScript emotionSelection;
    private NaverTTSManager TTSManager;


    public string title;
    public string imgPath;
    public string textPath;
    public string interactionPath;
    public List<NovelSceneData> scenes = new List<NovelSceneData>();
    public Dictionary<int, int> novelDictNumberToSceneIndex = new Dictionary<int, int>();
    private Dictionary<int, string> guideDictionary = new Dictionary<int, string>();
    private Dictionary<int, string> interactionDictionary = new Dictionary<int, string>();
    private Dictionary<int, string> emotionDictionary = new Dictionary<int, string>();
    private Dictionary<int, int> emotionIntDictionary = new Dictionary<int, int>();
    private Dictionary<int, string> novelDictNumberToText = new Dictionary<int, string>(); // 추가된 데이터 구조
    private Dictionary<int, string> sceneLineGuessAnswer = new Dictionary<int, string>();

    private List<int> interactionPages;
    private List<int> emotionChoicePages;

    public string guideText;
    public string emotionIs;
    public int emotionInteger;
    public string LineGuessAnswer;

    //public RectTransform canvasRect; // The RectTransform of the Canvas

    public GameObject EmotionSelectCanvas;
    public GameObject CorrectCanvas;
    public GameObject WrongCanvas;
    public GameObject HappyCanvas;
    public GameObject AngryCanvas;
    public GameObject SadCanvas;
    public GameObject FearCanvas;
    public GameObject SurpriseCanvas;
    public GameObject CalmCanvas;



    public void SwitchScene()
    {
        SaveCurrentSceneData();
        SceneManager.LoadScene(sceneToLoad);
    }

    public void GoToHomeMenu()
    {
        BackGroundMusicBook.Instance.StopMusic();
        GlobalSceneData.Data["currentPage"] = 0;
        SceneManager.LoadScene("New_Book_manu");
    }

    void SaveCurrentSceneData()
    {
        // 예제 데이터 저장
        GlobalSceneData.Data["currentPage"] = currentPage;
        GlobalSceneData.Data["interactionDictionary"] = interactionDictionary;
        // 필요한 다른 데이터도 저장합니다.
    }

    void LoadCurrentSceneData()
    {
        if (GlobalSceneData.Data.ContainsKey("currentPage"))
        {
            object exampleValue = GlobalSceneData.Data["currentPage"];
            // 저장된 데이터를 복원합니다.
            currentPage = (int)exampleValue;
            Debug.Log("Restored value: " + exampleValue);
        }
    }


    private void Awake()
    {
        // AudioSource가 null이면 새로 생성하고 DontDestroyOnLoad 설정
        if (audioSource == null)
        {
            GameObject audioSourceObject = new GameObject("AudioSource");
            audioSource = audioSourceObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(audioSourceObject);
            Debug.Log("auidioSource added");
        }

        if (audioSource == null)
        {
            //audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource component was missing and has been added.");
        }
    }



    void Start()
    {
        string BGMPath = Path.Combine(Application.persistentDataPath, "BookBGM.mp3");
        StartCoroutine(BackGroundMusicBook.Instance.LoadBackgroundMusic(BGMPath));

        //currentPage = 0;
        title = PlayerPrefs.GetString("title", "defaultTitle");
        Debug.Log("book's title is : " + title);

        imgPath = Path.Combine(Application.persistentDataPath, "SaveFile", title, "img");
        textPath = Path.Combine(Application.persistentDataPath, "SaveFile", title, title + ".json");
        interactionPath = Path.Combine(Application.persistentDataPath, "SaveFile", title, "interaction.json");

        Debug.Log(imgPath);
        Debug.Log(textPath);
        Debug.Log(interactionPath);

        LoadCurrentSceneData();

        emotionSelection = GetComponent<EmotionSelectScript>();
        TTSManager = GetComponent<NaverTTSManager>();

        if (!canvas) canvas=GetComponentInParent<Canvas>();
        if (!canvas) Debug.LogError("Book should be a child to canvas");

        // Call SplitImage to divide the image and set up pages
        //if (bookBigPages != null)
        //{
        //    InitializeBookPages();
        //}
        //else
        //{
        //    Debug.LogError("fullImageSprite not set");
        //}

        LineGuessing = LeftTextbox;
        LineGuessingText = textObjectLeft;
        originalSize = LineGuessing.sizeDelta;
        originalPosition = LineGuessing.anchoredPosition;
        originalButtonPosition = buttonRectTransform.anchoredPosition;
        originalFontSize = LineGuessingText.fontSize;
        originalTextboxSize = LineGuessingText.rectTransform.sizeDelta;
        originalButtonSize = buttonRectTransform.sizeDelta;
        

        bookBigPages = new List<Sprite>();

        StartCoroutine(LoadImages());
        StartCoroutine(LoadTexts());
        StartCoroutine(LoadInteractions());

        interactionPages = GetPagesWithInteractions();
        emotionChoicePages = GetPagesWithEmotionChoices();

        GotoEmotionCanvas.SetActive(false);
        GotoInteractionCanvas.SetActive(false);
        SpeakStartButton.SetActive(false);
        SpeakStopButton.SetActive(false);


        //audioSource = GetComponent<AudioSource>();
        //if (audioSource == null)
        //{
        //    audioSource = gameObject.AddComponent<AudioSource>();
        //    Debug.Log("AudioSource component was missing and has been added.");
        //}

        //EnsureSingleAudioListener();



        //LoadTextsToPages();

        UpdateTextVisibility(); // 페이지를 넘길 때마다 텍스트 업데이트

        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        UpdateSprites();
        CalcCurlCriticalPoints();
        
        float pageWidth = BookPanel.rect.width / 2.0f;
        float pageHeight = BookPanel.rect.height;
        NextPageClip.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageHeight * 2);


        ClippingPlane.rectTransform.sizeDelta = new Vector2(pageWidth * 2 + pageHeight, pageHeight + pageHeight * 2);

        //hypotenous (diagonal) page length
        float hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        float shadowPageHeight = pageWidth / 2 + hyp;

        Shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        Shadow.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);

        ShadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        ShadowLTR.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);

    }



   
    private Vector2 GetCharacterPosition(TextGenerator textGen, int charIndex, Text textComponent)
    {
        IList<UIVertex> verts = textGen.verts;
        Vector3 position = verts[charIndex * 4].position;
        Vector2 localPosition = new Vector2(position.x, position.y);
        return textComponent.rectTransform.TransformPoint(localPosition);
    }


    private void PositionButtonOverText(TextMeshProUGUI textComponent, string targetWord, RectTransform buttonRectTransform)
    {
        string text = textComponent.text;
        int startIndex = text.IndexOf(targetWord);

        if (startIndex == -1)
        {
            Debug.LogError("Target word not found in text.");
            return;
        }

        // Get the TMP_TextInfo for the text component
        TMP_TextInfo textInfo = textComponent.textInfo;
        textComponent.ForceMeshUpdate();  // Ensure the text mesh is up to date

        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;
        bool startFound = false;
        bool endFound = false;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (charInfo.index == startIndex && !startFound)
            {
                startPosition = charInfo.bottomLeft;
                startFound = true;
            }

            if (charInfo.index == startIndex + targetWord.Length - 1 && !endFound)
            {
                endPosition = charInfo.topRight;
                endFound = true;
            }

            if (startFound && endFound)
            {
                break;
            }
        }

        if (!startFound || !endFound)
        {
            Debug.LogError("Failed to find the positions for the target word.");
            return;
        }

        // Convert the positions to world space
        RectTransform textRectTransform = textComponent.GetComponent<RectTransform>();
        Vector3 worldStartPosition = textRectTransform.TransformPoint(startPosition);
        Vector3 worldEndPosition = textRectTransform.TransformPoint(endPosition);

        // Calculate the button size and position
        float textWidth = Vector3.Distance(worldStartPosition, worldEndPosition);
        float buttonHeight = textComponent.fontSize * 1.5f;  // Adjust the button height as needed
        Vector2 buttonSize = new Vector2(textWidth*3, buttonHeight);
        Vector2 buttonPosition = (worldStartPosition + worldEndPosition) / 2;

        // Convert the position to screen point
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, buttonPosition);

        // Convert the screen point to local point in the Button's parent RectTransform
        RectTransform parentRectTransform = buttonRectTransform.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPoint, null, out Vector2 localPoint);

        // Set the button's position and size
        buttonRectTransform.sizeDelta = buttonSize;
        buttonRectTransform.anchoredPosition = localPoint;

        // Adjust the scale
        buttonRectTransform.localScale = new Vector3(buttonRectTransform.localScale.x, 1, buttonRectTransform.localScale.z);

        Debug.Log("Button Position: " + buttonRectTransform.anchoredPosition);
        Debug.Log("Button Size: " + buttonRectTransform.sizeDelta);
    }


    public void OnPressReadStoryButton()
    {
        TTSManager.GetAndPlaySpeech("vdain", "Neutral", texts[currentPage / 2 - 1], "ReadText");
    }

    public List<int> GetPagesWithInteractions()
    {
        List<int> pages = new List<int>();
        var keys = new List<int>(interactionDictionary.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            if (interactionDictionary[keys[i]] != null && interactionDictionary.Count > 0)
            {
                pages.Add(keys[i] + 1); // 페이지 번호는 1부터 시작
            }
        }
        return pages;
    }


    public List<int> GetPagesWithEmotionChoices()
    {
        List<int> pages = new List<int>();
        var keys = new List<int>(interactionDictionary.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            if (guideDictionary[keys[i]] != null && guideDictionary.Count > 0)
            {
                pages.Add(keys[i] + 1); // 페이지 번호는 1부터 시작
            }
        }
        return pages;
    }


    IEnumerator LoadInteractions()
    {
        string filePath = interactionPath;

        string[] emotionArray = { "Calm", "Happy", "Sad", "Angry", "Fear", "Surprised", "main" };

        string filePath1 = textPath;

  


        if (File.Exists(filePath) && File.Exists(filePath1))
        {
            string jsonContent = File.ReadAllText(filePath);
            var jsonData = JsonConvert.DeserializeObject<InteractionData>(jsonContent);

            string jsonContent1 = File.ReadAllText(filePath1);
            var jsonData1 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent1);


            for (int i = 0; i < jsonData.guide.Count; i++)
            {
                var guides = jsonData.guide[i];
                var emotions = jsonData.emotion[i];

                for (int j = 0; j < guides.Count; j++)
                {
                    if (!string.IsNullOrWhiteSpace(guides[j]))
                    {
                        int novelDictNumber;
                        if (int.TryParse(emotions[j].Trim(), out novelDictNumber))
                        {
                            if (novelDictNumberToSceneIndex.ContainsKey(novelDictNumber))
                            {
                                int sceneIndex = novelDictNumberToSceneIndex[novelDictNumber];
                                if (!guideDictionary.ContainsKey(sceneIndex))
                                {
                                        
                                    if (jsonData1.ContainsKey("novel_num_dict"))
                                    {

                                        var novelNumDict = jsonData1["novel_num_dict"];
                                        var novelKeys = new List<string>(novelNumDict.Keys);

                                        sceneLineGuessAnswer[sceneIndex] = novelNumDict[novelKeys[novelDictNumber-1]];
                                    }
                                    
                                    guideDictionary[sceneIndex] = guides[j];
                                    emotionDictionary[sceneIndex] = emotionArray[i];
                                    emotionIntDictionary[sceneIndex] = i;
                                }
                                //else
                                //{
                                //    guideDictionary[sceneIndex] += "\n" + guides[j]; // 여러 가이드를 합칠 때 줄바꿈 추가
                                //}

                            }
                        }
                    }
                }

            }

            for (int i = 0; i < jsonData.interaction.Count; i++)
            {
                var interactions = jsonData.interaction[i];
                var emotions = jsonData.emotion[i];

                for (int j = 0; j < interactions.Count; j++)
                {
                    if (!string.IsNullOrWhiteSpace(interactions[j]))
                    {
                        int novelDictNumber;
                        if (int.TryParse(emotions[j].Trim(), out novelDictNumber))
                        {
                            if (novelDictNumberToSceneIndex.ContainsKey(novelDictNumber))
                            {
                                int sceneIndex = novelDictNumberToSceneIndex[novelDictNumber];
                                if (!interactionDictionary.ContainsKey(sceneIndex))
                                {
                                    interactionDictionary[sceneIndex] = interactions[j];
                                }
                                
                            }
                        }
                    }
                }

            }

            Debug.Log("Interactions loaded successfully.");
            Debug.Log("Pages with Interactions: " + string.Join(", ", GetPagesWithInteractions()));
            Debug.Log("Pages with Emotion Choices: " + string.Join(", ", GetPagesWithEmotionChoices()));
            PrintGuideDictionary();

        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

        yield return null;
    }

    private void PrintGuideDictionary()
    {
        foreach (var kvp in guideDictionary)
        {
            Debug.Log($"Novel Dict Number: {kvp.Key}, Guide: {kvp.Value}, Emotion: {emotionDictionary[kvp.Key]}, Answer: {sceneLineGuessAnswer[kvp.Key]}");
        }
    }

    void EnsureSingleAudioListener()
    {
        AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();

        // If there is more than one AudioListener, disable the extras
        if (audioListeners.Length > 1)
        {
            for (int i = 1; i < audioListeners.Length; i++)
            {
                audioListeners[i].enabled = false;
            }
            Debug.LogWarning("Multiple AudioListeners found. Disabled extra AudioListeners to ensure there is only one.");
        }
        else if (audioListeners.Length == 0)
        {
            // If no AudioListener is found, add one to the main camera
            Camera.main.gameObject.AddComponent<AudioListener>();
            Debug.LogWarning("No AudioListener found. Added an AudioListener to the main camera.");
        }
    }

    IEnumerator LoadImages()
    {
        //string folderPath = Path.Combine(Application.persistentDataPath, "SaveFile/img");
        string folderPath = imgPath;

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Directory not found: " + folderPath);
            yield break;
        }

        // 파일 이름을 기준으로 정렬
        string[] filePaths = Directory.GetFiles(folderPath, "*.png")
                    .OrderBy(f => ExtractNumber(Path.GetFileNameWithoutExtension(f)))
                    .ToArray();

        Debug.Log(filePaths);

        foreach (string filePath in filePaths)
        {
            yield return StartCoroutine(LoadImage(filePath));
        }

        InitializeBookPages();
        UpdateSprites();
    }

    int ExtractNumber(string fileName)
    {
        // 숫자를 추출하여 정렬에 사용할 수 있도록 처리
        // 예: "1-0"에서 "1"을 추출
        string numberPart = fileName.Split('-')[0];
        int number;
        if (int.TryParse(numberPart, out number))
        {
            return number;
        }
        return int.MaxValue; // 숫자가 없으면 큰 값을 반환하여 뒤로 가도록 함
    }


    IEnumerator LoadImage(string filePath)
    {
        string fileUrl = "file://" + filePath;

        using (WWW www = new WWW(fileUrl))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D texture = www.texture;
                Sprite sprite = TextureToSprite(texture);
                bookBigPages.Add(sprite);
                Debug.Log("Loaded image: " + filePath);
            }
            else
            {
                Debug.LogError("Error loading image: " + www.error);
            }
        }
    }

    Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }


    public void OnPressLineGuessing()
    {
        Debug.Log("line button is pressed1");

        if (!firstButtonPress)
        {
            Debug.Log("line button is pressed2");

            if (pressAllowed == true && SpeakStartStopButton == true)
            {
                Debug.Log("line button is pressed3");
                firstButtonPress = true;
                pressAllowed = false;
                StartCoroutine(EnlargeAndCenterImage());
                emotionSelection.OnGotoEmotionButtonPress();
                //StartCoroutine(InitialSequence());
                pressAllowed = true;
            }

        }
    }

    public void StartGotoOriginalPos()
    {
        AskEmotionCanvas.SetActive(false);
        StartCoroutine(GotoOriginalPosition());
    }

    private void GotoOriginalAtOnce()
    {
        Vector2 currentSize = LineGuessing.sizeDelta;
        Vector2 currentPosition = LineGuessing.anchoredPosition;
        Vector2 currentButtonPosition = buttonRectTransform.anchoredPosition;

        Vector2 targetPosition = originalPosition;
        //Vector2 targetButtonPosition = originalButtonPosition;
        Vector2 targetSize = originalSize;

        float currentFontSize = LineGuessingText.fontSize;
        float targetFontSize = originalFontSize;

        Vector2 currentTextboxSize = LineGuessingText.rectTransform.sizeDelta;
        Vector2 targetTextboxSize = originalTextboxSize;

        Vector2 currentButtonSize = buttonRectTransform.sizeDelta;
        //Vector2 targetButtonSize = originalButtonSize;


        LineGuessing.sizeDelta = targetSize;
        LineGuessing.anchoredPosition = targetPosition;
        LineGuessingText.fontSize = (int)targetFontSize;
        //buttonRectTransform.anchoredPosition = targetButtonPosition;
        //buttonRectTransform.sizeDelta = targetButtonSize;

    }

    private IEnumerator GotoOriginalPosition()
    {
        Vector2 currentSize = LineGuessing.sizeDelta;
        Vector2 currentPosition = LineGuessing.anchoredPosition;
        Vector2 currentButtonPosition = buttonRectTransform.anchoredPosition;

        Vector2 targetPosition = originalPosition;
        Vector2 targetButtonPosition = originalButtonPosition;
        Vector2 targetSize = originalSize;

        float currentFontSize = LineGuessingText.fontSize;
        float targetFontSize = originalFontSize;

        Vector2 currentTextboxSize = LineGuessingText.rectTransform.sizeDelta;
        Vector2 targetTextboxSize = originalTextboxSize;

        Vector2 currentButtonSize = buttonRectTransform.sizeDelta;
        Vector2 targetButtonSize = originalButtonSize;

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            LineGuessing.sizeDelta = Vector2.Lerp(currentSize, targetSize, t);
            LineGuessing.anchoredPosition = Vector2.Lerp(currentPosition, targetPosition, t);
            LineGuessingText.fontSize = (int)Mathf.Lerp(currentFontSize, targetFontSize, t);
            LineGuessingText.rectTransform.sizeDelta = Vector2.Lerp(currentTextboxSize, targetTextboxSize, t);
            buttonRectTransform.anchoredPosition = Vector2.Lerp(currentButtonPosition, targetButtonPosition, t);
            buttonRectTransform.sizeDelta = Vector2.Lerp(currentButtonSize, targetButtonSize, t);

            yield return null;
        }

        LineGuessing.sizeDelta = targetSize;
        LineGuessing.anchoredPosition = targetPosition;
        LineGuessingText.fontSize = (int)targetFontSize;
        buttonRectTransform.anchoredPosition = targetButtonPosition;
        buttonRectTransform.sizeDelta = targetButtonSize;
    
    }

    public void WhaleSpeak()
    {
        SpeakStartButton.SetActive(true);
        AskLineGuessCanvas.SetActive(true);

        string filePath = Path.Combine(Application.persistentDataPath, "tts.mp3");

        PlaySpeech(filePath);
    }


    IEnumerator EnlargeAndCenterImage()
    {
        originalSize = LineGuessing.sizeDelta;
        Vector2 enlargedSize = new Vector2((float)(LineGuessing.sizeDelta.x * 1.5), (float)(LineGuessing.sizeDelta.y * 2.0));

        originalPosition = LineGuessing.anchoredPosition;
        originalButtonPosition = buttonRectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(0.0f, -100.0f);

        originalFontSize = LineGuessingText.fontSize;
        float enlargedFontSize = (float)(originalFontSize * 1.5);

        originalTextboxSize = LineGuessingText.rectTransform.sizeDelta;
        Vector2 enlargedTextboxSize = new Vector2((float)(originalTextboxSize.x * 1.5), (float)(originalTextboxSize.y * 3.0));

        originalButtonSize = buttonRectTransform.sizeDelta;
        //Vector2 enlargedButtonSize = new Vector2((float)(buttonRectTransform.sizeDelta.x * 1.8), (float)(buttonRectTransform.sizeDelta.y * 1.4));

        RectTransform tempLineGuessing = LineGuessing;
        TextMeshProUGUI tempLineGuessingText = LineGuessingText;

        tempLineGuessing.sizeDelta = enlargedSize;
        tempLineGuessing.anchoredPosition = targetPosition;
        tempLineGuessingText.fontSize = (int)enlargedFontSize;

        RectTransform temp = buttonRectTransform;
        PositionButtonOverText(tempLineGuessingText, LineGuessAnswer, temp);

        Vector2 enlargedButtonSize = new Vector2(temp.sizeDelta.x, temp.sizeDelta.y);
        Vector2 buttonTargetPosition = new Vector2(temp.anchoredPosition.x, temp.anchoredPosition.y);

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            LineGuessing.sizeDelta = Vector2.Lerp(originalSize, enlargedSize, t);
            LineGuessing.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, t);
            LineGuessingText.fontSize = (int)Mathf.Lerp(originalFontSize, enlargedFontSize, t);
            LineGuessingText.rectTransform.sizeDelta = Vector2.Lerp(originalTextboxSize, enlargedTextboxSize, t);
            buttonRectTransform.anchoredPosition = Vector2.Lerp(originalButtonPosition, buttonTargetPosition, t);
            buttonRectTransform.sizeDelta = Vector2.Lerp(originalButtonSize, enlargedButtonSize, t);

            yield return null;
        }

        LineGuessing.sizeDelta = enlargedSize;
        LineGuessing.anchoredPosition = targetPosition;
        LineGuessingText.fontSize = (int)enlargedFontSize;
        buttonRectTransform.anchoredPosition = buttonTargetPosition;
        buttonRectTransform.sizeDelta = enlargedButtonSize;

        yield return new WaitForSeconds(2.0f);

        //SpeakStartButton.SetActive(true);
        //AskLineGuessCanvas.SetActive(true);

        //string filePath = Path.Combine(Application.persistentDataPath, "tts.mp3");

        //PlaySpeech(filePath);

    }

    public void PlaySpeech(string path)
    {
        StartCoroutine(LoadAndPlayAudio(path));
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
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }


    public string GetGuideForPage(int sceneIndex)
    {
        if (guideDictionary.ContainsKey(sceneIndex))
        {
            return guideDictionary[sceneIndex];
        }
        return string.Empty;
    }

    public string GetEmotionForPage(int sceneIndex)
    {
        if (emotionDictionary.ContainsKey(sceneIndex))
        {
            return emotionDictionary[sceneIndex];
        }
        return string.Empty;
    }

    public int GetEmotionIntegerPage(int sceneIndex)
    {
        if (emotionIntDictionary.ContainsKey(sceneIndex))
        {
            return emotionIntDictionary[sceneIndex];
        }
        return 0;
    }

    public string GetLineGuessAnswer(int sceneIndex)
    {
        if (sceneLineGuessAnswer.ContainsKey(sceneIndex))
        {
            string answer = sceneLineGuessAnswer[sceneIndex];

            int firstQuoteIndex = answer.IndexOf('"');
            int secondQuoteIndex = answer.IndexOf('"', firstQuoteIndex + 1);

            if (firstQuoteIndex != -1 && secondQuoteIndex != -1)
            {
                return answer.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);
            }

            return answer;
        }
        return string.Empty;
    }

    void UpdateTextVisibility()
    {
        Debug.Log("current Page is : " + currentPage);

        GotoEmotionCanvas.SetActive(false);
        GotoInteractionCanvas.SetActive(false);
        SpeakStartButton.SetActive(false);
        SpeakStopButton.SetActive(false);
        LineButtonCanvas.SetActive(false);
        StoryCanvasLeft.SetActive(false);
        StoryCanvasRight.SetActive(false);
        AskEmotionCanvas.SetActive(false);
        AskLineGuessCanvas.SetActive(false);
        EmotionSelectCanvas.SetActive(false);
        HappyCanvas.SetActive(false);
        AngryCanvas.SetActive(false);
        SurpriseCanvas.SetActive(false);
        SadCanvas.SetActive(false);
        FearCanvas.SetActive(false);
        CalmCanvas.SetActive(false);
        WrongCanvas.SetActive(false);
        CorrectCanvas.SetActive(false);


        if (currentPage == 0 || currentPage / 2 > texts.Length)
        {
            StoryCanvasLeft.SetActive(false);
            StoryCanvasRight.SetActive(false);
            LineButtonCanvas.SetActive(false);
        }
        else
        {

            if (currentPage / 2 <= texts.Length)
            {
                bool isLeftPage = (currentPage / 2) % 2 == 0;
                int pageIndex = currentPage / 2 - 1;

                if (isLeftPage)
                {
                    StoryCanvasLeft.SetActive(true);
                    //StoryCanvasRight.SetActive(false);

                    textObjectLeft.text = texts[pageIndex];
                    textObjectLeft.font = fontAsset;
                    textObjectLeft.alignment = TextAlignmentOptions.Center;


                    guideText = GetGuideForPage(pageIndex);
                    emotionIs = GetEmotionForPage(pageIndex);
                    emotionInteger = GetEmotionIntegerPage(pageIndex);
                    LineGuessAnswer = GetLineGuessAnswer(pageIndex);

                    // If Scene is interaction scene

                    if (interactionDictionary.ContainsKey(currentPage/2 - 1))
                    {
                        GotoInteractionCanvas.SetActive(true);
                        Debug.Log("This is interaction page");
                    }
                    else
                    {
                        GotoInteractionCanvas.SetActive(false);
                    }

                    // If Scene is emotion selection scene

                    if (guideDictionary.ContainsKey(currentPage/2 - 1))
                    {
                        Debug.Log("This is emotion selection page");
                        LineButtonCanvas.SetActive(true);
                        LineGuessing = LeftTextbox;
                        LineGuessingText = textObjectLeft;
                        PositionButtonOverText(LineGuessingText, LineGuessAnswer, buttonRectTransform);
                        Debug.Log("position is : " + buttonRectTransform.anchoredPosition);
                        //LineGuessing.anchoredPosition = Vector3.zero;

                    }
                    else
                    {
                        LineButtonCanvas.SetActive(false);
                        GotoEmotionCanvas.SetActive(false);
                        GotoInteractionCanvas.SetActive(false);
                        SpeakStartButton.SetActive(false);
                        SpeakStopButton.SetActive(false);
                    }

                }
                else
                {
                    //StoryCanvasLeft.SetActive(false);
                    StoryCanvasRight.SetActive(true);

                    textObjectRight.text = texts[currentPage / 2 - 1];
                    textObjectRight.font = fontAsset;
                    textObjectRight.alignment = TextAlignmentOptions.Center;


                    guideText = GetGuideForPage(pageIndex);
                    emotionIs = GetEmotionForPage(pageIndex);
                    emotionInteger = GetEmotionIntegerPage(pageIndex);
                    LineGuessAnswer = GetLineGuessAnswer(pageIndex);


                    // If Scene is Interaction scene                  

                    if (interactionDictionary.ContainsKey(currentPage / 2 - 1))
                    {
                        GotoInteractionCanvas.SetActive(true);
                    }
                    else
                    {
                        GotoInteractionCanvas.SetActive(false);
                    }

                    // If Scene is Emotion Seletion Scene

                    if (guideDictionary.ContainsKey(currentPage/2 - 1))
                    {
                        LineButtonCanvas.SetActive(true);
                        LineGuessing = RightTextbox;
                        LineGuessingText = textObjectRight;
                        PositionButtonOverText(LineGuessingText, LineGuessAnswer, buttonRectTransform);
                        Debug.Log("position is : " + buttonRectTransform.anchoredPosition);
                        //LineGuessing.anchoredPosition = Vector3.zero;

                    }
                    else
                    {
                        GotoEmotionCanvas.SetActive(false);
                        GotoInteractionCanvas.SetActive(false);
                        SpeakStartButton.SetActive(false);
                        SpeakStopButton.SetActive(false);
                        LineButtonCanvas.SetActive(false);
                    }
                }

            }
            else
            {
                textObjectLeft.text = "";
                textObjectRight.text = "";
            }
        }

        
    }


    void LoadTextsToPages()
    {
        string[] filePaths = Directory.GetFiles(Path.Combine(Application.dataPath, "ResourceTexts"), "*.txt");
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


    void InitializeBookPages()
    {
        // Assume each big page contains exactly two pages
        bookPages = new Sprite[(bookBigPages.Count + 1) * 2];

        for (int i = 0, j = 0; i < bookBigPages.Count; i++, j += 2)
        {
            Texture2D originalTexture = bookBigPages[i].texture;
            Rect originalRect = bookBigPages[i].rect;

            // Left page
            Rect leftRect = new Rect(originalRect.x, originalRect.y, originalRect.width / 2, originalRect.height);
            bookPages[j + 1] = Sprite.Create(originalTexture, leftRect, new Vector2(0.5f, 0.5f), bookBigPages[i].pixelsPerUnit);

            // Right page
            Rect rightRect = new Rect(originalRect.x + originalRect.width / 2, originalRect.y, originalRect.width / 2, originalRect.height);
            bookPages[j + 2] = Sprite.Create(originalTexture, rightRect, new Vector2(0.5f, 0.5f), bookBigPages[i].pixelsPerUnit);
        }

        Debug.Log("Initialized book pages.");
    }

    

    IEnumerator LoadTexts()
    {
        //string filePath = Path.Combine(Application.persistentDataPath, "SaveFile", "소나기123.json");
        string filePath = textPath;

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);

            if (jsonData.ContainsKey("splited_novel_dict") && jsonData.ContainsKey("novel_num_dict"))
            {
                var splitedNovelDict = jsonData["splited_novel_dict"];
                var novelNumDict = jsonData["novel_num_dict"];
                texts = new string[splitedNovelDict.Count];

                int index = 0;
                int novelIndex = 0;
                int prevNovelIndex = 0;
                var novelKeys = new List<string>(novelNumDict.Keys);


                foreach (var entry in splitedNovelDict)
                {
                    bool foundMatch = false;

                    for (; novelIndex < novelKeys.Count; novelIndex++)
                    {
                        var novelEntry = novelNumDict[novelKeys[novelIndex]];
                        string splitedText = Regex.Replace(entry.Value.Trim(), @"\s+", "");
                        string novelText = Regex.Replace(novelEntry.Trim(), @"\s+", "");

                        if (splitedText.Contains(novelText))
                        {
                            int novelDictNumber;
                            if (int.TryParse(novelKeys[novelIndex], out novelDictNumber))
                            {
                                novelDictNumberToSceneIndex[novelDictNumber] = index;
                                foundMatch = true;
                                //Debug.Log($"Matched: {novelDictNumber} -> {entry.Value}");

                                for (int i = prevNovelIndex; i < novelIndex; i++)
                                {
                                    int prevDictNumber;
                                    if (int.TryParse(novelKeys[i], out prevDictNumber))
                                    {
                                        novelDictNumberToSceneIndex[prevDictNumber] = index - 1;
                                    }
                                }
                                prevNovelIndex = novelIndex + 1;
                                novelIndex++;
                                break;
                            }
                        }
                    }


                    NovelSceneData scene = new NovelSceneData
                    {
                        Id = index,
                        Text = entry.Value
                    };
                    scenes.Add(scene);

                    string formattedText = FormatText(entry.Value);
                    texts[index] = formattedText;
                    index++;
                }

                // Handle remaining novel entries
                for (int i = prevNovelIndex; i < novelKeys.Count; i++)
                {
                    int prevDictNumber;
                    if (int.TryParse(novelKeys[i], out prevDictNumber))
                    {
                        novelDictNumberToSceneIndex[prevDictNumber] = index - 1;
                    }
                }


                Debug.Log("Texts loaded successfully.");
                Debug.Log("TextFile Length : " + texts.Length);

                //foreach(var text in novelDictNumberToSceneIndex)
                //{
                //    Debug.Log($"Novel Dict Number: {text.Key}, Scene Index: {text.Value}");
                //}
            }
            else
            {
                Debug.LogError("splited_novel_dict not found in JSON.");
            }
           
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

        yield return null;
    }

    private int ExtractNovelDictNumber(string text)
    {
        // Extract novel_dict_number from text (example implementation)
        int startIndex = text.IndexOf("[") + 1;
        int endIndex = text.IndexOf("]");
        if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
        {
            string numberStr = text.Substring(startIndex, endIndex - startIndex);
            if (int.TryParse(numberStr, out int result))
            {
                return result;
            }
        }
        Debug.LogWarning("novel_dict_number marker not found in text: " + text);
        return -1;
    }

    string FormatText(string input)
    {
        // 큰따옴표 안과 밖을 구분하여 처리하기 위한 플래그
        bool insideQuotes = false;
        char[] inputArray = input.ToCharArray();
        List<char> outputList = new List<char>();

        for (int i = 0; i < inputArray.Length; i++)
        {
            if (inputArray[i] == '\"')
            {
                insideQuotes = !insideQuotes;
                outputList.Add(inputArray[i]);
            }
            else if (!insideQuotes && inputArray[i] == '.' && i + 1 < inputArray.Length && inputArray[i + 1] == ' ')
            {
                outputList.Add(inputArray[i]);
                outputList.Add('\n');
            }
            else
            {
                outputList.Add(inputArray[i]);
            }
        }

        // 리스트를 문자열로 변환
        string formattedText = new string(outputList.ToArray());

        // 불필요한 연속 줄바꿈을 제거합니다.
        formattedText = Regex.Replace(formattedText, "\n+", "\n");

        // 마지막 줄바꿈을 제거합니다.
        formattedText = formattedText.TrimEnd('\n');

        return formattedText;
    }

    void SplitSprite(Sprite originalSprite)
    {
        Texture2D originalTexture = originalSprite.texture;
        Rect originalRect = originalSprite.rect;

        // Calculate left and right rect
        Rect leftRect = new Rect(originalRect.x, originalRect.y, originalRect.width / 2, originalRect.height);
        Rect rightRect = new Rect(originalRect.x + originalRect.width / 2, originalRect.y, originalRect.width / 2, originalRect.height);

        // Create new sprites
        Sprite leftSprite = Sprite.Create(originalTexture, leftRect, new Vector2(0.5f, 0.5f), originalSprite.pixelsPerUnit);
        Sprite rightSprite = Sprite.Create(originalTexture, rightRect, new Vector2(0.5f, 0.5f), originalSprite.pixelsPerUnit);

        bookPages[1] = leftSprite;
        bookPages[2] = rightSprite;

        // Assign sprites to images
        //Left.sprite = leftSprite;
        //Right.sprite = rightSprite;
        
    }

    private void CalcCurlCriticalPoints()
    {
        sb = new Vector3(0, -BookPanel.rect.height / 2);
        ebr = new Vector3(BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
        ebl = new Vector3(-BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
        st = new Vector3(0, BookPanel.rect.height / 2);
        radius1 = Vector2.Distance(sb, ebr);
        float pageWidth = BookPanel.rect.width / 2.0f;
        float pageHeight = BookPanel.rect.height;
        radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
    }

    public Vector3 transformPoint(Vector3 mouseScreenPos)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
            Vector2 localPos = BookPanel.InverseTransformPoint(mouseWorldPos);

            return localPos;
        }
        else if (canvas.renderMode == RenderMode.WorldSpace)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 globalEBR = transform.TransformPoint(ebr);
            Vector3 globalEBL = transform.TransformPoint(ebl);
            Vector3 globalSt = transform.TransformPoint(st);
            Plane p = new Plane(globalEBR, globalEBL, globalSt);
            float distance;
            p.Raycast(ray, out distance);
            Vector2 localPos = BookPanel.InverseTransformPoint(ray.GetPoint(distance));
            return localPos;
        }
        else
        {
            //Screen Space Overlay
            Vector2 localPos = BookPanel.InverseTransformPoint(mouseScreenPos);
            return localPos;
        }
    }
    void Update()
    {
        if (pageDragging && interactable)
        {
            UpdateBook();
        }
        //EnsureSingleAudioListener();

    }
    public void UpdateBook()
    {
        f = Vector3.Lerp(f, transformPoint(Input.mousePosition), Time.deltaTime * 10);
        if (mode == FlipMode.RightToLeft)
            UpdateBookRTLToPoint(f);
        else
            UpdateBookLTRToPoint(f);
    }
    public void UpdateBookLTRToPoint(Vector3 followLocation)
    {
        mode = FlipMode.LeftToRight;
        f = followLocation;
        ShadowLTR.transform.SetParent(ClippingPlane.transform, true);
        ShadowLTR.transform.localPosition = new Vector3(0, 0, 0);
        ShadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);
        Left.transform.SetParent(ClippingPlane.transform, true);

        Right.transform.SetParent(BookPanel.transform, true);
        Right.transform.localEulerAngles = Vector3.zero;
        LeftNext.transform.SetParent(BookPanel.transform, true);

        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float clipAngle = CalcClipAngle(c, ebl, out t1);
        //0 < T0_T1_Angle < 180
        clipAngle = (clipAngle + 180) % 180;

        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        //page position and angle
        Left.transform.position = BookPanel.TransformPoint(c);
        float C_T1_dy = t1.y - c.y;
        float C_T1_dx = t1.x - c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        Left.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - 90 - clipAngle);

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        LeftNext.transform.SetParent(NextPageClip.transform, true);
        Right.transform.SetParent(ClippingPlane.transform, true);
        Right.transform.SetAsFirstSibling();

        ShadowLTR.rectTransform.SetParent(Left.rectTransform, true);
    }
    public void UpdateBookRTLToPoint(Vector3 followLocation)
    {
        mode = FlipMode.RightToLeft;
        f = followLocation;
        Shadow.transform.SetParent(ClippingPlane.transform, true);
        Shadow.transform.localPosition = Vector3.zero;
        Shadow.transform.localEulerAngles = Vector3.zero;
        Right.transform.SetParent(ClippingPlane.transform, true);

        Left.transform.SetParent(BookPanel.transform, true);
        Left.transform.localEulerAngles = Vector3.zero;
        RightNext.transform.SetParent(BookPanel.transform, true);
        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float clipAngle = CalcClipAngle(c, ebr, out t1);
        if (clipAngle > -90) clipAngle += 180;

        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        //page position and angle
        Right.transform.position = BookPanel.TransformPoint(c);
        float C_T1_dy = t1.y - c.y;
        float C_T1_dx = t1.x - c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        Right.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - (clipAngle + 90));

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        RightNext.transform.SetParent(NextPageClip.transform, true);
        Left.transform.SetParent(ClippingPlane.transform, true);
        Left.transform.SetAsFirstSibling();

        Shadow.rectTransform.SetParent(Right.rectTransform, true);
    }
    private float CalcClipAngle(Vector3 c,Vector3 bookCorner,out  Vector3 t1)
    {
        Vector3 t0 = (c + bookCorner) / 2;
        float T0_CORNER_dy = bookCorner.y - t0.y;
        float T0_CORNER_dx = bookCorner.x - t0.x;
        float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
        float T0_T1_Angle = 90 - T0_CORNER_Angle;
        
        float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
        T1_X = normalizeT1X(T1_X, bookCorner, sb);
        t1 = new Vector3(T1_X, sb.y, 0);
        
        //clipping plane angle=T0_T1_Angle
        float T0_T1_dy = t1.y - t0.y;
        float T0_T1_dx = t1.x - t0.x;
        T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
        return T0_T1_Angle;
    }
    private float normalizeT1X(float t1,Vector3 corner,Vector3 sb)
    {
        if (t1 > sb.x && sb.x > corner.x)
            return sb.x;
        if (t1 < sb.x && sb.x < corner.x)
            return sb.x;
        return t1;
    }
    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 c;
        f = followLocation;
        float F_SB_dy = f.y - sb.y;
        float F_SB_dx = f.x - sb.x;
        float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
        Vector3 r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle),radius1 * Mathf.Sin(F_SB_Angle), 0) + sb;

        float F_SB_distance = Vector2.Distance(f, sb);
        if (F_SB_distance < radius1)
            c = f;
        else
            c = r1;
        float F_ST_dy = c.y - st.y;
        float F_ST_dx = c.x - st.x;
        float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
        Vector3 r2 = new Vector3(radius2 * Mathf.Cos(F_ST_Angle),
           radius2 * Mathf.Sin(F_ST_Angle), 0) + st;
        float C_ST_distance = Vector2.Distance(c, st);
        if (C_ST_distance > radius2)
            c = r2;
        return c;
    }
    public void DragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= bookPages.Length) return;
        pageDragging = true;
        mode = FlipMode.RightToLeft;
        f = point;


        NextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(0, 0);
        Left.transform.position = RightNext.transform.position;
        Left.transform.eulerAngles = new Vector3(0, 0, 0);
        Left.sprite = (currentPage < bookPages.Length) ? bookPages[currentPage] : background;
        Left.transform.SetAsFirstSibling();
        
        Right.gameObject.SetActive(true);
        Right.transform.position = RightNext.transform.position;
        Right.transform.eulerAngles = new Vector3(0, 0, 0);
        Right.sprite = (currentPage < bookPages.Length - 1) ? bookPages[currentPage + 1] : background;

        RightNext.sprite = (currentPage < bookPages.Length - 2) ? bookPages[currentPage + 2] : background;

        LeftNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) Shadow.gameObject.SetActive(true);
        UpdateBookRTLToPoint(f);
    }
    public void OnMouseDragRightPage()
    {
        if (interactable)
        DragRightPageToPoint(transformPoint(Input.mousePosition));
        
    }
    public void DragLeftPageToPoint(Vector3 point)
    {
        if (currentPage <= 0) return;
        pageDragging = true;
        mode = FlipMode.LeftToRight;
        f = point;

        NextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

        Right.gameObject.SetActive(true);
        Right.transform.position = LeftNext.transform.position;
        Right.sprite = bookPages[currentPage - 1];
        Right.transform.eulerAngles = new Vector3(0, 0, 0);
        Right.transform.SetAsFirstSibling();

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(1, 0);
        Left.transform.position = LeftNext.transform.position;
        Left.transform.eulerAngles = new Vector3(0, 0, 0);
        Left.sprite = (currentPage >= 2) ? bookPages[currentPage - 2] : background;

        LeftNext.sprite = (currentPage >= 3) ? bookPages[currentPage - 3] : background;

        RightNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) ShadowLTR.gameObject.SetActive(true);
        UpdateBookLTRToPoint(f);
    }
    public void OnMouseDragLeftPage()
    {
        if (interactable)
        DragLeftPageToPoint(transformPoint(Input.mousePosition));
        
    }
    public void OnMouseRelease()
    {
        if (interactable)
            ReleasePage();
    }
    public void ReleasePage()
    {
        if (pageDragging)
        {
            pageDragging = false;
            float distanceToLeft = Vector2.Distance(c, ebl);
            float distanceToRight = Vector2.Distance(c, ebr);
            if (distanceToRight < distanceToLeft && mode == FlipMode.RightToLeft)
                TweenBack();
            else if (distanceToRight > distanceToLeft && mode == FlipMode.LeftToRight)
                TweenBack();
            else
                TweenForward();
        }
    }
    Coroutine currentCoroutine;
    void UpdateSprites()
    {
        LeftNext.sprite= (currentPage > 0 && currentPage <= bookPages.Length) ? bookPages[currentPage-1] : background;
        RightNext.sprite=(currentPage>=0 &&currentPage<bookPages.Length) ? bookPages[currentPage] : background;
    }
    public void TweenForward()
    {
        if(mode== FlipMode.RightToLeft)
        currentCoroutine = StartCoroutine(TweenTo(ebl, 0.15f, () => { Flip(); }));
        else
        currentCoroutine = StartCoroutine(TweenTo(ebr, 0.15f, () => { Flip(); }));
    }
    void Flip()
    {
        if (mode == FlipMode.RightToLeft)
            currentPage += 2;
        else
            currentPage -= 2;
        //currentPage = Mathf.Clamp(currentPage, 0, bookPages.Length - 1); // 페이지 범위 제한
        
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.transform.SetParent(BookPanel.transform, true);
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        Right.transform.SetParent(BookPanel.transform, true);
        RightNext.transform.SetParent(BookPanel.transform, true);
        UpdateSprites();

        GotoOriginalAtOnce();
        //StartCoroutine(GotoOriginalPosition());

        UpdateTextVisibility(); // 페이지를 넘길 때마다 텍스트 업데이트

        audioSource.Stop();
        firstButtonPress = false;
        pressAllowed = true;
        SpeakStartStopButton = true;

        emotionSelection.gotoOriginalAtOnce();

        AskLineGuessCanvas.SetActive(false);
        AskEmotionCanvas.SetActive(false);
        SpeakStartButton.SetActive(false);
        SpeakStopButton.SetActive(false);

        Shadow.gameObject.SetActive(false);
        ShadowLTR.gameObject.SetActive(false);
        if (OnFlip != null)
            OnFlip.Invoke();
    }
    public void TweenBack()
    {
        if (mode == FlipMode.RightToLeft)
        {
            currentCoroutine = StartCoroutine(TweenTo(ebr,0.15f,
                () =>
                {
                    UpdateSprites();
                    RightNext.transform.SetParent(BookPanel.transform);
                    Right.transform.SetParent(BookPanel.transform);

                    Left.gameObject.SetActive(false);
                    Right.gameObject.SetActive(false);
                    pageDragging = false;
                }
                ));
        }
        else
        {
            currentCoroutine = StartCoroutine(TweenTo(ebl, 0.15f,
                () =>
                {
                    UpdateSprites();

                    LeftNext.transform.SetParent(BookPanel.transform);
                    Left.transform.SetParent(BookPanel.transform);

                    Left.gameObject.SetActive(false);
                    Right.gameObject.SetActive(false);
                    pageDragging = false;
                }
                ));
        }
    }
    public IEnumerator TweenTo(Vector3 to, float duration, System.Action onFinish)
    {
        int steps = (int)(duration / 0.025f);
        Vector3 displacement = (to - f) / steps;
        for (int i = 0; i < steps-1; i++)
        {
            if(mode== FlipMode.RightToLeft)
            UpdateBookRTLToPoint( f + displacement);
            else
                UpdateBookLTRToPoint(f + displacement);

            yield return new WaitForSeconds(0.025f);
        }
        if (onFinish != null)
            onFinish();
    }
}



