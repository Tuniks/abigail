using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributePowerPair {
    public Attributes attribute;
    public MonoBehaviour powerScript; // Must implement ITilePower
}

[RequireComponent(typeof(Tile))]
public class TilePowerManager : MonoBehaviour {
    public List<AttributePowerPair> powerAssignments;
    public ITilePower assignedPower;

    void Awake() {
        Tile tile = GetComponent<Tile>();
        if (tile == null || tile.isEnemy) return; // ✅ Skip assigning power to enemy tiles

        // Find the strongest attribute
        float maxValue = 0f;
        Attributes strongest = Attributes.Beauty;

        foreach (Attributes att in System.Enum.GetValues(typeof(Attributes))) {
            float value = tile.GetAttribute(att);
            if (value > maxValue) {
                maxValue = value;
                strongest = att;
            }
        }

        // Look up matching power from list
        foreach (AttributePowerPair pair in powerAssignments) {
            if (pair.attribute == strongest && pair.powerScript is ITilePower power) {
                assignedPower = power;
                tile.tilePower = power;
                return;
            }
        }

        // Fallback if none matched
        if (powerAssignments.Count > 0 && powerAssignments[0].powerScript is ITilePower defaultPower) {
            assignedPower = defaultPower;
            tile.tilePower = defaultPower;
        }
    }
}