using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArgumentCause{
    Silence,
    Face,
    Tag,
    Attribute,
}

[CreateAssetMenu(fileName = "Argument", menuName = "Custom/Argument")]
public class Argument : ScriptableObject{
    [Header("Cause")]
    public ArgumentCause cause;
    public GameObject facePrefab;
    public Tag tag;
    public Attributes attribute;

    [Header("Dialogue")]
    public string[] justificationLines = new string[0];
    public string[] argumentationLines = new string[0];
    public string[] judgeResponseLines = new string[0];

    [Header("Effect")]
    public bool isDefensive = true;
    public Attributes[] targetAttributes;
    public float multiplier;

    public bool IsAMatch(Argument other){
        if(other == null) return false;

        if(cause != other.cause) return false;

        if(cause == ArgumentCause.Face){
            if(facePrefab.name == other.facePrefab.name) return true;
        } else if(cause == ArgumentCause.Tag){
            if(tag == other.tag) return true;
        } else if(cause == ArgumentCause.Attribute){
            if(attribute == other.attribute) return true;
        } else if(cause == ArgumentCause.Silence){
            return true;
        }

        return false;
    }

    public bool IsRelevant(List<Attributes> attributes, Tile tile){
        if(cause == ArgumentCause.Face){
            if(facePrefab.name == tile.GetName()) return true;
        } else if(cause == ArgumentCause.Tag){
            if(tile.HasTag(tag)) return true;
        } else if(cause == ArgumentCause.Attribute){
            if(attributes.Contains(attribute)) return true;
        } else {
            return true;
        }

        return false;
    }

    public string GetJustificationLine(){
        return justificationLines[Random.Range(0, justificationLines.Length)];
    }

    public string GetArgumentationLine(){
        return argumentationLines[Random.Range(0, argumentationLines.Length)];
    }

    public string GetArgumentationResponse(){
        return judgeResponseLines[Random.Range(0, judgeResponseLines.Length)];
    }

}
