using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Challenges : MonoBehaviour{
    private Dictionary<int, string> challenges = new Dictionary<int, string>(){
        {0, "Who would win in a fight?"},
        {1, "Who would win in a beauty contest?"},
        {2, "Who would win in a race?"},
        {3, "Who would be the chiller dude at a party?"},
        {4, "Who would be a better lover?"},
    };

    public int[] GetRandomChallenges(int count){
        int[] result = new int[count];
        Dictionary<int, string> currentList = new Dictionary<int, string>(challenges);

        for(int i = 0; i < count; i++){
            result[i] = currentList.ElementAt(UnityEngine.Random.Range(0, currentList.Count)).Key;
            currentList.Remove(result[i]);
        }
        
        return result;
    }

    public string GetChallengeDescription(int id){
        return challenges[id];
    }

    // Calculation for each individual challenge, based on tiles
    public float EvaluateTile(int id, Tile tile){
        float result = 0;
        
        switch(id){
            case 0:
                result = (tile.GetStrength() + tile.GetMagic())/2f;
                break;
            case 1:
                result = tile.GetBeauty();
                break;
            case 2:
                result = (tile.GetSpeed() + tile.GetStamina())/2f;
                break;
            case 3:
                result = (tile.GetBeauty() + tile.GetStamina() + tile.GetMagic())/3f;
                break;
            case 4:
                result = (tile.GetBeauty() + tile.GetStamina())/2f;
                break;
        }

        return result;
    }

    // Returns which attributes are related to each challenge's calculation
    private List<Attributes> GetChallengeAttributes(int id){
        switch(id){
            case 0:
                return new List<Attributes>{Attributes.Strength, Attributes.Magic};
            case 1:
                return new List<Attributes>{Attributes.Beauty};
            case 2:
                return new List<Attributes>{Attributes.Speed, Attributes.Stamina};
            case 3:
                return new List<Attributes>{Attributes.Beauty, Attributes.Magic, Attributes.Stamina};
            case 4:
                return new List<Attributes>{Attributes.Beauty, Attributes.Stamina};
        }

        return new List<Attributes>{};
    }

    // Returns a list of 3 attributes based on id, at most 2 based on the challenge, 1 random
    private List<Attributes> Get3AttributesFromChallenge(int id){
        List<Attributes> attributes = new List<Attributes>();
        
        List<Attributes> challengeAttributes = GetChallengeAttributes(id);
        List<Attributes> allAttributes = new List<Attributes>((Attributes[])Enum.GetValues(typeof(Attributes)));

        int nonrand = 0;
        for(int i = 0; i < 3; i++){
            if(nonrand < 2 && challengeAttributes.Count > 0){
                Attributes att = challengeAttributes[UnityEngine.Random.Range(0, challengeAttributes.Count)];
                challengeAttributes.Remove(att);
                allAttributes.Remove(att);
                attributes.Add(att);
                nonrand++;
            } else {
                Attributes att = allAttributes[UnityEngine.Random.Range(0, allAttributes.Count)];
                allAttributes.Remove(att);
                attributes.Add(att);
            }
        }

        return attributes;
    }

    // Return a list of responses for arguing phase
    public List<(Attributes, string, float)> Get3ResponsesFromChallenge(int id, bool isPositive){
        List<(Attributes, string, float)> responses = new List<(Attributes, string, float)>();
        List<Attributes> attributes = Get3AttributesFromChallenge(id);

        foreach(Attributes att in attributes){
            responses.Add(Arguments.GetRandomResponseFromAttribute(att, isPositive));
        }

        return responses;
    }
}
 