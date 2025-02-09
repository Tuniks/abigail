using System.Collections.Generic;
using UnityEngine;


// ArgumentResponse is a data structure that will be used in the argument
// Response is the text, where $name$ will be inserted the name of the card face
// isPositive is used to determine if the response will be used when the player is currently winning or losing
// Multiplier is the value that will multiply the attribute if the player chooses that answer
public struct ArgumentResponse {
    public string response;
    public bool isPositive;
    public float multiplier;

    public ArgumentResponse(string _response, bool _isPositive, float _multiplier = 2f){
        this.response = _response;
        this.isPositive = _isPositive;
        this.multiplier = _multiplier;
    }
}

static public class Arguments {
    private static Dictionary<Attributes, ArgumentResponse[]> ResponsesForAttributes = new Dictionary<Attributes, ArgumentResponse[]>(){
        // Responses for Beauty
        {Attributes.Beauty, new ArgumentResponse[]{
            new ArgumentResponse("But $name$ is so pretty!", false),
            new ArgumentResponse("Cmon, how can you not love $name$?", false),
            new ArgumentResponse("Of course, I would marry my $name$.", true),
            new ArgumentResponse("Nothing can beat the allure of $name$.", true),
        }},
        
        // Responses for Vigor
        {Attributes.Vigor, new ArgumentResponse[]{
            new ArgumentResponse("But $name$ is way stronger!", false),
            new ArgumentResponse("No way, $name$ is so powerful!", false),
            new ArgumentResponse("Of course, look at my $name$'s strength!", true),
            new ArgumentResponse("What can I say, $name$ is a sturdy one!", true),
        }},

        // Responses for Magic
        {Attributes.Magic, new ArgumentResponse[]{
            new ArgumentResponse("But $name$ is so magical!", false),
            new ArgumentResponse("Cmon, aren't you enchanted by my $name$?", false),
            new ArgumentResponse("Nothing can beat my $name$'s incantations!", true),
            new ArgumentResponse("The hypnotic qualities of $name$ always win.", true),
        }},

        // Responses for Heart
        {Attributes.Heart, new ArgumentResponse[]{
            new ArgumentResponse("TEMP_RESPONSE_HEART_NEG", false),
            new ArgumentResponse("TEMP_RESPONSE_HEART_POS", true),
        }},

        // Responses for Intellect
        {Attributes.Intellect, new ArgumentResponse[]{
            new ArgumentResponse("TEMP_RESPONSE_INTELLECT_NEG", false),
            new ArgumentResponse("TEMP_RESPONSE_INTELLECT_POS", true),
        }},

        // Responses for Terror
        {Attributes.Terror, new ArgumentResponse[]{
            new ArgumentResponse("TEMP_RESPONSE_TERROR_NEG", false),
            new ArgumentResponse("TEMP_RESPONSE_TERROR_POS", true),
        }},
    };

    public static (Attributes, string, float) GetRandomResponseFromAttribute(Attributes att, bool isPositive){
        List<ArgumentResponse> possibleResponses = new List<ArgumentResponse>();


        ArgumentResponse[] allResponses =  ResponsesForAttributes[att];
        foreach (ArgumentResponse r in allResponses){
            if(r.isPositive == isPositive) possibleResponses.Add(r);
        }

        ArgumentResponse arg = possibleResponses[Random.Range(0, possibleResponses.Count)];
        return (att, arg.response, arg.multiplier);
    }
}
