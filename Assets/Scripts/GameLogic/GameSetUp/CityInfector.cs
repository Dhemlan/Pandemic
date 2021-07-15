using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Infects provided location in a manor dependent on the game phase (setup, epidemic draw, infection phase)
public class CityInfector : MonoBehaviour{
    
    public Board board;
    public OutbreakHandler outbreakHandler;

    protected InfectionCard drawInfectionCard(){
        InfectionCard drawn = board.drawInfectionCard();
        board.discardInfectionCard(drawn);
        return drawn;
    }

    protected InfectionCard nonInfectionPhaseInfectionDraw(){
        InfectionCard drawn = board.drawInfectionCardFromBottom();
        board.discardInfectionCard(drawn);
        return drawn;
    }

    public IEnumerator addCubesbyInfectionCard(InfectionCard drawn, int cubesToAdd){
        for (int j = 0; j < cubesToAdd; j++){
            yield return StartCoroutine(board.addCube(drawn));
            yield return new WaitForSeconds(.3f);
        }
    }  
}

/*
Diary
8/7/21
Debating the design of the CityInfector class. Initial, infect and epidemic methods will definitely change over time,
though the intent will be to handle this with more specific CityInfectors via inheritance.
Breaking the class down into additional classes seems to abide by SRP, but will then involve DI violations
(requiring 3 additional interfaces to resolve). Seems like  a lot of needless complexity for something that will be
managed with a different method. 

*/
