using UnityEngine;

public class CloneGameObject : MonoBehaviour
{
    // This is the original GameObject that you want to clone.
    public GameObject originalGameObject;

    // This method will be called when you want to clone the GameObject.
    public void Clone()
    {
        // Check if the original GameObject is assigned.
        if (originalGameObject != null)
        {
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

            // Additional properties of the cloned GameObject can be set here as needed.
        }
        else
        {
            // If the original GameObject is not assigned, log a warning.
            Debug.LogWarning("Original GameObject is not assigned.");
        }
    }
}
