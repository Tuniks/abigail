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
    public float minCameraDistance = 10f;
    public float maxCameraDistance = 20f;
    public float zoomSpeed = 5f;

    private CinemachineVirtualCamera vCam;
    private CinemachineFramingTransposer framingTransposer;
    private Vector3 lastPlayerPos;
    private Vector3 lastZoomIn;
    private float smoothedVelocity = 0f;
    private float timer = 0f;

    private enum ZoomState { ZoomedIn, ZoomedOut }
    private ZoomState currentState = ZoomState.ZoomedIn;

    private CinemachineConfiner2D confiner;

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
        confiner = vCam.GetComponent<CinemachineConfiner2D>();

        if (framingTransposer == null || confiner == null)
        {
            Debug.LogError("Missing FramingTransposer or Confiner2D on Cinemachine camera.");
            enabled = false;
            return;
        }

        lastPlayerPos = playerTransform.position;
        lastZoomIn = playerTransform.position;
        framingTransposer.m_CameraDistance = minCameraDistance;
    }

    void Update()
    {
        float deltaDist = Vector3.Distance(playerTransform.position, lastZoomIn);

        Vector3 delta = playerTransform.position - lastPlayerPos;
        float instantaneousVelocity = delta.magnitude / Time.deltaTime;
        lastPlayerPos = playerTransform.position;
        smoothedVelocity = Mathf.Lerp(smoothedVelocity, instantaneousVelocity, Time.deltaTime * velocitySmoothing);

        if (currentState == ZoomState.ZoomedIn)
        {
            if (deltaDist > distanceThreshold)
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
        else
        {
            if (smoothedVelocity <= velocityThreshold)
            {
                timer += Time.deltaTime;
                if (timer >= zoomBackDelay)
                {
                    if (WillPlayerRemainVisible(minCameraDistance))
                    {
                        currentState = ZoomState.ZoomedIn;
                        lastZoomIn = playerTransform.position;
                    }
                    timer = 0f;
                }
            }
            else timer = 0f;
        }

        float targetDistance = (currentState == ZoomState.ZoomedOut) ? maxCameraDistance : minCameraDistance;
        framingTransposer.m_CameraDistance = Mathf.Lerp(framingTransposer.m_CameraDistance, targetDistance, Time.deltaTime * zoomSpeed);
    }

    bool WillPlayerRemainVisible(float targetDistance)
    {
        if (confiner.m_BoundingShape2D == null) return true;

        // Get camera reference and its future position
        Vector3 camForward = vCam.transform.forward;
        Vector3 futureCamPos = playerTransform.position - camForward * targetDistance;

        // Estimate future view size (Orthographic approximation)
        float vertExtent = targetDistance * Mathf.Tan(vCam.m_Lens.FieldOfView * Mathf.Deg2Rad * 0.5f);
        float horizExtent = vertExtent * vCam.m_Lens.Aspect;

        Vector2 bottomLeft = new Vector2(futureCamPos.x - horizExtent, futureCamPos.y - vertExtent);
        Vector2 topRight = new Vector2(futureCamPos.x + horizExtent, futureCamPos.y + vertExtent);

        // Only check if player's position is still within that future view area
        return (playerTransform.position.x >= bottomLeft.x &&
                playerTransform.position.x <= topRight.x &&
                playerTransform.position.y >= bottomLeft.y &&
                playerTransform.position.y <= topRight.y);
    }

}
