using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardHandler : MonoBehaviour
{
    public Board board;
    public OverlayUI overlayUI;
    public PlayerManager playerManager;
    private List<InfectionCard> clickedCards = new List<InfectionCard>();


    public void eventClicked(PlayerCard card){
        StartCoroutine(handleEventCardClick(card));
    }

    public IEnumerator handleEventCardClick(PlayerCard card){
        //overlayUI.StopAllCoroutines();
        overlayUI.finishedWithDisplayOverlay();
        Vals.continueGameFlow = false;
        switch (card.getID()){
            case Vals.ONE_QUIET_NIGHT:
                if(Vals.oneQuietNightActive) yield break;
                Debug.Log("Playing One Quiet Night");
                Vals.oneQuietNightActive = true;
                Vals.continueGameFlow = true;
                break;
            case Vals.AIRLIFT:
                Debug.Log("Playing Airlift");
                string message = Strings.AIRLIFT;
                Vals.cardResolving = Vals.AIRLIFT;
                yield return StartCoroutine(overlayUI.requestSimpleSelectionFromPlayer(playerManager.getPlayers(), Vals.SELECTABLE_PLAYER, message));
                break;
            case Vals.RESILIENT_POPULATION:
                Debug.Log("Playing Resilient Population");
                if (board.getInfectionDiscardPile().Count > 0){
                    yield return StartCoroutine(resilientPopulation());     
                }
                Vals.continueGameFlow = true;
                break;
            case Vals.FORECAST:
                Debug.Log("Playing Forecast");
                yield return StartCoroutine(forecast());

                Vals.continueGameFlow = true;
                break;
            case Vals.GOVERNMENT_GRANT:
                Debug.Log("Playing Government Grant");
                Vals.cardResolving = Vals.GOVERNMENT_GRANT;
                break;
        }
        board.eventCardPlayed(card);
        overlayUI.closeToast();
        yield break;
    }

    private IEnumerator resilientPopulation(){
        List<InfectionCard> selected = new List<InfectionCard>();
        string message = Strings.RESILIENT_POPULATION;
        yield return StartCoroutine(overlayUI.requestMultiSelect(board.getInfectionDiscardPile(), selected, Vals.SELECTABLE_INFECTION_CARD, 1, null, message));
        board.removeInfectionCardFromDiscard(selected[0]);
    }

    private IEnumerator forecast(){
        Debug.Log("forecast executing");
        List<InfectionCard> top6Cards = new List<InfectionCard>();
        Stack<InfectionCard> infectionDeck = board.getInfectionDeck();
        
        for (int i = 0; i < 6; i++){
            top6Cards.Add(infectionDeck.Pop());
        }
        clickedCards.Clear();
        string message = Strings.FORECAST;
        yield return StartCoroutine(overlayUI.requestMultiSelect(top6Cards, new List<InfectionCard>(), Vals.SELECTABLE_INFECTION_CARD, 6, null, message));

        for (int i = clickedCards.Count; i > 0; i--){
            infectionDeck.Push(clickedCards[i - 1]);
        }
    }

    public void infectionCardClicked(bool active, InfectionCard card){
        if (active){
            clickedCards.Add(card);
        }
        else {
            clickedCards.Remove(card);
        }
    }
}
