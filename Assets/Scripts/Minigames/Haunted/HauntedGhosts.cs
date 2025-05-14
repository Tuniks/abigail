using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntedGhosts : MonoBehaviour{
    public List<GameObject> totalGhosts = new List<GameObject>();

    public Vector2 spawnTimeRange = Vector2.zero;
    private float currentSpawnTime = 0;

    private List<GameObject> currentGhosts;
    private GameObject current = null;
    
    public AudioClip ghostHitSound;

    void Start(){
        currentGhosts = new List<GameObject>(totalGhosts);
    }

    void Update(){
        currentSpawnTime-=Time.deltaTime;
        if(currentSpawnTime <= 0){
            SpawnGhost();
        }
    }

    private void SpawnGhost(){
        if(current!=null) current.SetActive(false);
        current = currentGhosts[Random.Range(0, currentGhosts.Count)];
        current.SetActive(true);
        currentSpawnTime = Random.Range(spawnTimeRange.x, spawnTimeRange.y);
    }

    public void GhostHit(GameObject ghost){
        ghost.SetActive(false);
        currentGhosts.Remove(ghost);
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(ghostHitSound);
    }

    public void ResetMinigame(){
        currentGhosts = new List<GameObject>(totalGhosts);
    }
}
