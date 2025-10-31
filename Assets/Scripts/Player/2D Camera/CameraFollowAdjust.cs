using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraZoomDistance : MonoBehaviour
{
    public Transform playerTransform;
    public float distanceThreshold = 2f;
    public float velocityThreshold = 0.2f;
    public float velocitySmoothing = 10f;
    public float movementZoomDelay = 0.3f;
    public float zoomBackDelay = 0.5f;

    [Tooltip("Orthographic Size when zoomed in")]
    public float minOrthoSize = 5f;
    [Tooltip("Orthographic Size when zoomed out")]
    public float maxOrthoSize = 10f;
    public float zoomSpeed = 5f;

    private CinemachineVirtualCamera vCam;
    private Vector3 lastPlayerPos;
    private Vector3 lastZoomInPos;
    private float smoothedVelocity;
    private float timer;

    private enum ZoomState { ZoomedIn, ZoomedOut }
    private ZoomState currentState = ZoomState.ZoomedIn;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("CameraZoomDistance: Player Transform not assigned.");
            enabled = false;
            return;
        }

        vCam = GetComponent<CinemachineVirtualCamera>();
        vCam.m_Lens.Orthographic = true;
        vCam.m_Lens.OrthographicSize = minOrthoSize;

        lastPlayerPos = playerTransform.position;
        lastZoomInPos = playerTransform.position;
    }

    void Update()
    {
        float movedSinceZoomIn = Vector3.Distance(playerTransform.position, lastZoomInPos);

        Vector3 delta = playerTransform.position - lastPlayerPos;
        float instVel = delta.magnitude / Time.deltaTime;
        lastPlayerPos = playerTransform.position;
        smoothedVelocity = Mathf.Lerp(smoothedVelocity, instVel, Time.deltaTime * velocitySmoothing);

        if (currentState == ZoomState.ZoomedIn)
        {
            if (movedSinceZoomIn > distanceThreshold)
            {
                timer += Time.deltaTime;
                if (timer >= movementZoomDelay)
                {
                    currentState = ZoomState.ZoomedOut;
                    timer = 0f;
                }
            }
            else timer = 0f;
        }
        else // ZoomedOut
        {
            if (smoothedVelocity <= velocityThreshold)
            {
                timer += Time.deltaTime;
                if (timer >= zoomBackDelay)
                {
                    currentState = ZoomState.ZoomedIn;
                    lastZoomInPos = playerTransform.position;
                    timer = 0f;
                }
            }
            else timer = 0f;
        }

        float targetSize = (currentState == ZoomState.ZoomedOut) ? maxOrthoSize : minOrthoSize;
        vCam.m_Lens.OrthographicSize = Mathf.Lerp(
            vCam.m_Lens.OrthographicSize,
            targetSize,
            Time.deltaTime * zoomSpeed
        );
    }
}
