using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angry_button : MonoBehaviour
{
    public GameObject menuCanvas;
    public Animator whaleAnimator;
    public Material[] expressions;

    public void selectButton(int expression)
    {
        whaleAnimator.SetInteger("NextInt", expression);
        menuCanvas.SetActive(false);

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
}

