using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpCityInfector : CityInfector
{
    public IEnumerator initialInfection(){
        yield return StartCoroutine(firstRoundInitialInfection());
        yield return StartCoroutine(secondRoundInitialInfection());
        yield return StartCoroutine(thirdRoundInitialInfection());
    }

    protected IEnumerator initialInfectionRound(int cubesPerInfectionCard){
        for(int i = 0; i < Vals.CARDS_PER_INITIAL_INFECTION_ROUND; i++){
            InfectionCard drawn = drawInfectionCard();
            yield return StartCoroutine(addCubesbyInfectionCard(drawn, cubesPerInfectionCard));
        }
    }

    protected IEnumerator firstRoundInitialInfection(){    
        yield return StartCoroutine(initialInfectionRound(Vals.INITIAL_INFECTION_FIRST_PHASE));
    }

    protected IEnumerator secondRoundInitialInfection(){
        yield return StartCoroutine(initialInfectionRound(Vals.INITIAL_INFECTION_SECOND_PHASE));
    }

    protected IEnumerator thirdRoundInitialInfection(){
        yield return StartCoroutine(initialInfectionRound(Vals.INITIAL_INFECTION_THIRD_PHASE));
    }    
}
