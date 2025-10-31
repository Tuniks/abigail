using UnityEngine;
using UnityEngine.UI;

public interface ITilePower {
    public Sprite Icon { get; }
    void Activate(Tile tile);


}