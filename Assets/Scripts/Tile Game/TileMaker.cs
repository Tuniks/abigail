using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour{
    public static TileMaker instance;
    
    public GameObject[] faces;
    public GameObject[] backgrounds;
    public GameObject[] glazes;
    public GameObject[] materials;

    public GameObject tilePrefab;

    void Awake(){
        instance = this;
    }

    public Tile GetRandomTile(){
        GameObject facePrefab = faces[Random.Range(0, faces.Length)];
        GameObject bgPrefab = backgrounds[Random.Range(0, backgrounds.Length)];
        GameObject matPrefab = materials[Random.Range(0, materials.Length)];
        GameObject glzPrefab = glazes[Random.Range(0, glazes.Length)];

        GameObject tile = Instantiate(tilePrefab);

        tile.GetComponent<Tile>().Initialize(
            facePrefab,
            bgPrefab,
            matPrefab,
            glzPrefab
        );

        return tile.GetComponent<Tile>();
    }
}
