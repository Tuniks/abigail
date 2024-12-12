using UnityEngine;
using TMPro;

public class GameObjectInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The GameObject to activate and deactivate.")]
    public GameObject targetObject;

    [Tooltip("The TextMeshPro asset to update.")]
    public TextMeshProUGUI textDisplay;

    [Header("Text Content")]
    [Tooltip("The text to display in the TextMeshPro asset.")]
    [TextArea]
    public string inspectorText;

    private bool isActive = false;

  
    private void OnMouseDown()
    {
        if (targetObject != null && textDisplay != null)
        {
            // Activate the target object and display the text
            targetObject.SetActive(true);
            textDisplay.text = inspectorText;
            isActive = true;
        }
        else
        {
            Debug.LogWarning("Target Object or TextMeshPro reference is missing.");
        }
    }

    private void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Key 'E' pressed.");
        
            // Clear the text and deactivate the target object
            if (textDisplay != null)
            {
                textDisplay.text = "";
                Debug.Log("Text Display cleared.");
            }
            if (targetObject != null)
            {
                targetObject.SetActive(false);
                Debug.Log("Target Object deactivated: " + targetObject.name);
            }
            isActive = false;
            Debug.Log("isActive set to false.");
        }
    }

}