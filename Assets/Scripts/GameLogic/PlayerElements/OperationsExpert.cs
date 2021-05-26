using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationsExpert : Role
{
    public OperationsExpert(){
        id = Vals.OPERATIONS_EXPERT;
        name = Vals.ROLES[id];
    }

    public override bool buildAction(Player player){
        Debug.Log("ops build");
        return true;
    }

    public override bool nonStandardMove(Player player){
        if(player.getLocation().getResearchStationStatus()){
            return true;
        }
        return false;
    }

}
