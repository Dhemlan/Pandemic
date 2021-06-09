using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    public BoardUI boardUI;
    public CardUI cardUI;
    public OverlayUI overlayUI;
    public GameFlowManager gameFlowManager;
    public PlayerManager playerManager;
    
    private Stack<InfectionCard> infectionDeck = new Stack<InfectionCard>();
    private List<InfectionCard> infectionDiscardPile = new List<InfectionCard>();
    private Stack<InfectionCard> epidemicInfectionCards = new Stack<InfectionCard>();
    
    private Stack<PlayerCard> playerDeck = new Stack<PlayerCard>();
    private List<PlayerCard> playerDiscardPile = new List<PlayerCard>();
    private List<PlayerCard> availableEventCards = new List<PlayerCard>();
     
    private List<Location> outbreakCitiesThisMove = new List<Location>();
    private Queue<Location> outbreaksToResolve = new Queue<Location>();

    private int infectionRateTrackIndex = 0;
    private int[] infectionRateTrack = Vals.INFECTION_RATE_TRACK;
    private int outbreakCount = 0;
    private int[] diseaseCubeSupply = new int[] {Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT};
    private int[] cardsToCure = new int[] {Vals.DEFAULT_CARDS_TO_CURE, Vals.DEFAULT_CARDS_TO_CURE, Vals.DEFAULT_CARDS_TO_CURE, Vals.DEFAULT_CARDS_TO_CURE};
    private int[] diseaseStatus = new int[] {Vals.DISEASE_UNCURED,Vals.DISEASE_UNCURED,Vals.DISEASE_UNCURED,Vals.DISEASE_UNCURED};
    private int researchStationSupply = Vals.DEFAULT_RESEARCH_STATION_SUPPLY;

    public IEnumerator boardSetUp(int epidemicCardCount){
        BoardGenerator generator = gameObject.GetComponent<BoardGenerator>();
        yield return StartCoroutine(generator.generateBoard(this, infectionDeck, epidemicInfectionCards, epidemicCardCount));
        //yield return StartCoroutine(infectCities());
        playerManager.generateCharacters(4);
        generator.preparePlayerCards(playerDeck, playerManager, availableEventCards);
        boardUI.boardSetUp(playerDeck.Count);
        overlayUI.gatherColliders(GetComponentsInChildren<CircleCollider2D>(), GetComponentsInChildren<BoxCollider2D>());
        yield break;
    }

    public IEnumerator drawPhase(){
        List<PlayerCard> drawnCards = new List<PlayerCard>();
            try {
                drawnCards.Add(playerDeck.Pop()); 
                drawnCards.Add(playerDeck.Pop());
            }
            catch(InvalidOperationException){
                gameFlowManager.gameOver(Vals.GAME_OVER_CARDS);
            } 
            boardUI.setPlayerDeckCount(playerDeck.Count);                   
            yield return StartCoroutine(cardUI.playerDraw(playerManager.getCurPlayer(), drawnCards[0], drawnCards[1]));
            
            foreach (PlayerCard drawn in drawnCards){
                Debug.Log("Player draws " + drawn.getName());
                if(drawn.getId() == Vals.EPIDEMIC){
                    // Increase Step
                    infectionRateTrackIndex++;
                    boardUI.advanceInfectionRateTrack();
                    // Infect Step
                    InfectionCard epidemicDrawn = epidemicInfectionCards.Pop();
                    Location loc = epidemicDrawn.getLocation();
                    infectionDiscardPile.Add(epidemicDrawn);
                    yield return StartCoroutine(cardUI.infectionDraw(loc.getName(), loc.getColour()));
                    if (diseaseStatus[(int)loc.getColour()] < Vals.DISEASE_ERADICATED){
                        for (int j = 0; j < Vals.CUBES_PER_EPIDEMIC_INFECT; j++){
                            yield return StartCoroutine(addCube(loc, loc.getColour()));
                        }
                    }
                    outbreakCitiesThisMove.Clear();
                    //Intensify Step
                    Utils.ShuffleAndPlaceOnTop(infectionDiscardPile, infectionDeck);
                }
                else {
                    if (drawn.getColour() == Vals.Colour.EVENT){
                        availableEventCards.Add(drawn);
                        Debug.Log("Event drawn");
                    }
                    playerManager.drawPhaseAdd(drawn);
                }
            }
            yield return StartCoroutine(playerManager.checkHandLimit(playerManager.getCurPlayer()));
    }

    public IEnumerator infectionPhase(){
        for (int i = 0; i < infectionRateTrack[infectionRateTrackIndex]; i++){
            InfectionCard drawn = infectionDeck.Pop(); 
            Debug.Log("Infection: " + drawn.getName());
            Location loc = drawn.getLocation();
            infectionDiscardPile.Add(drawn);
            yield return StartCoroutine(cardUI.infectionDraw(drawn.getName(), loc.getColour()));
            if (diseaseStatus[(int)loc.getColour()] < Vals.DISEASE_ERADICATED){
                yield return StartCoroutine(addCube(loc, loc.getColour()));
            }
            outbreakCitiesThisMove.Clear();
        }
    }

    public IEnumerator addCube(Location loc, Vals.Colour colour){
        if(loc.qSpecialistHere()){
            yield break;
        }
        foreach(Location neighbour in loc.getNeighbours()){
            if(neighbour.qSpecialistHere()){
                yield break;
            }
        }

        if (loc.checkOutbreak(colour)){
            yield return StartCoroutine(outbreakOccurs(loc, colour));
            Debug.Log("Outbreak identified - cube not added");
        }
        else {
            loc.addCube(colour);
            Debug.Log("Adding cube in " + loc.getName());
            diseaseCubeSupply[(int)colour]--;
            boardUI.setCubeCount(colour, diseaseCubeSupply[(int)colour]);
            
            if (diseaseCubeSupply[(int)colour] == 0){
                Debug.Log("out of " + colour);
                gameFlowManager.gameOver(Vals.GAME_OVER_CUBES);       
            }
            yield return StartCoroutine(boardUI.addCube(loc, colour));
        }
        yield break;
    }

    public bool removeCube(Location loc, Vals.Colour colour){
        if (loc.removeCube()){
            diseaseCubeSupply[(int)colour]++;
            if(diseaseStatus[(int)colour] == Vals.DISEASE_CURED && diseaseCubeSupply[(int)colour] == Vals.INITIAL_DISEASE_CUBE_COUNT){
                eradicateDisease(colour);
            }
            Debug.Log("Removing cube in " + loc.getName());
            boardUI.setCubeCount(colour, diseaseCubeSupply[(int)colour]);
            boardUI.removeCube(loc, colour);
            return true;
        }
        Debug.Log("no cubes to remove");
        return false;
    }

    public void removeCubes(Location loc, Vals.Colour colour){
        while (removeCube(loc, colour)){};
    }
    
    public IEnumerator outbreakOccurs(Location loc, Vals.Colour cubeColour){
        if (!outbreakCitiesThisMove.Contains(loc)){
            outbreakCount++;
            Debug.Log("Outbreak #" + outbreakCount + " in " + loc.getName());
            boardUI.increaseOutbreakCounter(outbreakCount);
            if (outbreakCount == Vals.OUTBREAK_COUNT_LIMIT) {
                gameFlowManager.gameOver(Vals.GAME_OVER_OUTBREAKS);
                yield break;         
            }
            outbreakCitiesThisMove.Add(loc);
            foreach (Location neighbour in loc.getNeighbours()){
                yield return StartCoroutine(addCube(neighbour, cubeColour));
                Debug.Log("Request to add cube in " + neighbour.getName());
            }
        }
        yield break;
    }

    public int[] retrieveCureRequirements(){
        return cardsToCure;
    }

    public IEnumerator cure(int numberOfCardsRequired, Player player, Vals.Colour colourToDiscard){
        List<PlayerCard> discarded = new List<PlayerCard>();
        yield return StartCoroutine(overlayUI.requestSelectableFromPlayer(player.getHand(), discarded, Vals.SELECTABLE_PLAYER_CARD, numberOfCardsRequired, colourToDiscard));
        diseaseStatus[(int)colourToDiscard] = Vals.DISEASE_CURED;
        boardUI.diseaseCured(colourToDiscard);
        if(diseaseCubeSupply[(int)colourToDiscard] == Vals.INITIAL_DISEASE_CUBE_COUNT){
            eradicateDisease(colourToDiscard);
        }
        playerManager.discardCards(player, discarded);
        playerManager.updateHand(player);
        playerManager.incrementCompletedActions();
        checkCureVictoryCondition();
    }

    public void discardCard(PlayerCard card){
        playerDiscardPile.Add(card);
    }

    public IEnumerator displayPlayerDiscard(){
        string message = "Player discard pile";
        yield return StartCoroutine(overlayUI.displayItemsUntilClosed(playerDiscardPile, Vals.PLAYER_CARD, message));
    }

    public IEnumerator displayInfectionDiscard(){
        string message = "Infection discard pile";
        yield return StartCoroutine(overlayUI.displayItemsUntilClosed(infectionDiscardPile, Vals.INFECTION_CARD, message));
    }

    public IEnumerator displayAvailableEvents(){
        string message = "Available event cards";
        yield return StartCoroutine(overlayUI.displayItemsUntilClosed(availableEventCards, Vals.EVENT_CARD, message));
        
    }

    public IEnumerator infectCities(){
        for (int i = Vals.INITIAL_INFECTION_ROUNDS; i > 0; i--){
            for (int j = 0; j < Vals.CARDS_PER_INITIAL_INFECTION_ROUND; j++){
                InfectionCard drawn = infectionDeck.Pop();
                Location loc = drawn.getLocation();
                infectionDiscardPile.Add(drawn);
                yield return StartCoroutine(cardUI.infectionDraw(drawn.getName(), loc.getColour()));
                
                Debug.Log("Initial infection in " + drawn.getName());
                for (int k = 0; k < i; k++){          
                    yield return StartCoroutine(addCube(loc, loc.getColour()));
                }             
            }
        }
    }

    public bool isDiseaseCured(Vals.Colour colour){
        Debug.Log("checking disease " + (int)colour);
        return diseaseStatus[(int)colour] > 0 ? true : false;
    }

    public IEnumerator buildResearchStation(Location loc){
        if (researchStationSupply == 0){
            Debug.Log("Remove a station");
            Vals.removeResearchStation = true;
            yield return new WaitUntil(() => !Vals.removeResearchStation);  
            //boardUI.toggleResearchStation()
            researchStationSupply++;

        }
        loc.buildResearchStation();
        boardUI.toggleResearchStation(loc);
        researchStationSupply--;
        Debug.Log("research stations in supply "  + researchStationSupply);
        yield break;
    }

    public void promptDrawPhase(){
        boardUI.activateDrawButton();
    }

    public void eventCardDrawn(PlayerCard card){
        availableEventCards.Add(card);
    }

    public void eventCardPlayed(PlayerCard eventCard){
        availableEventCards.Remove(eventCard);
        if (!eventCard.getRemoveAfterPlaying()){
            playerDiscardPile.Add(eventCard);
        }
        if(!playerManager.removeCardFromHand(eventCard, false)){
            playerManager.storedEventCardPlayed();
        };
    }

    private void eradicateDisease(Vals.Colour colour){
        diseaseStatus[(int)colour] = Vals.DISEASE_ERADICATED;
        boardUI.diseaseEradicated(colour);
    }

    private void checkCureVictoryCondition(){
        for (int i = 0; i < diseaseStatus.Length; i++){
            if(diseaseStatus[i] == Vals.DISEASE_UNCURED){
                return;
            }
        }
        gameFlowManager.gameOver(Vals.GAME_OVER_CURES);
    }

    public void removeInfectionCardFromDiscard(InfectionCard toRemove){
        infectionDiscardPile.Remove(toRemove);
    }

    public List<InfectionCard> getInfectionDiscardPile(){
        return infectionDiscardPile;
    }

    public List<PlayerCard> getEventsInDiscard(){
        List<PlayerCard> playerCards = new List<PlayerCard>();
        foreach(PlayerCard card in playerDiscardPile){
            if (card.getColour() == Vals.Colour.EVENT){
                playerCards.Add(card);
            }
        }
        return playerCards;
    }

    public void removePlayerCardFromDiscard(PlayerCard toRemove){
        playerDiscardPile.Remove(toRemove);
    }

    public Stack<InfectionCard> getInfectionDeck(){
        return infectionDeck;
    }

}
