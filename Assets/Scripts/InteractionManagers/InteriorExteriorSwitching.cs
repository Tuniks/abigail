using UnityEngine;

public class InteriorExteriorSwitching : MonoBehaviour
{
    public GameObject Interior;
    public GameObject Exterior;
    public GameObject Player;

    private bool playerIsInside = false;
    private bool InteriorMode = false;

    void Update()
    {
        // Press E to go inside
        if (playerIsInside && Input.GetKeyDown(KeyCode.E) && !InteriorMode)
        {
            Interior.SetActive(true);
            Exterior.SetActive(false);
            InteriorMode = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Player)
        {
            playerIsInside = true;

            // Automatically switch to exterior if coming from interior
            if (InteriorMode)
            {
                Interior.SetActive(false);
                Exterior.SetActive(true);
                InteriorMode = false;
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