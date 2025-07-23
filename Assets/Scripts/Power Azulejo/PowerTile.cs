using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTile : MonoBehaviour{
    public Tile parent;
    
    public bool isPlayerTile = false;

    // Death stuff
    private float deathDuration = .3f;
    private float deathTargetScale = 0;

    // Outline stuff
    public SpriteRenderer outline;
    private bool canMove = false;
    private Color activeColor;
    private Color inactiveColor;
    private float colorTransitionDuration = .25f;

    public void GenerateTileData(){
        activeColor = PowerAudioVisualManager.Instance.GetActiveColor(isPlayerTile);
        inactiveColor = PowerAudioVisualManager.Instance.GetInactiveColor(isPlayerTile);
        outline.color = inactiveColor;
    }

    public void Die(){
        PowerManager.Instance.GetPowerSumoGame().RegisterDeath(isPlayerTile, this.gameObject);
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity * 0.2f;
        PowerCinemachine.Instance.DeathCamShake();
        StartCoroutine(DeathAnimation()); 
    }

    private IEnumerator DeathAnimation(){
        float lerp = 0;
        float current;
        float origin = gameObject.transform.localScale.x;

        while((lerp/deathDuration) <= 1){
            current = Mathf.Lerp(origin, deathTargetScale, (lerp/deathDuration));
            gameObject.transform.localScale = new Vector3(current, current, current);
            lerp += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }

    public void LaunchTile(Vector3 dir, float pct){
        GetComponent<PowerTilePhysics>().LaunchInDirection(dir, pct);
    }

    // ========= GETTERS E SETTERS =========
    public void SetParent(Tile _parent){
        parent = _parent;
    }

    public void SetIsPlayer(bool _state){
        isPlayerTile = _state;
    }

    public void SetCanMove(bool _state){
        canMove = _state;
        if(canMove){
            StartCoroutine(LerpColor(activeColor));
        } else {
            StartCoroutine(LerpColor(inactiveColor));
        }
    }

    private IEnumerator LerpColor(Color target){
        Color origin = outline.color;
        if(target == origin) yield break;

        float lerp = 0;
        Color current;

        while((lerp/colorTransitionDuration) <= 1){
            current = Color.Lerp(origin, target, (lerp/colorTransitionDuration));
            outline.color = current;
            lerp += Time.deltaTime;
            yield return null;
        }

    }
}
