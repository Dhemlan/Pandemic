using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard : Card, IComparable<PlayerCard>
{
    private bool removeAfterPlayed = false;

    public PlayerCard(Location loc, int id, string name){
        this.Location = loc;
        this.id = id;
        this.Name = name;
        if (loc != null) colour = loc.Colour;
        else{
            if (id == Vals.EPIDEMIC){
                colour = Vals.Colour.EPIDEMIC;
            }
            else{
                colour = Vals.Colour.EVENT;
            }
        }
    }

    public int CompareTo(PlayerCard o){
        if(colour > o.Colour){
            return -1;
        }
        else return 1;
    }

    public void removeAfterPlaying(){
        removeAfterPlayed = true;
    }

    public bool getRemoveAfterPlaying(){
        return removeAfterPlayed;
    }

}
