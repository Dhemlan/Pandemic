using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    private Location[] locations;
    private int epidemicCardCount;

    public PlayerManager playerManager;
    
    public IEnumerator generateBoard(Board board, Stack<InfectionCard> infectionDeck, Stack<InfectionCard> epidemicInfectionCards, int epidemicCardCount){
        this.epidemicCardCount = epidemicCardCount;
        setUpLocations();
        yield return StartCoroutine(buildInitialResearchStations(board));
        createInfectionDeck(infectionDeck, epidemicInfectionCards);
    }

    private void setUpLocations(){
        locations = GameObject.Find("Locations").GetComponentsInChildren<Location>();
    }

    private void createInfectionDeck(Stack<InfectionCard> infectionDeck, Stack<InfectionCard> epidemicInfectionCards){
        List<InfectionCard> infectionCards = new List<InfectionCard>();
        for (int i = 0; i < locations.Length; i++){
            infectionCards.Add(new InfectionCard(locations[i], i, locations[i].gameObject.name));
        }
        Utils.ShuffleAndPlaceOnTop(infectionCards, infectionDeck);
        // Set aside "bottom" cards for epidemic draws
        for (int i = 0; i < epidemicCardCount; i++){
            epidemicInfectionCards.Push(infectionDeck.Pop());
        }
    }
    public IEnumerator buildInitialResearchStations(Board board){
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.ATLANTA]));
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.BANGKOK]));
        /*yield return StartCoroutine(board.buildResearchStation(locations[Vals.SYDNEY]));
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.LIMA]));
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.PARIS]));
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.ESSEN]));*/
    }

    public void preparePlayerCards(Stack<PlayerCard> playerDeck, PlayerManager playerManager, List<PlayerCard> eventCards){
        List<PlayerCard> cityCards = new List<PlayerCard>();
        for (int i = 0; i < locations.Length; i++){
            cityCards.Add(new PlayerCard(locations[i], i, locations[i].gameObject.name));
        }

        cityCards.Add(new PlayerCard(null, Vals.ONE_QUIET_NIGHT, "One Quiet Night"));
        cityCards.Add(new PlayerCard(null, Vals.AIRLIFT, "Airlift"));
        cityCards.Add(new PlayerCard(null, Vals.GOVERNMENT_GRANT, "Government Grant"));
        cityCards.Add(new PlayerCard(null, Vals.FORECAST, "Forecast"));
        cityCards.Add(new PlayerCard(null, Vals.RESILIENT_POPULATION, "Resilient Population"));

        Stack<PlayerCard> shuffledPlayerCards = new Stack<PlayerCard>();
        Utils.ShuffleAndPlaceOnTop(cityCards, shuffledPlayerCards);
        dealInitialPlayerHands(shuffledPlayerCards, playerManager, eventCards);
        createPlayerDeck(shuffledPlayerCards, playerDeck);
    }

    public void dealInitialPlayerHands(Stack<PlayerCard> shuffledPlayerCards, PlayerManager playerManager, List<PlayerCard> eventCards){
        int cardsRequired = playerManager.getInitialHandCount();
        List<PlayerCard> initialHandCards = new List<PlayerCard>();
        for (int i = 0; i < cardsRequired; i++){
            PlayerCard drawn = shuffledPlayerCards.Pop();
            initialHandCards.Add(drawn);
            if (drawn.getColour() == Vals.Colour.EVENT){eventCards.Add(drawn);}
        }
        playerManager.setInitialHands(initialHandCards);
    }

    public void createPlayerDeck(Stack<PlayerCard> shuffledPlayerCards, Stack<PlayerCard> playerDeck){
        // Create  and stack epidemic card piles with smallest piles at the bottom
        int decrementingPileCount = epidemicCardCount;
        for (int k = 0; k < epidemicCardCount; k++){
            int pileSize = shuffledPlayerCards.Count / decrementingPileCount;
            List<PlayerCard> curPile = new List<PlayerCard>();
            for (int j = 0; j < pileSize; j++){
                curPile.Add(shuffledPlayerCards.Pop());
            }
            curPile.Add(new PlayerCard(null, Vals.EPIDEMIC, "Epidemic"));
            Utils.ShuffleAndPlaceOnTop(curPile, playerDeck);
            decrementingPileCount--;     
        } 
    }


}
