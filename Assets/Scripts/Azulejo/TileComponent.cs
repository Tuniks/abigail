using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes {
    Beauty,
    Vigor,
    Magic,
    Heart,
    Intellect,
    Terror,
}

public enum Tag {
    Animal,
    Aquatic,
    Winged,
    Life,
    Death,
    Flora,
    Tool,
    Food,
    Mystical,
    Clothing,
    Surreal,
    Trash,
    Metal,
    Gem,
    Insect,
    Blue,
    Brown,
    Red,
    Gray,
    Orange,
    Yellow,
    Purple,
    Felt,
    Clay,
    Collage,
}

public class TileComponent : MonoBehaviour{
    public float beauty = 0f;
    public float vigor = 0f;
    public float magic = 0f;
    public float heart = 0f;
    public float intellect = 0f;
    public float terror = 0f;

    public string title = "";
    public string description = "";
    public Tag[] tags;
}
