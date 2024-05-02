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
    public GameObject backgroundPlane;
    public Transform newWhale;
    public GameObject newWhaleObject;

    void Start()
    {
        menuCanvus.SetActive(false);
        newWhaleObject.SetActive(false);
        //backgroundPlane.SetActive(false);
    }

    public void StartSequence()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        backgroundPlane.SetActive(false);
        menuCanvus.SetActive(true);

        Vector3 centerPosition = new Vector3(-4.9f, -5.1f, whale.position.z+6);
        whale.position = Vector3.Lerp(whale.position, centerPosition, 0.5f);
        whale.localScale = Vector3.Lerp(whale.localScale, whale.localScale * 4.0f, 0.5f);

        
        yield return new WaitForSeconds(2);

    }
}

