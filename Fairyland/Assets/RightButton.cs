using UnityEngine;
using UnityEngine.UI;

public class RightButton : MonoBehaviour
{
    public ImageSwitcher imageSwitcher;

    public void OnClick()
    {
        imageSwitcher.NextImage();
    }
}