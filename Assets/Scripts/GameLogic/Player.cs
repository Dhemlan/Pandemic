using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public PlayerManager playerManager;
    
    public int turnOrderPos;
    private Role role;
    private Location curLoc;
    public Location startingLoc;
    public List<PlayerCard> hand = new List<PlayerCard>();
    private List<Location> handLocs = new List<Location>();
    private int handLimit = Vals.DEFAULT_HAND_LIMIT;
    private int maxActions = 4;

    public Board board;

    public void Awake(){
        curLoc = startingLoc;
        role = new Role();
        startingLoc.playerEnters(this);
    }

    public void addCardToHand(PlayerCard card){
        hand.Add(card);
        hand.Sort();
        updateLocationsofHand();
    }

    public void discardCards(List<PlayerCard> cards){
        foreach(PlayerCard card in cards){
            hand.Remove(card);
            board.discardCard(card);
        }
        updateLocationsofHand();
    }

    public void discardCard(PlayerCard card){
        hand.Remove(card);
        board.discardCard(card);
        updateLocationsofHand();
    }

    // return cards required to cure
    public int roleModifiedCardsToCure(int standardCardsToCure){
        return standardCardsToCure - (standardCardsToCure - role.getCardsToCure());
    }

    public Nullable<Vals.Colour> determineCureActionAvailable(int[] cardCureRequirements, bool researchStationAvailable){
        if (researchStationAvailable){
            int[] cardsOfEachColour = new int[Vals.DISEASE_COUNT];
            PlayerCard prev = hand[0];
            int colourCount = 1;
            int cardsToCure = roleModifiedCardsToCure(cardCureRequirements[(int)prev.getColour()]);
            for (int i = 1; i < hand.Count; i++){
                if (hand[i].getColour() == prev.getColour() && !board.isDiseaseCured(hand[i].getColour())){
                    colourCount++;
                    if (colourCount == cardsToCure){
                        return prev.getColour();
                    }
                }
                else {
                    colourCount = 1;
                }
                prev = hand[i];
            }
        }
        return null;
    }

    public void treatAction(Location loc){
        if (board.isDiseaseCured(loc.getColour())){
            Debug.Log("clear to remove all");
            board.removeCubes(loc);
        }
        else {
            Debug.Log("remove single cube");
            board.removeCube(loc);
        }
        playerManager.incrementCompletedActions();
    }

    public void shuttleFlightAction(Location dest){
        incrementCompletedActions();
        curLoc = dest;
    }

    public bool isDriveFerryValid(Location dest){
        return curLoc.hasNeighbour(dest);
    }

    public void driveFerryAction(Location dest){
        incrementCompletedActions();
        curLoc = dest;
    }

    public void commercialFlightAction(Location dest){
        Debug.Log("commerical");
        discardCard(hand[(handLocs.IndexOf(dest))]);
        curLoc = dest;
        playerManager.incrementCompletedActions();
    }

    public void charterFlightAction(Location dest){
        Debug.Log("charter");
        discardCard(hand[(handLocs.IndexOf(curLoc))]);
        curLoc = dest;
        playerManager.incrementCompletedActions();
    }

    public bool otherMovement(Location dest){
        updateLocationsofHand();
        bool hasCurrentLoc = handLocs.Contains(curLoc);
        bool hasClickedLoc = handLocs.Contains(dest);

        Debug.Log("Other movement" + hasCurrentLoc + hasClickedLoc);
        if (hasCurrentLoc && hasClickedLoc){
            // request choice
            curLoc = dest;
            playerManager.incrementCompletedActions();
            return true;
        }
        else if (hasCurrentLoc){
            charterFlightAction(dest);
            return true;
            
        }
        else if (hasClickedLoc){
            commercialFlightAction(dest);
            return true;
        }
        return false;
    }

    public void buildAction(){
        if (curLoc.getResearchStationStatus()) return;
        foreach (PlayerCard card in hand){
            if (curLoc.Equals(card.getLocation())){
                board.buildResearchStation(curLoc);
                discardCard(card);
                Debug.Log("building in " + curLoc.getName());
                playerManager.incrementCompletedActions();
                break;
            }
        }   
    }

    public Player shareAction(){
        Debug.Log("Player share action");
        List<Player> localPlayers = curLoc.getLocalPlayers();
        Player otherPlayer = null;
        if (localPlayers.Count > 1){
            Debug.Log("player nearby");
            if (handLocs.Contains(curLoc)){
                Debug.Log("Can give" + curLoc.getName());
                if(localPlayers[0] == this){
                    otherPlayer = localPlayers[1];
                }
                else {
                    otherPlayer = localPlayers[0];
                }
                PlayerCard cardToTrade = retrieveCardByLoc(curLoc);
                hand.Remove(cardToTrade);
                otherPlayer.addCardToHand(cardToTrade);
                playerManager.incrementCompletedActions();
                updateLocationsofHand();          
            }
            else {
                foreach(Player player in localPlayers){
                    if(player != this && player.hasCardByLoc(curLoc)){
                        Debug.Log("can take " + curLoc.getName());
                        otherPlayer = player;
                        PlayerCard cardToTrade = otherPlayer.retrieveCardByLoc(curLoc);
                        addCardToHand(cardToTrade);
                        otherPlayer.getHand().Remove(cardToTrade);
                        playerManager.incrementCompletedActions();     
                    }
                }
                updateLocationsofHand();
            }
        }
        return otherPlayer;
    }

    public void updateLocationsofHand(){
        handLocs.Clear();
        foreach (PlayerCard card in hand){
            handLocs.Add(card.getLocation());
        }
    }

    public void incrementCompletedActions(){
        playerManager.incrementCompletedActions();
    }

    public bool hasCardByLoc(Location loc){
        if (handLocs.Contains(loc)) return true;
        return false;
    }

        public void getLocationsInHand(List<Location> locs){
        foreach (PlayerCard card in hand){
            locs.Add(card.getLocation());
        }
    }

    public PlayerCard retrieveCardByLoc(Location loc){
        foreach(PlayerCard card in hand){
            if (card.getLocation().Equals(loc)){
                return card;
            }
        }
        return null;
    }

    public void setCurLoc(Location loc){
        curLoc = loc;
    }

    public int getMaxActions(){
        return maxActions;
    }

    public int overHandLimit(){
        return hand.Count - handLimit;
    }

    public int getTurnOrderPos(){
        return turnOrderPos;
    }

    public Location getLocation(){
        return curLoc;
    }
    
    public List<PlayerCard> getHand(){
        return hand;
    }
}
