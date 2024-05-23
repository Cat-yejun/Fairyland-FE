using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class SceneData
{
    public static Dictionary<string, object> Data = new Dictionary<string, object>();
}


public class SceneSwitcher : MonoBehaviour
{
    //public Button yourButton; // 버튼 참조
    public string sceneToLoad = "newBook"; // 이동할 씬의 이름
    public string sceneToUnload = "Role_Playing_3d"; // 현재 씬의 이름

    public void SwitchScene()
    {
        SaveCurrentSceneData();
        SceneManager.LoadScene(sceneToLoad);
    }

    void SaveCurrentSceneData()
    {
        // 예제 데이터 저장
        SceneData.Data["currentPage"] = 2;
        // 필요한 다른 데이터도 저장합니다.
    }

    void LoadCurrentSceneData()
    {
        if (SceneData.Data.ContainsKey("currentPage"))
        {
            object exampleValue = SceneData.Data["currentPage"];
            // 저장된 데이터를 복원합니다.
            Debug.Log("Restored value: " + exampleValue);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadCurrentSceneData();
    }

}
