using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : Role
{
    public Dispatcher(){
        id = Vals.DISPATCHER;
        name = Vals.ROLES[id];
    }

    public override bool nonStandardMove(Player player, Location dest){
        Debug.Log("dispatcher move");
        if (dest.getLocalPlayers().Count > 0){
            return true;
        }
        return false;
    }
}
