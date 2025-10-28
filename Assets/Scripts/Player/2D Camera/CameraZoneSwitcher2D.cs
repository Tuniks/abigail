using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CameraZoneSwitcher2D : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public int activePriority = 20;
    public int defaultPriority = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCam.Priority = activePriority;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCam.Priority = defaultPriority;
        }
    }
}