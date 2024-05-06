using UnityEngine;
using UnityEngine.UI;

public class LeftButton : MonoBehaviour
{
    public ImageSwitcher imageSwitcher;

    public void OnClick()
    {
        imageSwitcher.PreviousImage();
    }
}
