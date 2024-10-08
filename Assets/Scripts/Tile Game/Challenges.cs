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
            result[i] = currentList.ElementAt(Random.Range(0, currentList.Count)).Key;
            currentList.Remove(result[i]);
        }
        
        return result;
    }

    public string GetChallengeDescription(int id){
        return challenges[id];
    }

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
}
 