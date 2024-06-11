using UnityEngine;
using TMPro;

public class InputFieldExample : MonoBehaviour
{
    public TMP_InputField inputField;

    public void Submit()
    {
        string inputValue = inputField.text;
        Debug.Log("Input Value: " + inputValue);
    }
}
