using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{
    public TileComponent face;
    public TileComponent background;
    public TileComponent material;
    public TileComponent glaze; 

    private PlayerHand playerHand;

    public void Initialize(GameObject _face, GameObject _bg, GameObject _mat, GameObject _glz){
        SubstituteComponent(_face, face.gameObject);
        SubstituteComponent(_bg, background.gameObject);
        SubstituteComponent(_mat, material.gameObject);
        SubstituteComponent(_glz, glaze.gameObject);

        face = _face.GetComponent<TileComponent>();
        background = _bg.GetComponent<TileComponent>();
        material = _mat.GetComponent<TileComponent>();
        glaze = _glz.GetComponent<TileComponent>();
    }

    private void SubstituteComponent(GameObject current, GameObject old){
        // current.transform.parent = gameObject.transform;
        // current.transform.localPosition = old.transform.localPosition;
        // current.transform.localRotation = old.transform.localRotation;
        // current.transform.localScale = old.transform.localScale;
        Destroy(old);
    }

    public void SetHand(PlayerHand hand){
        playerHand = hand;
    }

    void OnMouseDown(){
        if(playerHand == null) return;

        playerHand.Activate(this);
    }

    public float GetBeauty(){
        return face.beauty + background.beauty + material.beauty + glaze.beauty;
    }

    public float GetStrength(){
        return face.strength + background.strength + material.strength + glaze.strength;
    }

    public float GetStamina(){
        return face.stamina + background.stamina + material.stamina + glaze.stamina;
    }

    public float GetMagic(){
        return face.magic + background.magic + material.magic + glaze.magic;
    }

    public float GetSpeed(){
        return face.speed + background.speed + material.speed + glaze.speed;
    }


    
}
