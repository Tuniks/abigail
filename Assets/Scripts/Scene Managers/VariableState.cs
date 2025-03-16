using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class VariableState : MonoBehaviour{
    private InMemoryVariableStorage variableStore;

    private Dictionary<string, float> floatVariables = new Dictionary<string, float>();
    private Dictionary<string, string> stringVariables = new Dictionary<string,string>(){
        {"$tile_type", "abc"},
    };
    private Dictionary<string, bool> boolVariables = new Dictionary<string,bool>();

    [YarnCommand]
    public void LoadVariables(){
        variableStore = FindAnyObjectByType<InMemoryVariableStorage>();

        variableStore.SetAllVariables(
            floatVariables,
            stringVariables,
            boolVariables
        );
    }

    [YarnCommand]
    public void StoreVariables(){
        variableStore = FindAnyObjectByType<InMemoryVariableStorage>();

        var variables = variableStore.GetAllVariables();
        floatVariables = variables.Item1;
        stringVariables = variables.Item2;
        boolVariables = variables.Item3;
        Debug.Log(variables.Item2["$tile_type"]);
    }
}
