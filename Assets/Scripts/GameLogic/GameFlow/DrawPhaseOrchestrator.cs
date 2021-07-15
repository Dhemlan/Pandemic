using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPhaseOrchestrator : MonoBehaviour
{
    public OutbreakHandler outbreakHandler;
    public Board board;
    public PlayerManager playerManager;
    public CardUI cardUI;
    public BoardUI boardUI;
    public EpidemicCityInfector epidemicCityInfector;

    public IEnumerator drawPhase(){
        List<PlayerCard> drawnCards = new List<PlayerCard>();
        yield return StartCoroutine(drawCards(drawnCards));
        yield return StartCoroutine(resolveCards(drawnCards));
        yield return StartCoroutine(playerManager.checkHandLimit(playerManager.getCurPlayer()));
    }

    public IEnumerator drawCards(List<PlayerCard> drawnCards){
        for (int i = 0; i < Vals.DRAW_PHASE_CARD_DRAW_COUNT; i++){
            drawnCards.Add(board.drawPlayerCard());
        }
        // fix
        boardUI.setPlayerDeckCount(board.PlayerDeck.Count);                   
        yield return StartCoroutine(cardUI.playerDraw(playerManager.getCurPlayer(), drawnCards[0], drawnCards[1]));
    }

    public IEnumerator resolveCards(List<PlayerCard> drawnCards){
        foreach (PlayerCard drawn in drawnCards){
            Debug.Log("Player draws " + drawn.Name);
            if(drawn.ID == Vals.EPIDEMIC){
               yield return StartCoroutine(resolveEpidemic());
            }
            else {
                if (drawn.Colour == Vals.Colour.EVENT){
                    // fix
                    board.eventCardDrawn(drawn); 
                }
                playerManager.drawPhaseAdd(drawn);
            }
        }
    }

    public IEnumerator resolveEpidemic(){
        increaseStep();
        yield return StartCoroutine(infectStep());
        yield return StartCoroutine(intensifyStep());
        
    }

    public void increaseStep(){
        board.InfectionRateTrackIndex++;
        boardUI.advanceInfectionRateTrack();
    }

    public IEnumerator infectStep(){
        InfectionCard epidemicDrawn = epidemicCityInfector.epidemicDraw();
        Location loc = epidemicDrawn.Location;
        yield return StartCoroutine(cardUI.infectionDraw(loc.retrieveLocName(), loc.Colour));

        if (!board.isEradicated(epidemicDrawn.Colour)){
            yield return StartCoroutine(epidemicCityInfector.epidemicInfection(epidemicDrawn));
            yield return StartCoroutine(outbreakHandler.resolveAllOutbreaks());
        }
    }

    public IEnumerator intensifyStep(){
        // fix
        if (board.specificEventAvailable("intensify")){
            // fix
            board.requestUserAcceptContinue(Strings.CONTINUE_INTENSIFY);
            yield return new WaitUntil(() => Vals.continueGameFlow);
        }
        board.intensifyEpidemicStep();
    }
}
