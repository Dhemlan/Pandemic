using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    public BoardUI boardUI;
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
    private int[][] neighbours = ConstantVals.locNeighbours;
    private List<int> outbreakCitiesThisMove = new List<int>();

    private int infectionRateTrackIndex = 0;
    private int[] infectionRateTrack = new int[] {2,2,2,3,3,4,4};
    private int epidemicCardCount = 6;
    private int outbreakCount = 0;
    private int[] diseaseCubeSupply = new int[] {ConstantVals.INITIAL_DISEASE_CUBE_COUNT,ConstantVals.INITIAL_DISEASE_CUBE_COUNT,ConstantVals.INITIAL_DISEASE_CUBE_COUNT,ConstantVals.INITIAL_DISEASE_CUBE_COUNT};

    public IEnumerator drawPhase(){
        List<PlayerCard> drawnCards = new List<PlayerCard>();
            try {
                drawnCards.Add(playerDeck.Pop()); 
                drawnCards.Add(playerDeck.Pop());
            }
            catch(InvalidOperationException){
                gameFlowManager.gameOver(ConstantVals.GAME_OVER_CARDS);
            } 
            boardUI.setPlayerDeckCount(playerDeck.Count);                   
            yield return StartCoroutine(boardUI.playerDraw(drawnCards[0].getName(), drawnCards[1].getName()));
            
            foreach (PlayerCard drawn in drawnCards){
                Debug.Log("Player draws " + drawn.getName());
                if(drawn.getId() == ConstantVals.EPIDEMIC){
                    // Increase Step
                    infectionRateTrackIndex++;
                    boardUI.advanceInfectionRateTrack();

                    // Infect Step
                    InfectionCard epidemicDrawn = EpidemicInfectionCards.Pop();
                    Location loc = locations[epidemicDrawn.getId()];
                    infectionDiscardPile.Add(epidemicDrawn);
                    yield return StartCoroutine(boardUI.infectionDraw(loc.getName(), loc.getColour()));
                    for (int j = 0; j < ConstantVals.CUBES_PER_EPIDEMIC_INFECT; j++){
                        yield return StartCoroutine(addCube(loc, loc.getColour(), epidemicDrawn.getId()));
                    }
                    outbreakCitiesThisMove.Clear();

                    //Intensify Step
                    Utils.ShuffleAndPlaceOnTop(infectionDiscardPile, infectionDeck);
                }
                else {
                    playerManager.drawPhaseAdd(drawn);
                }
            }
    }

    public IEnumerator infectionPhase(){
        for (int i = 0; i < infectionRateTrack[infectionRateTrackIndex]; i++){
            InfectionCard drawn = infectionDeck.Pop(); 
            Debug.Log("Infection: " + drawn.getName());
            Location loc = locations[drawn.getId()];
            infectionDiscardPile.Add(drawn);
            yield return StartCoroutine(boardUI.infectionDraw(drawn.getName(), loc.getColour()));
            yield return StartCoroutine(addCube(loc, loc.getColour(), drawn.getId()));
            outbreakCitiesThisMove.Clear();
        }
        playerManager.endPlayerTurn();
    }

    public IEnumerator addCube(Location loc, ConstantVals.Colour colour, int locId){
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
                //gameFlowManager.gameOver(ConstantVals.GAME_OVER_CUBES);       
            }
            yield return StartCoroutine(boardUI.addCube(loc, colour));
        }
        yield break;
    }
    
    public IEnumerator outbreakOccurs(Location loc, ConstantVals.Colour cubeColour, int locId){
        if (!outbreakCitiesThisMove.Contains(locId)){
            outbreakCount++;
            Debug.Log("Outbreak #" + outbreakCount + " in " + loc.getName());
            boardUI.increaseOutbreakCounter(outbreakCount);
            if (outbreakCount == ConstantVals.OUTBREAK_COUNT_LIMIT) {
                gameFlowManager.gameOver(ConstantVals.GAME_OVER_OUTBREAKS);
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

    public void boardSetUp(){
        generateDecks();
        setUpLocations();
    }

    private void generateDecks(){
        int i = 0;
        List<PlayerCard> cityCards = new List<PlayerCard>();
        List<InfectionCard> infectionCards = new List<InfectionCard>();
        foreach (string s in ConstantVals.cities){
            infectionCards.Add(new InfectionCard(i, s));
            cityCards.Add(new PlayerCard(i, s));
            i++;
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
            curPile.Add(new PlayerCard(ConstantVals.EPIDEMIC, "Epidemic"));
            Utils.ShuffleAndPlaceOnTop(curPile, playerDeck);
            decrementingPileCount--;     
        } 
        boardUI.setPlayerDeckCount(playerDeck.Count);
    }

    private void setUpLocations(){
        locations = GameObject.Find("Locations").GetComponentsInChildren<Location>();
        foreach (Location loc in locations){         
        }
    }

    public IEnumerator infectCities(){
        for (int i = ConstantVals.INITIAL_INFECTION_ROUNDS; i > 0; i--){
            for (int j = 0; j < ConstantVals.CARDS_PER_INITIAL_INFECTION_ROUND; j++){
                InfectionCard drawn = infectionDeck.Pop();
                Location loc = locations[drawn.getId()];
                infectionDiscardPile.Add(drawn);
                yield return StartCoroutine(boardUI.infectionDraw(drawn.getName(), loc.getColour()));
                
                Debug.Log("Initial infection in " + drawn.getName());
                for (int k = 0; k < i; k++){
                    /*loc.addCube(loc.getColour());
                    StartCoroutine(boardUI.addCube(loc, loc.getColour()));
                    diseaseCubeSupply[(int)loc.getColour()]--;
                    */            
                    yield return StartCoroutine(addCube(loc, loc.getColour(), drawn.getId()));
                }
                
                
            }
        }
    }

}
