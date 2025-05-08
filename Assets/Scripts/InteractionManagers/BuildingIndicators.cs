using UnityEngine;

public class BuildingIndicators : MonoBehaviour
{
    public float wiggleSpeed = 5f;
    public float wiggleAmount = 5f;
    public string playerTag = "Player";
    public InteriorExteriorSwitching interiorScript;

    private bool playerInside = false;
    private float wiggleTimer = 0f;

    void Update()
    {
        if (interiorScript != null && interiorScript.hasEverEnteredInterior)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            return; // ✅ permanently stop shaking
        }

        if (playerInside)
        {
            wiggleTimer += Time.deltaTime * wiggleSpeed;
            float zRotation = Mathf.Sin(wiggleTimer) * wiggleAmount;
            transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
        }
    }
}