using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{
    public TileComponent face;
    public TileComponent background;
    public TileComponent material;
    public TileComponent glaze; 

    private PlayerHand playerHand;

    private Dictionary<Attributes, float> multipliers = new Dictionary<Attributes, float>();

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
        Destroy(old);
    }

    public void SetHand(PlayerHand hand){
        playerHand = hand;
    }

    void OnMouseDown(){
        if(playerHand == null) return;

        playerHand.Activate(this);
    }

    public void AddMultiplier(Attributes att, float value){
        if(multipliers.ContainsKey(att)){
            multipliers[att] = multipliers[att] * value;
        } else multipliers[att] = value;
    }

    public string GetName(){
        return face.title;
    }

    public float GetBeauty(){
        float mult = multipliers.ContainsKey(Attributes.Beauty) ? multipliers[Attributes.Beauty] : 1f;
        return mult * (face.beauty + background.beauty + material.beauty + glaze.beauty);
    }

    public float GetStrength(){
        float mult = multipliers.ContainsKey(Attributes.Strength) ? multipliers[Attributes.Strength] : 1f;
        return mult * (face.strength + background.strength + material.strength + glaze.strength);
    }

    public float GetStamina(){
        float mult = multipliers.ContainsKey(Attributes.Stamina) ? multipliers[Attributes.Stamina] : 1f;
        return mult * (face.stamina + background.stamina + material.stamina + glaze.stamina);
    }

    public float GetMagic(){
        float mult = multipliers.ContainsKey(Attributes.Magic) ? multipliers[Attributes.Magic] : 1f;
        return mult * (face.magic + background.magic + material.magic + glaze.magic);
    }

    public float GetSpeed(){
        float mult = multipliers.ContainsKey(Attributes.Speed) ? multipliers[Attributes.Speed] : 1f;
        return mult * (face.speed + background.speed + material.speed + glaze.speed);
    }


    
}
