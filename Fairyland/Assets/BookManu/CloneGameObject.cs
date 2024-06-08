using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Text;

public class CloneGameObject : MonoBehaviour
{
    // This is the original GameObject that you want to clone.
    public GameObject originalGameObject;

    // Start is called before the first frame update
    void Start()
    {
        CloneFromPNGs();
    }

    // This method will be called to clone the GameObject based on PNG files in a directory.
    public void CloneFromPNGs()
    {
        // Define the path to the directory containing PNG files.
        string directoryPath = Application.persistentDataPath + "/SaveFile/";

        // Check if the directory exists.
        if (Directory.Exists(directoryPath))
        {
            // Get all PNG files in the directory.
            string[] pngFiles = Directory.GetFiles(directoryPath, "*.png");

            // Clone the original GameObject for each PNG file.
            foreach (string pngFilePath in pngFiles)
            {
                // Normalize file name to NFC
                string normalizedFileName = NormalizeFileName(Path.GetFileNameWithoutExtension(pngFilePath));

                // Create a clone of the original GameObject.
                GameObject clonedGameObject = Instantiate(originalGameObject);

                // Set the name of the cloned GameObject.
                clonedGameObject.name = originalGameObject.name + "_Clone";

                // Set the parent of the cloned GameObject to be the same as the original GameObject's parent.
                clonedGameObject.transform.SetParent(originalGameObject.transform.parent);

                // Set the position of the cloned GameObject to be the same as the original GameObject.
                clonedGameObject.transform.position = originalGameObject.transform.position;

                // Set the local position of the cloned GameObject to be the same as the original GameObject.
                clonedGameObject.transform.localPosition = originalGameObject.transform.localPosition;

                // Set the scale of the cloned GameObject to be the same as the original GameObject.
                clonedGameObject.transform.localScale = originalGameObject.transform.localScale;

                // Extract the file name without the extension to use as the text.
                string[] fileNameParts = normalizedFileName.Split('-');

                // Check if at least two parts exist (to ensure there is a name after the "-").
                if (fileNameParts.Length >= 2)
                {
                    // Get the second part (after the "-") to use as the text.
                    string nameAfterDash = fileNameParts[1];

                    // Change the text of the nested child object.
                    ChangeNestedChildText(clonedGameObject, "BookCover/BookTitle/Text", nameAfterDash);
                }
                else
                {
                    Debug.LogWarning("File name does not contain a dash (-) separator: " + normalizedFileName);
                }

                // Start coroutine to set the image of the nested child object.
                StartCoroutine(SetBookCoverImage(clonedGameObject, "BookCover", pngFilePath));
                Debug.Log("경로 이름: " + normalizedFileName);
            }
        }
        else
        {
            // If the directory does not exist, log a warning.
            Debug.LogWarning("Directory not found: " + directoryPath);
        }
        // After all clones are created, hide the original GameObject.
        originalGameObject.SetActive(false);
    }

    // Normalize file name to NFC
    private string NormalizeFileName(string fileName)
    {
        return fileName.Normalize(NormalizationForm.FormC);
    }

    // This method changes the text of a nested child object.
    private void ChangeNestedChildText(GameObject parent, string childPath, string newText)
    {
        // Find the nested child object by path.
        Transform childTransform = parent.transform.Find(childPath);

        // If the nested child object is found and it has a TMP_Text component, change the text.
        if (childTransform != null)
        {
            TMP_Text textComponent = childTransform.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = newText;
            }
            else
            {
                Debug.LogWarning("No TMP_Text component found on the nested child object.");
            }
        }
        else
        {
            Debug.LogWarning("Nested child object with the specified path not found.");
        }
    }

    // This coroutine loads a PNG file and sets it as the sprite for an Image component.
    private IEnumerator SetBookCoverImage(GameObject parent, string childPath, string pngFilePath)
    {
        // Find the nested child object by path.
        Transform childTransform = parent.transform.Find(childPath);

        // If the nested child object is found and it has an Image component, set the sprite.
        if (childTransform != null)
        {
            Image imageComponent = childTransform.GetComponent<Image>();
            if (imageComponent != null)
            {
                // Load the PNG file as a texture.
                byte[] pngBytes = File.ReadAllBytes(pngFilePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(pngBytes);

                // Create a sprite from the texture.
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(texture, rect, pivot);

                // Set the sprite of the Image component.
                imageComponent.sprite = sprite;
            }
            else
            {
                Debug.LogWarning("No Image component found on the nested child object.");
            }
        }
        else
        {
            Debug.LogWarning("Nested child object with the specified path not found.");
        }

        yield return null;
    }
}
