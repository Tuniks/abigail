using UnityEngine;

public class InteriorExteriorSwitching : MonoBehaviour
{
    public GameObject Interior;
    public GameObject Exterior;
    public GameObject Player;

    private bool playerIsInside = false;
    private bool interiorMode = false;

    public bool hasEverEnteredInterior { get; private set; } = false;

    void Update()
    {
        if (playerIsInside && Input.GetKeyDown(KeyCode.E) && !interiorMode)
        {
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
