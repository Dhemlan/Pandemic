using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard : Card, IComparable<PlayerCard>
{

    public PlayerCard(Location loc, int id, string name){
        this.loc = loc;
        this.id = id;
        this.name = name;
        if (loc != null) colour = loc.getColour();
        else{
            if (id == ConstantVals.EPIDEMIC){
                colour = ConstantVals.Colour.EPIDEMIC;
            }
            else{
                colour = ConstantVals.Colour.EVENT;
            }
        }
    }

    public int CompareTo(PlayerCard o){
        if(colour < o.getColour()){
            return -1;
        }
        else return 1;
    }

}
