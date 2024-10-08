using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes {
    Beauty,
    Strength,
    Stamina,
    Magic,
    Speed,
}

public class TileComponent : MonoBehaviour{
    public float beauty = 0f;
    public float strength = 0f;
    public float stamina = 0f;
    public float magic = 0f;
    public float speed = 0f;
}
