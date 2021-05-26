using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuarantineSpecialist : Role
{
    public QuarantineSpecialist(){
        id = Vals.QUARANTINE_SPECIALIST;
        name = Vals.ROLES[id];
    }
}
