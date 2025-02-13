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
    Mystical,
    Clothing,
    Surreal,
    Trash,
    Insect,
    Blue,
    Red,
    Grey,
}

public class TileComponent : MonoBehaviour{
    public float beauty = 0f;
    public float vigor = 0f;
    public float magic = 0f;
    public float heart = 0f;
    public float intellect = 0f;
    public float terror = 0f;

    public string title = "";
    public Tag[] tags;
}
