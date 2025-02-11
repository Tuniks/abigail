using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Argument", menuName = "Custom/Argument")]
public class ArgumentCollection : ScriptableObject{
    public Argument[] argumentCollection = new Argument[0];

    public Argument[] CheckDefensiveOverlap(Argument[] other){
        List<Argument> match = new List<Argument>();
        
        foreach(Argument arg in argumentCollection){
            foreach(Argument arg2 in other){
                if(arg.IsAMatch(arg2)) match.Add(arg2);
            }
        }

        return match.ToArray();
    }
}
