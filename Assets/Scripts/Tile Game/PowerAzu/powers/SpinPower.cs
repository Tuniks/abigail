using UnityEngine;

public class SpinPower : MonoBehaviour, ITilePower {
    public float spinSpeed = 360f;
    public float friction = 180f;
    public GameObject spinEffectPrefab;
    public AudioClip spinSound;
    public float stretchAmount = 0.15f;
    public Sprite icon;
    public Sprite Icon => icon;



    private float currentSpeed = 0f;
    private bool spinning = false;
    private GameObject spinEffectInstance;
    private float accumulatedRotation = 0f;
    private Vector3 originalScale;

    public void Activate(Tile tile) {
        if (spinning) return;

        spinning = true;
        currentSpeed = spinSpeed;
        accumulatedRotation = 0f;
        originalScale = tile.transform.localScale;

        if (spinSound)
            AudioSource.PlayClipAtPoint(spinSound, tile.transform.position);

        if (spinEffectPrefab != null) {
            spinEffectInstance = Instantiate(spinEffectPrefab, tile.transform.position, Quaternion.identity, tile.transform);
        }
    }

    void Update() {
        if (!spinning) return;

        float rotationThisFrame = currentSpeed * Time.deltaTime;
        accumulatedRotation += rotationThisFrame;
        transform.Rotate(0, 0, rotationThisFrame);

        float stretchFactor = Mathf.Sin(Time.time * 10f) * stretchAmount;
        transform.localScale = new Vector3(
            originalScale.x + stretchFactor,
            originalScale.y - stretchFactor,
            originalScale.z);

        if (accumulatedRotation >= 360f && spinEffectInstance != null) {
            Destroy(spinEffectInstance);
            spinEffectInstance = null;
        }

        currentSpeed -= friction * Time.deltaTime;
        if (currentSpeed <= 0f) {
            currentSpeed = 0f;
            spinning = false;
            transform.localScale = originalScale;
        }
    }
}