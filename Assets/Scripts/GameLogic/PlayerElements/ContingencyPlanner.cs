using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContingencyPlanner : Role
{
    private PlayerCard storedEventCard = null;
    
    public ContingencyPlanner(){
        id = Vals.CONTINGENCY_PLANNER;
        name = Vals.ROLES[id];
    }

    public override IEnumerator characterAction<T>(T eventCardToStore){
        Debug.Log("Contingency Planner");
        if (storedEventCard == null){
            storedEventCard = (PlayerCard)(object)eventCardToStore;
            storedEventCard.removeAfterPlaying();
            Debug.Log(storedEventCard.getName() + "retrieved");
        }
        yield break;
    }

    public bool eventCardStored(){
        return storedEventCard == null ? false : true;
    }
  
    public void storedEventCardPlayed(){
        storedEventCard = null;
    }



}
