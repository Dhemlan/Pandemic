using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role
{   
    private int cardsToCure = ConstantVals.DEFAULT_CARDS_TO_CURE;
    private bool researchStationRequiredToCure = true;

    public int getCardsToCure(){
        return cardsToCure;
    }

    public bool getResearchStationRequiredToCure(){
        return researchStationRequiredToCure;
    }

}
