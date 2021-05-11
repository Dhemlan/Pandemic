using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int turnOrderPos;
    private Role role;
    private Location curLoc;
    public Location startingLoc;
    public List<PlayerCard> hand = new List<PlayerCard>();
    private int handLimit = ConstantVals.DEFAULT_HAND_LIMIT;
    private int maxActions = 4;

    public void Awake(){
        curLoc = startingLoc;
        role = new Role();
    }

    public void addCardToHand(PlayerCard card){
        hand.Add(card);
        hand.Sort();
    }

    public int overHandLimit(){
        return hand.Count - handLimit;
    }

    public int getTurnOrderPos(){
        return turnOrderPos;
    }

    // return cards required to cure
    public int attemptCure(int[] cardCureRequirements, bool researchStationAvailable){
        int[] cardsOfEachColour = new int[ConstantVals.DISEASE_COUNT];
        foreach (PlayerCard card in hand){
            cardsOfEachColour[(int)card.getColour()]++;
        }
        for (int i = 0; i < cardsOfEachColour.Length; i++){
            if (cardsOfEachColour[i] >= (cardCureRequirements[i] - (cardCureRequirements[i] - role.getCardsToCure()))){
                return role.getCardsToCure();
            }
        }
        return -1;
    }

    public Location getLocation(){
        return curLoc;
    }

    public void discardCards(List<PlayerCard> cards){
        foreach(PlayerCard card in cards){
            hand.Remove(card);
        }
    }

    public List<PlayerCard> getHand(){
        return hand;
    }

    public void getLocationsInHand(List<Location> locs){
        foreach (PlayerCard card in hand){
            locs.Add(card.getLocation());
        }
    }

    public void setCurLoc(Location loc){
        curLoc = loc;
    }

    public int getMaxActions(){
        return maxActions;
    }

    public void treatAction(){

    }

    public void shuttleFlightAction(){

    }

    public void driveFerryAction(){

    }

    public void commercialFlightAction(){

    }

    public void charterFlightAction(){

    }
}
