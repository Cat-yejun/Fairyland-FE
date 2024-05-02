using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public float _oscillationSpeed = 5f; // 진동 속도

    // Update is called once per frame
    void Update()
    {

        // sin 함수를 사용하여 부드러운 진동을 만듭니다.
        float oscillation = Mathf.Sin(Time.time * _oscillationSpeed);

        // 회전 적용
        transform.rotation = Quaternion.Euler(0, 0, oscillation*3);
    }

    void resetAnim()
    {
        transform.rotation = Quaternion.identity;
    }
}

