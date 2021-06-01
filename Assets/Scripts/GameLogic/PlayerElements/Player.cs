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
    private int handLimit = 4;//Vals.DEFAULT_HAND_LIMIT;
    private int maxActions = 4;

    private Player otherPlayerInInteraction;
    private PlayerCard cardToTrade;

    public Board board;

    public void Awake(){
        curLoc = startingLoc;
        startingLoc.playerEnters(this);
    }

    public void addCardToHand(PlayerCard card){
        hand.Add(card);
        hand.Sort();   
    }

    public void removeCardFromHand(PlayerCard card){
        hand.Remove(card); 
    }



    public void discardCard(PlayerCard card){
        hand.Remove(card); 
    }

    public void treatAction(Location loc, Vals.Colour colour){
        bool removeAll = role.treatAction();
        if (removeAll || board.isDiseaseCured(colour)){
            Debug.Log("clear to remove all");
            board.removeCubes(loc, colour);
        }
        else {
            Debug.Log("remove single cube");
            board.removeCube(loc, colour);
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
        playerManager.removeCardFromHand(this, retrieveCardByLoc(dest), true);
        curLoc = dest;
        playerManager.incrementCompletedActions();
    }

    public void charterFlightAction(Location dest){
        Debug.Log("charter");
        playerManager.removeCardFromHand(this, retrieveCardByLoc(curLoc), true);
        curLoc = dest;
        playerManager.incrementCompletedActions();
    }

    public bool nonStandardMove(Location dest){
        if (role.nonStandardMove(this)){
            curLoc = dest;
            playerManager.incrementCompletedActions();
            return true;
        }
        return false;
    }

    public bool otherMovement(Location dest){
        if (nonStandardMove(dest)){
            return true;
        }
        
        bool hasCurrentLoc = hasCardByLoc(curLoc);
        bool hasClickedLoc = hasCardByLoc(dest);

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
        if (role.buildAction(this)){
            board.buildResearchStation(curLoc);
            Debug.Log("building in " + curLoc.getName());
            playerManager.incrementCompletedActions();
        } 
    }

    public IEnumerator shareAction(){
        Debug.Log("Player share action");
        List<Player> localPlayers = curLoc.getLocalPlayers();
        if (localPlayers.Count > 1){
            List<PlayerCard> potentialTradeableCards = new List<PlayerCard>();
            role.findGiveableCards(this, potentialTradeableCards);
            foreach (Player player in localPlayers){
                if (player != this){
                    player.getRole().findGiveableCards(player, potentialTradeableCards);
                }
            }
            Debug.Log("Potential cards: " + potentialTradeableCards.Count);
            if (potentialTradeableCards.Count > 0){
                if (potentialTradeableCards.Count > 1){
                    Debug.Log("1a - select card to trade");
                    List<PlayerCard> selectedCards = new List<PlayerCard>();
                    yield return StartCoroutine(playerManager.requestUserSelectCard(potentialTradeableCards, selectedCards, 1, null));
                }
                else {
                    cardToTrade = potentialTradeableCards[0];
                    Debug.Log("1b - trade " + cardToTrade.getName());
                }

                Player cardOwner = this;
                foreach (Player player in localPlayers){
                    if (player == this) continue;
                    if (player.hasCard(cardToTrade)){
                        // Case: current player receives
                        Debug.Log("receiving" + cardToTrade);
                        cardExchange(cardToTrade, player, this);
                        playerManager.incrementCompletedActions();
                        yield break;
                    }
                }
                // Current player gives
                Debug.Log("giving");
                if (localPlayers.Count > 2){
                    //request target
                    Debug.Log("choose who to give to");
                    string message = "Select player to trade with";
                    yield return StartCoroutine(playerManager.requestUserSelectPlayerToInteract(localPlayers, message));
                }
                else{
                    if (localPlayers[0] == this){
                        otherPlayerInInteraction = localPlayers[1];
                    }
                    else {
                        otherPlayerInInteraction = localPlayers[0];
                    }
                }
                cardExchange(cardToTrade, this, otherPlayerInInteraction);
                playerManager.incrementCompletedActions();
            }        
        }   
    }

    public void cardExchange(PlayerCard card, Player provider, Player receiver){
        playerManager.removeCardFromHand(provider, card, false);
        playerManager.addCardToHand(receiver, card);
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

    public void incrementCompletedActions(){
        playerManager.incrementCompletedActions();
    }

    public bool hasCard(PlayerCard card){
        return hand.Contains(card);
    }

    public bool hasCardByLoc(Location loc){
        foreach (PlayerCard card in hand){
            Location cardLoc = card.getLocation();
            if(cardLoc != null && cardLoc.Equals(loc)){
                Debug.Log("has " + loc.getName());
                return true;
            }
        }
        return false;
    }

    public PlayerCard retrieveCardByLoc(Location loc){
        foreach(PlayerCard card in hand){
            Location cardLoc = card.getLocation();
            Debug.Log(card.getName());
            if (cardLoc.Equals(loc)){
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

    public void setRole(Role role){
        this.role = role;
    }
    
    public Role getRole(){
        return role;
    }

    public int getRoleID(){
        return role.getID();
    }

    public void setOtherPlayerInInteraction(Player player){
        otherPlayerInInteraction = player;
    }
}
