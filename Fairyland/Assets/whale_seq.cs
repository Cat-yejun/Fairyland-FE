using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whale_seq : MonoBehaviour
{
    public Transform whale;
    public GameObject menuCanvus;
    public GameObject angryCanvas;
    public GameObject happyCanvas;
    public GameObject surpriseCanvas;
    public GameObject backgroundPlane;
    public GameObject correctCanvas;
    public GameObject failCanvas;
    public GameObject menuText;

    public Transform newWhale;
    public GameObject newWhaleObject;

    public float moveDuration = 1.0f;
    public float scaleDuration = 1.0f;
    public float rotationDuration = 1.0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 scaleVelocity = Vector3.zero;
    private float rotationVelocity;

    void Start()
    {
        angryCanvas.SetActive(false);
        happyCanvas.SetActive(false);
        surpriseCanvas.SetActive(false);
        menuCanvus.SetActive(false);
        newWhaleObject.SetActive(false);
        correctCanvas.SetActive(false);
        failCanvas.SetActive(false);
        menuText.SetActive(false);
        //backgroundPlane.SetActive(false);
    }

    public void StartSequence()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        backgroundPlane.SetActive(false);

        Vector3 targetPosition = new Vector3(newWhale.position.x, newWhale.position.y, newWhale.position.z);
        Vector3 targetScale = newWhale.localScale;
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

