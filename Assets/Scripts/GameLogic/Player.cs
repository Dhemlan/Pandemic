using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int turnOrderPos;
    private Role role;
    private Location curLoc;
    public List<PlayerCard> hand = new List<PlayerCard>();
    public GameObject pawn;


    public void addCardToHand(PlayerCard card){
        hand.Add(card);
    }

    public int getTurnOrderPos(){
        return turnOrderPos;
    }

    public List<PlayerCard> getHand(){
        return hand;
    }
}
