using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpidemicCityInfector : CityInfector
{
    public InfectionCard epidemicDraw(){
        return nonInfectionPhaseInfectionDraw();
    }

    public IEnumerator epidemicInfection(InfectionCard epidemicDraw){
        yield return StartCoroutine(addCubesbyInfectionCard(epidemicDraw, Vals.CUBES_PER_EPIDEMIC_INFECT));
        yield return StartCoroutine(outbreakHandler.resolveAllOutbreaks());
    }
}
