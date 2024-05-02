using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class expressionChange : MonoBehaviour
{
    public Material[] expressions;
    private int currentExpressionIndex = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeExpression();
        }
    }

    void ChangeExpression()
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            currentExpressionIndex = (currentExpressionIndex + 1) % expressions.Length;
            renderer.material = expressions[currentExpressionIndex];
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer not found on the game object.");
        }
    }
}
