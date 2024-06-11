using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveImgScene : MonoBehaviour
{
    public GameObject textScreen;
    public GameObject ImgScreen;



    public void NextButtonClick()
    {
        textScreen.SetActive(false);
        ImgScreen.SetActive(true);

    }


}
