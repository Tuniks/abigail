using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

public class FeltInteractions : AreaInteractions{
    PlayableDirector ryanAnimator;
    
    // Yarn Commands
    [YarnCommand]
    public void RyansThrowFlyTile(){
        ryanAnimator.Play();
    }
}
