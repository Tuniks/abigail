using UnityEngine;

public class BreakSpriteInHalf : MonoBehaviour
{
    public GameObject spriteLeft;  // Left half of the sprite
    public GameObject spriteRight; // Right half of the sprite
    public float breakForce = 2f;  // Force to apply to each half

    private bool isBroken = false;

    private void OnMouseDown()
    {
        if (!isBroken)
        {
            isBroken = true;

            // Add Rigidbody2D components to the halves
            Rigidbody2D rbLeft = spriteLeft.AddComponent<Rigidbody2D>();
            Rigidbody2D rbRight = spriteRight.AddComponent<Rigidbody2D>();

            // Apply force to separate the halves
            rbLeft.AddForce(new Vector2(-breakForce, 0), ForceMode2D.Impulse); // Move left half to the left
            rbRight.AddForce(new Vector2(breakForce, 0), ForceMode2D.Impulse); // Move right half to the right

            // Optional: Destroy the original sprite
            Destroy(gameObject);
        }
    }
}