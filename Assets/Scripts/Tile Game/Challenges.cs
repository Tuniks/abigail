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
        {5, "Who would overtake my nightmares?"},
        {6, "Who would require a piñata at their birthday party?"},
        {7, "Who would worship a star in the sky?"},
        {8, "What says mothers love the most?"},
        {9, "What does a good time look like?"},
        {10, "What did you both miss when you were away?"},
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
        switch(id){
            case 0:
                return (tile.GetVigor() + tile.GetMagic())/2f;
            case 1:
                return (tile.GetBeauty() + tile.GetMagic())/2f;
            case 2:
                return tile.GetVigor();
            case 3:
                return (tile.GetBeauty() + tile.GetMagic() + tile.GetHeart())/3f;
            case 4:
                return (tile.GetBeauty() + tile.GetHeart())/2f;
            case 5:
                return tile.GetTerror();
            case 6:
                return (tile.GetVigor() + tile.GetHeart())/2f;
            case 7:
                return tile.GetMagic();
            case 8:
                return tile.GetHeart();
            case 9:
                return tile.GetHeart();
            case 10:
                return tile.GetHeart();
                
        }

        return 0;
    }

    // Returns which attributes are related to each challenge's calculation
    public List<Attributes> GetChallengeAttributes(int id){
        switch(id){
            case 0:
                return new List<Attributes>{Attributes.Vigor, Attributes.Magic};
            case 1:
                return new List<Attributes>{Attributes.Beauty, Attributes.Magic};
            case 2:
                return new List<Attributes>{Attributes.Vigor};
            case 3:
                return new List<Attributes>{Attributes.Beauty, Attributes.Magic, Attributes.Heart};
            case 4:
                return new List<Attributes>{Attributes.Beauty, Attributes.Heart};
            case 5:
                return new List<Attributes>{Attributes.Terror};
            case 6:
                return new List<Attributes>{Attributes.Vigor, Attributes.Heart};
            case 7:
                return new List<Attributes>{Attributes.Magic};
            case 8:
                return new List<Attributes>{Attributes.Heart};
            case 9:
                return new List<Attributes>{Attributes.Heart};
            case 10:
                return new List<Attributes>{Attributes.Heart};
        }

        return new List<Attributes>{};
    }
}
 