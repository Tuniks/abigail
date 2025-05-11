using UnityEngine;
using Yarn.Unity;
using Cinemachine;

public class CameraFocusController : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public Transform playerTransform;
    public float lerpSpeed = 5f;

    private Transform currentTarget;
    private Transform lerpAnchor;

    private void Start()
    {
        var runner = FindObjectOfType<DialogueRunner>();
        if (runner != null)
        {
            runner.AddCommandHandler<string>("focus_camera", FocusCamera);
            runner.AddCommandHandler("reset_camera_focus", ResetCameraFocus);
        }
        else
        {
            Debug.LogError("No DialogueRunner found in scene.");
        }

        // Create a dummy transform to follow smoothly
        GameObject anchorObj = new GameObject("CameraLerpAnchor");
        lerpAnchor = anchorObj.transform;
        lerpAnchor.position = playerTransform.position;
        vCam.Follow = lerpAnchor;

        currentTarget = playerTransform;
    }

    public void FocusCamera(string targetName)
    {
        GameObject targetObject = GameObject.Find(targetName);
        if (targetObject != null)
        {
            Debug.Log("Switching camera focus to: " + targetObject.name);
            currentTarget = targetObject.transform;
        }
        else
        {
            Debug.LogWarning("Target not found: " + targetName);
        }
    }

    public void ResetCameraFocus()
    {
        if (playerTransform != null)
        {
            Debug.Log("Resetting camera focus to player.");
            currentTarget = playerTransform;
        }
        else
        {
            Debug.LogWarning("Player Transform not assigned.");
        }
    }

    private void Update()
    {
        if (currentTarget != null && lerpAnchor != null)
        {
            lerpAnchor.position = Vector3.Lerp(
                lerpAnchor.position,
                currentTarget.position,
                Time.deltaTime * lerpSpeed
            );
        }
    }
}