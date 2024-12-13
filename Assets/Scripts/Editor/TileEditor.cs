using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor{
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        Tile tile = (Tile)target;

        if(GUILayout.Button("Generate Tile")){
            tile.OnGeneratePressed();
        }
    }
}
