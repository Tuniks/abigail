using UnityEngine;

public class LockZPosition : MonoBehaviour
{
    public float lockedZ = 0f; // Set this to the desired Z position

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;
        newPosition.z = lockedZ;
        transform.position = newPosition;
    }
}