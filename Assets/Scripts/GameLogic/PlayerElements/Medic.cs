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
}
