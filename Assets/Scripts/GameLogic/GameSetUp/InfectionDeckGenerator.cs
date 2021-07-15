using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates and arranges infection cards for initial game set-up

public class InfectionDeckGenerator
{
    private Board board;

    public InfectionDeckGenerator(Board board){
        this.board = board;
    }

    public void createInfectionDeck(){
        List<InfectionCard> infectionCards = new List<InfectionCard>();
        Location[] locations = GameObject.Find("Locations").GetComponentsInChildren<Location>();
        for (int i = 0; i < locations.Length; i++){
            infectionCards.Add(new InfectionCard(locations[i], i, locations[i].gameObject.name));
        }
        Utils.ShuffleAndPlaceOnTop(infectionCards, board.InfectionDeck);
    }

    public void setAsideEpidemicInfectionCards(){
        for (int i = 0; i < board.EpidemicCardCount; i++){
            board.EpidemicInfectionCards.Push(board.InfectionDeck.Pop());
        }
    }
}
