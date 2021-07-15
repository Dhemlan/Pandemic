using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionCard : Card
{

    public InfectionCard(Location loc, int id, string name){
        this.Location = loc;
        this.id = id;
        this.name = name;
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

    public string toString(){
        return name;
    }

}
