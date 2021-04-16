using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Stack<InfectionCard> infectionDeck = new Stack<InfectionCard>();
    private Stack<PlayerCard> playerDeck = new Stack<PlayerCard>();
    private List<InfectionCard> infectionDiscardPile = new List<InfectionCard>();
    private List<PlayerCard> playerDiscardPile = new List<PlayerCard>();

    private Location[] locations;

    private int infectionRate = ConstantVals.BASE_INFECTION_RATE;
    private int epidemicCardCount;
    
    void Awake(){
        boardSetUp();
    }

    public void movePhase(){
        
    }

    public void drawPhase(){

        
    }

    public void infectionPhase(){
        for (int i = 0; i < infectionRate; i++){
            InfectionCard drawn = infectionDeck.Pop();
            Debug.Log(drawn.getName());
            infectionDiscardPile.Add(drawn);
        }
    }

    private void boardSetUp(){
        generateDecks();
        getLocations();


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
        Utils.ShuffleAndPlaceOnTop(infectionCards, infectionDeck);
    }

    private void getLocations(){
        locations = GameObject.Find("Locations").GetComponentsInChildren<Location>();
        foreach (Location loc in locations){
            Debug.Log(loc.gameObject.name);
        }
    }

}
