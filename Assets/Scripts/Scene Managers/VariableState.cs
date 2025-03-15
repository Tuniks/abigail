using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class VariableState : MonoBehaviour{
    private DialogueRunner dialogueRunner;
    private InMemoryVariableStorage variableStore;

    private Dictionary<string, float> floatVariables = new Dictionary<string, float>();
    private Dictionary<string, string> stringVariables = new Dictionary<string,string>(){
        {"$tile_type", "abc"},
    };
    private Dictionary<string, bool> boolVariables = new Dictionary<string,bool>();
    
    void Start(){
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        variableStore = FindAnyObjectByType<InMemoryVariableStorage>();
    }

    [YarnCommand]
    public void LoadVariables(){
        Debug.Log(stringVariables["$tile_type"]);

        variableStore.SetAllVariables(
            floatVariables,
            stringVariables,
            boolVariables
        );
    }

    [YarnCommand]
    public void StoreVariables(){
        var variables = variableStore.GetAllVariables();
        floatVariables = variables.Item1;
        stringVariables = variables.Item2;
        boolVariables = variables.Item3;
        Debug.Log(variables.Item2["$tile_type"]);
    }
}
