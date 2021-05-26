using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist : Role
{
    public Scientist(){
        id = Vals.SCIENTIST;
        name = Vals.ROLES[id];
        cardsToCure = Vals.DEFAULT_CARDS_TO_CURE - 1;
    }
}
