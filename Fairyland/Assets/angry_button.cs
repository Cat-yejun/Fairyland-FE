using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class angry_button : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject menuText;
    public GameObject angryCanvas;
    public GameObject happyCanvas;
    public GameObject surpriseCanvas;
    public Animator whaleAnimator;
    public Material[] expressions;
    public GameObject correctCanvas;
    public GameObject failCanvas;
    public GameObject whaleObject;
    public GameObject backgroundPlane;
    public Transform whale;

    public Button angryButton;
    public Button happyButton;
    public Button surpriseButton;

    public int prevSelection = 0;

    private bool isDescriptionShown = false;

    public float moveDuration = 1.0f;
    public float scaleDuration = 1.0f;
    public float rotationDuration = 1.0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 scaleVelocity = Vector3.zero;
    private float rotationVelocity;

    public void selectButton(int expression)
    {
        StartCoroutine(selectionButton(expression));

    }

    private IEnumerator selectionButton(int expression)
    {
        angryButton.interactable = false;
        happyButton.interactable = false;
        surpriseButton.interactable = false;

        menuText.SetActive(false);
        whaleAnimator.SetInteger("NextInt", expression);
        changeExpression(expression);
        if (!isDescriptionShown)
        {
            if (expression == 1)
            {
                happyCanvas.SetActive(true);
                angryCanvas.SetActive(false);
                surpriseCanvas.SetActive(false);
                prevSelection = 1;
            }
            else if (expression == 2)
            {
                angryCanvas.SetActive(true);
                happyCanvas.SetActive(false);
                surpriseCanvas.SetActive(false);
                prevSelection = 2;
            }
            else
            {
                surpriseCanvas.SetActive(true);
                happyCanvas.SetActive(false);
                angryCanvas.SetActive(false);
                prevSelection = 3;
            }
            isDescriptionShown = true;
            yield return new WaitForSeconds(1);
            angryButton.interactable = true;
            happyButton.interactable = true;
            surpriseButton.interactable = true;
            whaleAnimator.SetInteger("NextInt", 0);
            changeExpression(0);

            //menuCanvas.SetActive(false);
        }
        else
        {
            if (expression == prevSelection)
            {
                StartCoroutine(ShowFeedbackAndHide());
            }
            else
            {
                whaleAnimator.SetInteger("NextInt", expression);
                changeExpression(expression);
                if (expression == 1)
                {
                    happyCanvas.SetActive(true);
                    angryCanvas.SetActive(false);
                    surpriseCanvas.SetActive(false);
                    prevSelection = 1;
                }
                else if (expression == 2)
                {
                    angryCanvas.SetActive(true);
                    happyCanvas.SetActive(false);
                    surpriseCanvas.SetActive(false);
                    prevSelection = 2;
                }
                else
                {
                    surpriseCanvas.SetActive(true);
                    happyCanvas.SetActive(false);
                    angryCanvas.SetActive(false);
                    prevSelection = 3;
                }
                isDescriptionShown = true;
                yield return new WaitForSeconds(1);
                angryButton.interactable = true;
                happyButton.interactable = true;
                surpriseButton.interactable = true;
                whaleAnimator.SetInteger("NextInt", 0);
                changeExpression(0);


            }
        }

    }

    public void changeExpression(int expression)
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            renderer.material = expressions[expression];
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer not found on the game object.");
        }
    }

    private IEnumerator ShowFeedbackAndHide()
    {
        if (prevSelection == 1)
        {
            happyCanvas.SetActive(false);
            angryCanvas.SetActive(false);
            surpriseCanvas.SetActive(false);
            correctCanvas.SetActive(true);
        }
        else
        {
            happyCanvas.SetActive(false);
            angryCanvas.SetActive(false);
            surpriseCanvas.SetActive(false);
            failCanvas.SetActive(true);
        }

        yield return new WaitForSeconds(4);

        correctCanvas.SetActive(false);
        failCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        //whaleObject.SetActive(false);

        isDescriptionShown = false;

        backgroundPlane.SetActive(true);


        //Vector3 targetPosition = new Vector3(5.88f, 1.42f, 90.00f);
        Vector3 targetPosition = new Vector3(5.46f, 1.35f, 89.99f);
        Vector3 targetScale = new Vector3(195.0f, 195.0f, 195.0f);
        Quaternion originalRotation = whale.rotation;
        Quaternion targetRotation = Quaternion.Euler(15, 200, 0);

        float elapsedTime = 0;

        whaleAnimator.SetInteger("NextInt", 0);
        changeExpression(0);

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

    }
}

