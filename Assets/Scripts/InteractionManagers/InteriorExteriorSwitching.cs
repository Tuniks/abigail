using UnityEngine;

public class InteriorExteriorSwitching : MonoBehaviour
{
    public GameObject Interior;
    public GameObject Exterior;
    public GameObject Player;

    private bool playerIsInside = false;
    private bool interiorMode = false;
    public AudioClip EnterSound;
    public AudioSource audioSource;

    public bool hasEverEnteredInterior { get; private set; } = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerIsInside && Input.GetKeyDown(KeyCode.E) && !interiorMode)
        {
            audioSource.PlayOneShot(EnterSound, 0.7F);
            Interior.SetActive(true);
            Exterior.SetActive(false);
            interiorMode = true;
            hasEverEnteredInterior = true; // ✅ Mark it forever
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Player)
        {
            playerIsInside = true;

            if (interiorMode)
            {
                Interior.SetActive(false);
                Exterior.SetActive(true);
                interiorMode = false; // ok to reset this
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Player)
        {
            playerIsInside = false;
        }
    }
}
