using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Stack<InfectionCard> infectionDeck = new Stack<InfectionCard>();
    private List<InfectionCard> infectionDiscardPile = new List<InfectionCard>();
    private Stack<InfectionCard> EpidemicInfectionCards = new Stack<InfectionCard>();

    private Stack<PlayerCard> playerDeck = new Stack<PlayerCard>();
    private List<PlayerCard> playerDiscardPile = new List<PlayerCard>();
    private List<PlayerCard> eventCardsInHands = new List<PlayerCard>();
    

    private Location[] locations;
    private int[][] neighbours = ConstantVals.locNeighbours;
    private List<int> outbreakCitiesThisMove = new List<int>();

    private int infectionRateTrackIndex = 0;
    private int[] infectionRateTrack = new int[] {2,2,2,3,3,4,4};
    private int epidemicCardCount = 2;
    private int outbreakCount = 0;
    
    void Awake(){
        boardSetUp();
    }

    public void movePhase(){
        
    }

    public void drawPhase(){
        for (int i = 0; i < 2; i++){
            PlayerCard drawn = playerDeck.Pop();
            Debug.Log("Player draws " + drawn.getName());   
            if(drawn.getId() == ConstantVals.EPIDEMIC){
                // Increase Step
                infectionRateTrackIndex++;

                // Infect Step
                InfectionCard epidemicDrawn = EpidemicInfectionCards.Pop();
                for (int j = 0; j < ConstantVals.CUBES_PER_EPIDEMIC_INFECT; j++){
                    addCube(epidemicDrawn.getId());
                }
                infectionDiscardPile.Add(epidemicDrawn);
                outbreakCitiesThisMove.Clear();

                //Intensify Step
                Utils.ShuffleAndPlaceOnTop(infectionDiscardPile, infectionDeck);
            }
            else {
                // add to hand
            }   
        }      
    }

    public void infectionPhase(){
        for (int i = 0; i < infectionRateTrack[infectionRateTrackIndex]; i++){
            InfectionCard drawn = infectionDeck.Pop();
            Debug.Log("Infection: " + drawn.getName());
            addCube(drawn.getId());
            infectionDiscardPile.Add(drawn);
            outbreakCitiesThisMove.Clear();
        }
    }

    public void addCube(int locId){
        if (locations[locId].checkOutbreak(ConstantVals.Colour.YELLOW)){
            outbreakOccurs(locId, ConstantVals.Colour.YELLOW);
        }
        else {
            locations[locId].addCube(ConstantVals.Colour.YELLOW);
        }
    }
    
    public void outbreakOccurs(int locId, ConstantVals.Colour cubeColour){
        if (!outbreakCitiesThisMove.Contains(locId)){
            Debug.Log("Outbreak! in " + locId);
            outbreakCount++;
            outbreakCitiesThisMove.Add(locId);
            for (int i = 0; i < neighbours[locId].Length; i++){
                 addCube(neighbours[locId][i]);
            }
        }
    }

    private void boardSetUp(){
        generateDecks();
        getLocations();
        infectCities();
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
        createPlayerDeck(cityCards);
    }

    public void createInfectionDeck(List<InfectionCard> infectionCards){
        // Infection Deck
        Utils.ShuffleAndPlaceOnTop(infectionCards, infectionDeck);

        // Set aside "bottom" cards for epidemic draws
        for (int i = 0; i < epidemicCardCount; i++){
            EpidemicInfectionCards.Push(infectionDeck.Pop());
        }
    }

    public void createPlayerDeck(List<PlayerCard> cityCards){
        Stack<PlayerCard> shuffledPlayerCards = new Stack<PlayerCard>();
        Utils.ShuffleAndPlaceOnTop(cityCards, shuffledPlayerCards);
        
        // Create  and stack epidemic card piles with smallest piles at the bottom
        for (int k = 0; k < epidemicCardCount; k++){
            int pileSize = shuffledPlayerCards.Count / epidemicCardCount;
            List<PlayerCard> curPile = new List<PlayerCard>();
            for (int j = 0; j < pileSize; j++){
                curPile.Add(shuffledPlayerCards.Pop());
            }
            curPile.Add(new PlayerCard(ConstantVals.EPIDEMIC, "Epidemic"));
            Utils.ShuffleAndPlaceOnTop(curPile, playerDeck);
        } 
    }

    private void getLocations(){
        locations = GameObject.Find("Locations").GetComponentsInChildren<Location>();
        foreach (Location loc in locations){
            
        }
    }

    private void infectCities(){
        for (int i = ConstantVals.INITIAL_INFECTION_ROUNDS; i > 0; i--){
            for (int j = 0; j < ConstantVals.CARDS_PER_INITIAL_INFECTION_ROUND; j++){
                InfectionCard drawn = infectionDeck.Pop();
                Debug.Log("Initial infection in " + drawn.getName());
                for (int k = 0; k < i; k++){
                    locations[drawn.getId()].addCube(ConstantVals.Colour.YELLOW);
                }
                infectionDiscardPile.Add(drawn);
            }
        }
    }

}
