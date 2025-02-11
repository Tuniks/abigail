using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArgumentCollection", menuName = "Custom/ArgumentCollection")]
public class ArgumentCollection : ScriptableObject{
    public Argument[] argumentCollection = new Argument[0];

    public List<Argument> GetOverlap(Argument[] other){
        List<Argument> match = new List<Argument>();
        
        foreach(Argument arg in argumentCollection){
            foreach(Argument arg2 in other){
                if(arg.IsAMatch(arg2)) match.Add(arg2);
            }
        }

        return match;
    }
}
