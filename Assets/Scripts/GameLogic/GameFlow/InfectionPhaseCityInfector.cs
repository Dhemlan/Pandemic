using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionPhaseCityInfector : CityInfector
{
    public InfectionCard infectionPhaseDraw(){
        return drawInfectionCard();
    }

    public IEnumerator infectCity(InfectionCard drawn){
        yield return StartCoroutine(addCubesbyInfectionCard(drawn, Vals.CUBES_PER_INFECTION_DRAW));
        yield return StartCoroutine(outbreakHandler.resolveAllOutbreaks());
    }
}
