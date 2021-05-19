using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    public BoardUI boardUI;
    public CardUI cardUI;
    public GameFlowManager gameFlowManager;
    public PlayerManager playerManager;
    
    private Stack<InfectionCard> infectionDeck = new Stack<InfectionCard>();
    private List<InfectionCard> infectionDiscardPile = new List<InfectionCard>();
    private Stack<InfectionCard> EpidemicInfectionCards = new Stack<InfectionCard>();

    private Stack<PlayerCard> playerDeck = new Stack<PlayerCard>();
    private List<PlayerCard> playerDiscardPile = new List<PlayerCard>();
    private List<PlayerCard> eventCardsInHands = new List<PlayerCard>();
    
    private Location[] locations;
    private GameObject[] locObjects;
    private int[][] neighbours = Vals.locNeighbours;
    private List<int> outbreakCitiesThisMove = new List<int>();

    private int infectionRateTrackIndex = 0;
    private int[] infectionRateTrack = new int[] {2,2,2,3,3,4,4};
    private int epidemicCardCount = 6;
    private int outbreakCount = 0;
    private int[] diseaseCubeSupply = new int[] {Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT};
    private int[] cardsToCure = new int[] {Vals.DEFAULT_CARDS_TO_CURE, Vals.DEFAULT_CARDS_TO_CURE, Vals.DEFAULT_CARDS_TO_CURE, Vals.DEFAULT_CARDS_TO_CURE};
    private int[] diseaseStatus = new int[] {1,1,1,0};


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
                    InfectionCard epidemicDrawn = EpidemicInfectionCards.Pop();
                    Location loc = locations[epidemicDrawn.getId()];
                    infectionDiscardPile.Add(epidemicDrawn);
                    yield return StartCoroutine(cardUI.infectionDraw(loc.getName(), loc.getColour()));
                    if (diseaseStatus[(int)loc.getColour()] < Vals.DISEASE_ERADICATED){
                        for (int j = 0; j < Vals.CUBES_PER_EPIDEMIC_INFECT; j++){
                            yield return StartCoroutine(addCube(loc, loc.getColour(), epidemicDrawn.getId()));
                        }
                    }
                    outbreakCitiesThisMove.Clear();

                    //Intensify Step
                    Utils.ShuffleAndPlaceOnTop(infectionDiscardPile, infectionDeck);
                }
                else {
                    playerManager.drawPhaseAdd(drawn);
                }
            }
            yield return StartCoroutine(playerManager.checkHandLimit(playerManager.getCurPlayer()));
    }

    public IEnumerator infectionPhase(){
        for (int i = 0; i < infectionRateTrack[infectionRateTrackIndex]; i++){
            InfectionCard drawn = infectionDeck.Pop(); 
            Debug.Log("Infection: " + drawn.getName());
            Location loc = locations[drawn.getId()];
            infectionDiscardPile.Add(drawn);
            yield return StartCoroutine(cardUI.infectionDraw(drawn.getName(), loc.getColour()));
            if (diseaseStatus[(int)loc.getColour()] < Vals.DISEASE_ERADICATED){
                yield return StartCoroutine(addCube(loc, loc.getColour(), drawn.getId()));
            }
            outbreakCitiesThisMove.Clear();
        }
        playerManager.endPlayerTurn();
    }

    public IEnumerator addCube(Location loc, Vals.Colour colour, int locId){
        if (loc.checkOutbreak(colour)){
            yield return StartCoroutine(outbreakOccurs(loc, colour, locId));
            Debug.Log("Outbreak identified - cube not added");
        }
        else {
            loc.addCube(colour);
            Debug.Log("Adding cube in " + loc.getName());
            diseaseCubeSupply[(int)colour]--;
            boardUI.setCubeCount(colour, diseaseCubeSupply[(int)colour]);
            
            if (diseaseCubeSupply[(int)colour] == 0){
                Debug.Log("out of " + colour);
                //gameFlowManager.gameOver(Vals.GAME_OVER_CUBES);       
            }
            yield return StartCoroutine(boardUI.addCube(loc, colour));
        }
        yield break;
    }

    public bool removeCube(Location loc){
        Vals.Colour colour = loc.getColour();
        if (loc.removeCube()){
            diseaseCubeSupply[(int)colour]++;
            if(diseaseCubeSupply[(int)colour] == Vals.INITIAL_DISEASE_CUBE_COUNT){
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

    public void removeCubes(Location loc){
        while (removeCube(loc)){};
    }
    
    public IEnumerator outbreakOccurs(Location loc, Vals.Colour cubeColour, int locId){
        if (!outbreakCitiesThisMove.Contains(locId)){
            outbreakCount++;
            Debug.Log("Outbreak #" + outbreakCount + " in " + loc.getName());
            boardUI.increaseOutbreakCounter(outbreakCount);
            if (outbreakCount == Vals.OUTBREAK_COUNT_LIMIT) {
                gameFlowManager.gameOver(Vals.GAME_OVER_OUTBREAKS);
                yield break;
               // Environment.Exit(0);            
            }
            outbreakCitiesThisMove.Add(locId);
            for (int i = 0; i < neighbours[locId].Length; i++){
                int newLoc = neighbours[locId][i];
                yield return StartCoroutine(addCube(locations[newLoc], cubeColour, newLoc));
                Debug.Log("Request to add cube in " + locations[newLoc].getName());
            }
        }
        yield break;
    }

    public int[] retrieveCureRequirements(){
        return cardsToCure;
    }

    public IEnumerator cure(int numberOfCardsRequired, Player player, Vals.Colour colourToDiscard){
        List<PlayerCard> discarded = new List<PlayerCard>();
        yield return StartCoroutine(cardUI.allowSelectionToDiscard(player.getHand(), discarded, numberOfCardsRequired, colourToDiscard));
        diseaseStatus[(int)colourToDiscard] = Vals.DISEASE_CURED;
        boardUI.diseaseCured(colourToDiscard);
        if(diseaseCubeSupply[(int)colourToDiscard] == Vals.INITIAL_DISEASE_CUBE_COUNT){
            eradicateDisease(colourToDiscard);
        }
        player.discardCards(discarded);
        playerManager.updateHand(player);
        playerManager.incrementCompletedActions();
        checkCureVictoryCondition();
    }

    public void discardCard(PlayerCard card){
        playerDiscardPile.Add(card);
    }

    public IEnumerator displayPlayerDiscard(){
        List<GameObject> displayedCards = new List<GameObject>();
        string message = "Player discard pile";
        cardUI.displayInteractableCards(playerDiscardPile, displayedCards, message);
        yield return new WaitForSeconds(2.0f);
        cardUI.clearCards(displayedCards);

    }

    public void boardSetUp(){
        setUpLocations();
        generateDecks();
        locations[Vals.ATLANTA].buildResearchStation();
        locations[Vals.BANGKOK].buildResearchStation();
        
    }

    private void generateDecks(){
        List<PlayerCard> cityCards = new List<PlayerCard>();
        List<InfectionCard> infectionCards = new List<InfectionCard>();
        for (int i = 0; i < locations.Length; i++){
            infectionCards.Add(new InfectionCard(locations[i], i, locations[i].gameObject.name));
            cityCards.Add(new PlayerCard(locations[i], i, locations[i].gameObject.name));
        }
        createInfectionDeck(infectionCards);
        generatePlayerDeck(cityCards);
    }

    public void createInfectionDeck(List<InfectionCard> infectionCards){
        Utils.ShuffleAndPlaceOnTop(infectionCards, infectionDeck);
        // Set aside "bottom" cards for epidemic draws
        for (int i = 0; i < epidemicCardCount; i++){
            EpidemicInfectionCards.Push(infectionDeck.Pop());
        }
    }

    public void generatePlayerDeck(List<PlayerCard> cityCards){
        // add event cards here
        Stack<PlayerCard> shuffledPlayerCards = new Stack<PlayerCard>();
        Utils.ShuffleAndPlaceOnTop(cityCards, shuffledPlayerCards);
        int cardsRequired = playerManager.getInitialHandCount();
        List<PlayerCard> initialHandCards = new List<PlayerCard>();
        for (int i = 0; i < cardsRequired; i++){
            initialHandCards.Add(shuffledPlayerCards.Pop());
        }
        playerManager.setInitialHands(initialHandCards);
        createPlayerDeck(shuffledPlayerCards);
    }

    public void createPlayerDeck(Stack<PlayerCard> shuffledPlayerCards){
            
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
        boardUI.setPlayerDeckCount(playerDeck.Count);
    }

    private void setUpLocations(){
        locations = GameObject.Find("Locations").GetComponentsInChildren<Location>();
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
                    yield return StartCoroutine(addCube(loc, loc.getColour(), drawn.getId()));
                }             
            }
        }
    }

    public bool isDiseaseCured(Vals.Colour colour){
        Debug.Log("checking disease " + (int)colour);
        return diseaseStatus[(int)colour] > 0 ? true : false;
    }

    public void buildResearchStation(Location loc){
        loc.buildResearchStation();
        boardUI.buildResearchStation();
    }

    public void promptDrawPhase(){
        boardUI.activateDrawButton();
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

}
