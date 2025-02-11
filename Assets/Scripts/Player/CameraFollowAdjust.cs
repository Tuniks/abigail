using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraZoomDistance : MonoBehaviour
{
    public Transform playerTransform;
    public float velocityThreshold = 0.1f;    // The smoothed velocity above which the player is considered moving.
    public float velocitySmoothing = 10f;     // Higher values mean faster smoothing.
    public float movementZoomDelay = 0.3f;    // Delay (in seconds) before zooming out when the player starts moving.
    public float zoomBackDelay = 0.5f;        // Delay (in seconds) before zooming in when the player stops moving.
    public float minCameraDistance = 10f;     // Zoomed-in distance.
    public float maxCameraDistance = 20f;     // Zoomed-out distance.
    public float zoomSpeed = 5f;              // Lerp speed for the camera distance transition.

    private CinemachineVirtualCamera vCam;
    private CinemachineFramingTransposer framingTransposer;
    private Vector3 lastPlayerPos;
    private float smoothedVelocity = 0f;

    private enum ZoomState { ZoomedIn, ZoomedOut }
    private ZoomState currentState = ZoomState.ZoomedIn;
    private float timer = 0f;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned.");
            enabled = false;
            return;
        }
        vCam = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer not found on the Virtual Camera. Add it in the Body section.");
            enabled = false;
            return;
        }
        lastPlayerPos = playerTransform.position;
        framingTransposer.m_CameraDistance = minCameraDistance;
    }

    void Update()
    {
        // Calculate instantaneous velocity.
        Vector3 delta = playerTransform.position - lastPlayerPos;
        float instantaneousVelocity = delta.magnitude / Time.deltaTime;
        lastPlayerPos = playerTransform.position;

        // Smooth the velocity to avoid jitter.
        smoothedVelocity = Mathf.Lerp(smoothedVelocity, instantaneousVelocity, Time.deltaTime * velocitySmoothing);

        // State machine: if zoomed in and smoothed velocity exceeds threshold, start counting.
        // When the delay is exceeded, switch to zoomed out.
        if (currentState == ZoomState.ZoomedIn)
        {
            if (smoothedVelocity > velocityThreshold)
            {
                timer += Time.deltaTime;
                if (timer >= movementZoomDelay)
                {
                    currentState = ZoomState.ZoomedOut;
                    timer = 0f;
                }
            }
            else
            {
                timer = 0f;
            }
        }
        // If zoomed out and smoothed velocity falls below threshold, start counting idle time.
        // When the idle delay is exceeded, switch to zoomed in.
        else // currentState == ZoomState.ZoomedOut
        {
            if (smoothedVelocity <= velocityThreshold)
            {
                timer += Time.deltaTime;
                if (timer >= zoomBackDelay)
                {
                    currentState = ZoomState.ZoomedIn;
                    timer = 0f;
                }
            }
            else
            {
                timer = 0f;
            }
        }

        // Determine the target camera distance based on state.
        float targetDistance = (currentState == ZoomState.ZoomedOut) ? maxCameraDistance : minCameraDistance;

        // Smoothly interpolate to the target distance.
        framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, targetDistance, Time.deltaTime * zoomSpeed);
    }
}
