using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingInteraction : MonoBehaviour
{
    public bool CurtainInteractionHappened = false;
    public GameObject BeforeInteraction;
    public GameObject AfterInteraction;
    public AudioClip Sound;
    public AudioSource audioSource;

    [Header("Interaction Tiles")]
    public Tile[] tile = null;
    public List<Tile> FishTiles = new List<Tile>(); // Holds tiles you've collected

    public Transform player;
    public float interactionRadius = 2f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (BeforeInteraction != null && player != null)
        {
            float distance = Vector2.Distance(BeforeInteraction.transform.position, player.position);

            if (Input.GetKeyDown(KeyCode.E) && !CurtainInteractionHappened && distance <= interactionRadius)
            {
                AfterInteraction.SetActive(true);
                BeforeInteraction.SetActive(false);
                CurtainInteractionHappened = true;
                audioSource.PlayOneShot(Sound, 0.7F);
            }

            else if (Input.GetKeyDown(KeyCode.E) && CurtainInteractionHappened && distance <= interactionRadius)
            {
                AfterInteraction.SetActive(false);
                BeforeInteraction.SetActive(true);
                CurtainInteractionHappened = false;
                audioSource.PlayOneShot(Sound, 0.7F);

                if (tile != null && tile.Length > 0)
                {
                    // Select one random tile
                    Tile selectedTile = tile[Random.Range(0, tile.Length)];

                    // Add it to FishTiles and PlayerInventory
                    FishTiles.Add(selectedTile);
                    PlayerInventory.Instance.AddTilesToCollection(new Tile[] { selectedTile });
                }
            }
        }
    }
}