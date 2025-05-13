using UnityEngine;
using Yarn.Unity;
using Cinemachine;

public class CameraFocusController : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public Transform playerTransform;
    public float lerpSpeed = 5f;
    public float focusFOV = 40f;

    private Transform currentTarget;
    private Transform lerpAnchor;

    private float originalFOV;
    private float targetFOV;
    private bool isFocusing = false;

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

        GameObject anchorObj = new GameObject("CameraLerpAnchor");
        lerpAnchor = anchorObj.transform;
        lerpAnchor.position = playerTransform.position;
        vCam.Follow = lerpAnchor;

        currentTarget = playerTransform;
        originalFOV = vCam.m_Lens.FieldOfView;
        targetFOV = originalFOV;
    }

    public void FocusCamera(string targetName)
    {
        GameObject targetObject = GameObject.Find(targetName);
        if (targetObject != null)
        {
            currentTarget = targetObject.transform;
            targetFOV = focusFOV;
            isFocusing = true;
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
            currentTarget = playerTransform;
            targetFOV = originalFOV;
            isFocusing = false;
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

        // Lerp the FOV smoothly
        if (vCam != null)
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(
                vCam.m_Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * lerpSpeed
            );
        }
    }
}
