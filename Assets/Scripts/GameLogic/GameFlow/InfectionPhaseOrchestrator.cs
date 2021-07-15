using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionPhaseOrchestrator : MonoBehaviour
{
    public InfectionPhaseCityInfector cityInfector;
    public Board board;
    public CardUI cardUI;
    public OutbreakHandler outbreakHandler;
    
    public IEnumerator infectionPhase(){
        if(skipInfectionPhase()) yield break;
        else{
            int infectionRate = board.currentInfectionRate();    
            for (int i = 0; i < infectionRate; i++){
                if (board.specificEventAvailable("infection")){
                    yield return StartCoroutine(waitForPlayerConfirmation());
                    if (skipInfectionPhase()) yield break;
                }
                InfectionCard drawn = cityInfector.infectionPhaseDraw();
                Debug.Log("Infection: " + drawn.Name);
                yield return StartCoroutine(cardUI.infectionDraw(drawn.Name, drawn.Colour));
                if (!board.isEradicated(drawn.Colour)){
                    yield return StartCoroutine(cityInfector.infectCity(drawn));
                }
                yield return StartCoroutine(outbreakHandler.resolveAllOutbreaks());
            }
        }
    }

    public bool skipInfectionPhase(){
        if (Vals.oneQuietNightActive){    
            Vals.oneQuietNightActive = false;
            return true;
        }
        return false;
    }

    public IEnumerator waitForPlayerConfirmation(){
        // update
        board.requestUserAcceptContinue(Strings.CONTINUE_INFECTION_PHASE);
        yield return new WaitUntil(() => Vals.continueGameFlow);
    }
}
