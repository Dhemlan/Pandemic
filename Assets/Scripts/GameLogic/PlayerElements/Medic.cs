using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : Role
{
    public Medic(){
        id = Vals.MEDIC;
        name = Vals.ROLES[id];
    }

    public override bool treatAction(){
        return true;
    }

    public override void enterLocation(Board board, Location loc){
        for (int i = 0; i < Vals.DISEASE_COUNT; i++){
            if (board.isDiseaseCured((Vals.Colour)i)){
                board.removeCubes(loc, (Vals.Colour)i);
            }
        }
    }
}
