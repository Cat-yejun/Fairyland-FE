using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RolePlayingScript : MonoBehaviour
{
    public GameObject whaleObject; // 고래 오브젝트
    public Animator whaleAnimator;
    public Material[] whaleExpressions; // 고래 표정 Material 배열
    public GameObject canvasLine1; // 첫 번째 라인
    //public GameObject canvasLine2; // 두 번째 라인
    public Button speakStopButton; // speakStop 버튼
    public Button speakStartButton; // speakStart 버튼
    public float initialDelay = 2f; // 초기 지연 시간
    public float displayDuration = 3f; // 텍스트와 버튼 표시 시간


    //public Button yourButton; // 버튼 참조
    public string sceneToLoad = "newBook"; // 이동할 씬의 이름
    public string sceneToUnload = "Role_Playing_3d"; // 현재 씬의 이름

    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


    // Start is called before the first frame update
    void Start()
    {
        // 초기 상태 설정
        changeExpression(0);
        canvasLine1.SetActive(false);
        //canvasLine2.SetActive(false);
        speakStopButton.gameObject.SetActive(false);
        speakStartButton.gameObject.SetActive(false);

        // 초기 딜레이 후 첫 번째 상태로 전환
        StartCoroutine(InitialSequence());
    }

    IEnumerator InitialSequence()
    {
        yield return new WaitForSeconds(initialDelay);

        // 첫 번째 라인과 speakStop 버튼 활성화
        changeExpression(1);
        whaleAnimator.SetInteger("NextInt", 4);
        canvasLine1.SetActive(true);
        speakStartButton.interactable = false;
        speakStartButton.gameObject.SetActive(true);

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

    IEnumerator SpeakStopSequence()
    {
        //다음 표정과 라인 전환
        //speakStartButton.interactable = false;

        //yield return new WaitForSeconds(3f); // 음성 처리 시간

        //changeExpression(3);
        //canvasLine1.SetActive(false);
        ////canvasLine2.SetActive(true);

        //yield return new WaitForSeconds(displayDuration); // 고래가 말하는 시간

        //// speakStart 버튼을 클릭 가능하도록 설정
        //speakStartButton.interactable = true;
        yield return new WaitForSeconds(3f);

        SwitchScene();

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
