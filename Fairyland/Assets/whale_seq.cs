using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class whale_seq : MonoBehaviour
{
    public Transform whale;
    public GameObject whaleObject;
    public Transform bubble;

    public GameObject menuCanvus;
    public GameObject angryCanvas;
    public GameObject happyCanvas;
    public GameObject surpriseCanvas;
    public GameObject backgroundPlane;
    public GameObject correctCanvas;
    public GameObject failCanvas;
    public GameObject menuText;
    public Button lineButton;
    public GameObject WhaleTalkCanvas;
    


    //public Transform newWhale;
    //public GameObject newWhaleObject;

    public float moveDuration = 1.0f;
    public float scaleDuration = 1.0f;
    public float rotationDuration = 1.0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 scaleVelocity = Vector3.zero;
    private float rotationVelocity;

    public RectTransform imageToEnlarge;
    public RectTransform buttonRectTransform;
    public TextMeshProUGUI textToEnlarge;

    private bool firstButtonPress = false;
    private bool pressAllowed = true;
    private bool isSpeakStart = false;
    private bool SpeakStartStopButton = true;

    public GameObject SpeakStartCanvas;
    public GameObject SpeakStopCanvas;
    public Button speakStopButton; // speakStop 버튼
    public Button speakStartButton; // speakStart 버튼
    private float initialDelay = 2f; // 초기 지연 시간
    private float displayDuration = 3f; // 텍스트와 버튼 표시 시간

    void Start()
    {
        if (whale == null)
        {
            Debug.LogError("Whale transform is not assigned.");
        }
        //if (newWhale == null)
        //{
        //    Debug.LogError("New Whale transform is not assigned.");
        //}

        angryCanvas.SetActive(false);
        happyCanvas.SetActive(false);
        surpriseCanvas.SetActive(false);
        menuCanvus.SetActive(false);
        //newWhaleObject.SetActive(false);
        correctCanvas.SetActive(false);
        failCanvas.SetActive(false);
        menuText.SetActive(false);
        WhaleTalkCanvas.SetActive(false);
        SpeakStartCanvas.SetActive(false);
        SpeakStopCanvas.SetActive(false);
        Debug.Log("set all canvas to false");

        //backgroundPlane.SetActive(false);
    }

    public void linePressSequence()
    {

        //if (newWhale == null)
        //{
        //    Debug.LogError("Attempted to start sequence but New Whale Transform is not assigned.");
        //    return;
        //}
        //else
        //{
        //    Debug.Log("newWhale is not null!");
        //}
        //lineButton.interactable = false;
        if (!firstButtonPress)
        {   
            if(pressAllowed == true && SpeakStartStopButton == true)
            {
                firstButtonPress = true;
                pressAllowed = false;
                StartCoroutine(EnlargeAndCenterImage());
                StartCoroutine(InitialSequence());
                pressAllowed = true;
            }
            
        }
    }

    IEnumerator EnlargeAndCenterImage()
    {
        Vector2 originalSize = imageToEnlarge.sizeDelta;
        Vector2 enlargedSize = new Vector2((float)(imageToEnlarge.sizeDelta.x * 1.5), (float)(imageToEnlarge.sizeDelta.y * 2.0));


        Vector2 originalPosition = imageToEnlarge.anchoredPosition;
        Vector2 originalButtonPosition = buttonRectTransform.anchoredPosition;
        Vector2 targetPosition = Vector2.zero;

        float originalFontSize = textToEnlarge.fontSize;
        float enlargedFontSize = (float)(originalFontSize * 1.5);

        Vector2 originalTextboxSize = textToEnlarge.rectTransform.sizeDelta;
        Vector2 enlargedTextboxSize = new Vector2((float)(originalTextboxSize.x * 1.5), (float)(originalTextboxSize.y * 3.0));

        Vector2 originalButtonSize = buttonRectTransform.sizeDelta;
        Vector2 enlargedButtonSize = new Vector2((float)(buttonRectTransform.sizeDelta.x * 1.8), (float)(buttonRectTransform.sizeDelta.y * 1.4));

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            imageToEnlarge.sizeDelta = Vector2.Lerp(originalSize, enlargedSize, t);
            imageToEnlarge.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, t);
            textToEnlarge.fontSize = (int) Mathf.Lerp(originalFontSize, enlargedFontSize, t);
            textToEnlarge.rectTransform.sizeDelta = Vector2.Lerp(originalTextboxSize, enlargedTextboxSize, t);
            buttonRectTransform.anchoredPosition = Vector2.Lerp(originalButtonPosition, targetPosition, t);
            buttonRectTransform.sizeDelta = Vector2.Lerp(originalButtonSize, enlargedButtonSize, t);

            yield return null;
        }

        imageToEnlarge.sizeDelta = enlargedSize;
        imageToEnlarge.anchoredPosition = targetPosition;
        textToEnlarge.fontSize = (int) enlargedFontSize;
        buttonRectTransform.anchoredPosition = targetPosition;
        buttonRectTransform.sizeDelta = enlargedButtonSize;
    }

    public void OnSpeakStartButtonClick()
    {
        SpeakStartStopButton = false;

        SpeakStopCanvas.SetActive(true);
        speakStopButton.interactable = true;
        SpeakStartCanvas.SetActive(false);
        isSpeakStart = true;

        // 음성을 받아오는 로직 추가

        SpeakStartStopButton = true;
    }

    public void OnSpeakStopButtonClick()
    {
        SpeakStartStopButton = false;

        SpeakStopCanvas.SetActive(false);
        SpeakStartCanvas.SetActive(true);
        isSpeakStart = false;
        speakStartButton.interactable = false;

        StartCoroutine(SpeakStopSequence());

        SpeakStartStopButton = true;
    }

    IEnumerator SpeakStopSequence()
    {

        yield return new WaitForSeconds(3f);
        // 음성 처리 시간 / 음성 처리 로직 추가

        StartCoroutine(Sequence());
    }

    IEnumerator InitialSequence()
    {
        WhaleTalkCanvas.SetActive(true);
        SpeakStartCanvas.SetActive(true);
        speakStartButton.interactable = false;

        yield return new WaitForSeconds(displayDuration);

        speakStartButton.interactable = true;

    }

    IEnumerator Sequence()
    {
        //if (whale == null || newWhale == null || newWhaleObject == null)
        //{
        //    Debug.LogError("One or more required objects are null.");
        //    yield break;
        //}

        backgroundPlane.SetActive(false);
        WhaleTalkCanvas.SetActive(false);
        SpeakStartCanvas.SetActive(false);
        SpeakStopCanvas.SetActive(false);

        Debug.Log(bubble.position);

        Vector3 targetPosition = new Vector3(0.0f, -1.5f, 0.0f);
        Vector3 targetScale = new Vector3(438.0f, 438.0f, 438.0f);
        Quaternion originalRotation = whale.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 180, 0);

        float elapsedTime = 0;

        while (Vector3.Distance(whale.position, targetPosition) > 3.0f || Vector3.Distance(whale.localScale, targetScale) > 3.0f)
        {
            whale.position = Vector3.SmoothDamp(whale.position, targetPosition, ref velocity, moveDuration, Mathf.Infinity, Time.deltaTime);
            whale.localScale = Vector3.SmoothDamp(whale.localScale, targetScale, ref scaleVelocity, scaleDuration, Mathf.Infinity, Time.deltaTime);
            whale.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / rotationDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        whale.position = targetPosition;
        whale.localScale = targetScale;
        whale.rotation = targetRotation;

        menuCanvus.SetActive(true);
        menuText.SetActive(true);

    }
}

