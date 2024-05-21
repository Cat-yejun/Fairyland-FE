using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ImageDownloaderTest : MonoBehaviour
{
    // 이미지 다운로드를 위한 링크 목록
    private string[] imageLinks = new string[]
    {
        "https://oaidalleapiprodscus.blob.core.windows.net/private/org-Ezx4aMh17ilkf27pjCwerokV/capstone2/img-3iv73UC5mhcafjUlZJ2hYI4b.png?st=2024-05-21T12%3A55%3A44Z&se=2024-05-21T14%3A55%3A44Z&sp=r&sv=2021-08-06&sr=b&rscd=inline&rsct=image/png&skoid=6aaadede-4fb3-4698-a8f6-684d7786b067&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2024-05-21T10%3A26%3A21Z&ske=2024-05-22T10%3A26%3A21Z&sks=b&skv=2021-08-06&sig=7AV2QTsShEPeLY8SoI6xmfNaRt8hJTupzrRq8L4kbgc%3D",
        "https://oaidalleapiprodscus.blob.core.windows.net/private/org-Ezx4aMh17ilkf27pjCwerokV/capstone2/img-bKPE0ZJgnkgiA7itbpJXwjtg.png?st=2024-05-21T12%3A56%3A14Z&se=2024-05-21T14%3A56%3A14Z&sp=r&sv=2021-08-06&sr=b&rscd=inline&rsct=image/png&skoid=6aaadede-4fb3-4698-a8f6-684d7786b067&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2024-05-21T10%3A09%3A28Z&ske=2024-05-22T10%3A09%3A28Z&sks=b&skv=2021-08-06&sig=u1WQXLu3Fx%2BtEkaGJ5KwZdxiU3mVq%2BC4QdUDiRas6Ro%3D",
        "https://oaidalleapiprodscus.blob.core.windows.net/private/org-Ezx4aMh17ilkf27pjCwerokV/capstone2/img-Gh6kgLrDusfNPw1eCszcZbKV.png?st=2024-05-21T12%3A56%3A43Z&se=2024-05-21T14%3A56%3A43Z&sp=r&sv=2021-08-06&sr=b&rscd=inline&rsct=image/png&skoid=6aaadede-4fb3-4698-a8f6-684d7786b067&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2024-05-21T10%3A09%3A25Z&ske=2024-05-22T10%3A09%3A25Z&sks=b&skv=2021-08-06&sig=L5d%2Bc2/iO9NNyW6fMLWO/MkG0mRFJeLJU7k/OmL5s2E%3D"
    };

    public Button downloadButton;

    private void Start()
    {
        // 버튼 클릭 이벤트에 메서드 추가
        if (downloadButton != null)
        {
            downloadButton.onClick.AddListener(OnDownloadButtonClick);
        }
    }

    public void OnDownloadButtonClick()
    {
        // 버튼 클릭 시 이미지 다운로드 시작
        StartCoroutine(DownloadImages());
    }

    private IEnumerator DownloadImages()
    {
        for (int i = 0; i < imageLinks.Length; i++)
        {
            string url = imageLinks[i];
            yield return StartCoroutine(DownloadImage(url, $"image_{i}.png"));
        }
    }

    private IEnumerator DownloadImage(string url, string fileName)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download image: {uwr.error}");
            }
            else
            {
                // 텍스처를 얻음
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                // 텍스처를 PNG로 변환
                byte[] imageData = texture.EncodeToPNG();

                // 파일 경로 생성
                string filePath = Path.Combine(Application.dataPath, "Savefile", fileName);

                // 폴더가 없으면 생성
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // 파일 저장
                File.WriteAllBytes(filePath, imageData);

                Debug.Log($"Image saved to {filePath}");
            }
        }
    }
}
