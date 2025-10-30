

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonSimpleController : MonoBehaviour{
    private PlayerStatus ps;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpHeight = 1.6f;
    public float gravity = -24f; 
    public bool allowAirControl = true;

    [Header("Camera")]
    public Transform Cam;                 
    public float mouseSensitivity = 150f; 
    public bool invertY = false;
    public float cameraDistance = 4.5f;
    public float cameraHeight = 1.6f;     
    public Vector2 pitchLimits = new Vector2(-30f, 75f);
    public float cameraCollisionRadius = 0.2f; 
    public float cameraFollowLerp = 20f;       

    CharacterController controller;
    Vector3 velocity; 
    float yaw;        
    float pitch;      

    void Awake(){
        controller = GetComponent<CharacterController>();

        if (Cam != null)
        {   
            Vector3 toCam = Cam.position - GetPivot();
            Vector3 flat = new Vector3(toCam.x, 0f, toCam.z);
            if (flat.sqrMagnitude > 0.0001f)
                yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
            float dist = toCam.magnitude;
            if (dist > 0.0001f)
                pitch = Mathf.Asin(Mathf.Clamp(toCam.normalized.y, -1f, 1f)) * Mathf.Rad2Deg;
        }
    }

    void Start(){
        ps = PlayerStatus.Instance;
        if (ps.CanPoint()) SetCursorLocked(true);
    }

    void Update(){
        // Mouse look updates yaw/pitch
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mx = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float my = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            yaw += mx;
            pitch += (invertY ? my : -my);
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
        }

        // Move input (WASD)
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        moveInput = Vector3.ClampMagnitude(moveInput, 1f);
        
        // Negate movement if can't move
        if(!ps.CanMove()) moveInput = Vector3.zero;

        // Camera-relative move
        Vector3 camF = Vector3.forward;
        Vector3 camR = Vector3.right;

        if (Cam != null)
        {
            Vector3 f = Cam.forward; f.y = 0f; f.Normalize();
            Vector3 r = Cam.right; r.y = 0f; r.Normalize();
            camF = f;
            camR = r;
        }

        Vector3 desiredMove = camF * moveInput.z + camR * moveInput.x;
        desiredMove.Normalize();

        // Grounding + gravity
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f; // small downward force to keep grounded

        // Jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && ps.CanMove()){
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine horizontal + vertical motion
        Vector3 horizontal = desiredMove * moveSpeed;
        if (!isGrounded && !allowAirControl) horizontal = Vector3.zero;

        Vector3 motion = horizontal + new Vector3(0f, velocity.y, 0f);
        controller.Move(motion * Time.deltaTime);

        // Face the movement direction (optional: only if moving)
        Vector3 faceDir = horizontal;
        faceDir.y = 0f;
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.2f);
        }

        // Update camera position/orbit
        if (Cam != null )
        {
            UpdateCamera();
        }
    }

    void UpdateCamera()
    {
        // Desired camera position from spherical orbit
        Quaternion orbit = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = GetPivot();
        Vector3 idealCamPos = pivot + orbit * (Vector3.back * cameraDistance);

        // Simple collision: push camera closer if wall in the way
        Vector3 dir = (idealCamPos - pivot);
        float dist = dir.magnitude;
        if (dist > 0.001f)
        {
            dir /= dist;
            if (Physics.SphereCast(pivot, cameraCollisionRadius, dir, out RaycastHit hit, dist))
            {
                idealCamPos = hit.point + hit.normal * cameraCollisionRadius;
            }
        }

        // Smooth follow
        Cam.position = Vector3.Lerp(Cam.position, idealCamPos, 1f - Mathf.Exp(-cameraFollowLerp * Time.deltaTime));
        Cam.rotation = Quaternion.LookRotation((pivot - Cam.position).normalized, Vector3.up);
    }

    Vector3 GetPivot()
    {
        return transform.position + Vector3.up * cameraHeight;
    }

    public void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
