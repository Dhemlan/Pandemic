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
    private int handLimit = Vals.DEFAULT_HAND_LIMIT;
    private int maxActions = 4;

    private PlayerCard cardToTrade;
    public Board board;
    public GameObject boardPawn;

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
        moveCompleted();
    }

    public bool isDriveFerryValid(Location dest){
        return curLoc.hasNeighbour(dest);
    }

    public void driveFerryAction(Location dest){
        moveCompleted();
    }

    public void commercialFlightAction(PlayerCard cardToDiscard, Location dest){
        Debug.Log("commercial");
        moveRequiringDiscard(cardToDiscard, dest);
    }

    public void charterFlightAction(PlayerCard cardToDiscard, Location dest){
        Debug.Log("charter");
        moveRequiringDiscard(cardToDiscard, dest);
    }

    public void moveRequiringDiscard(PlayerCard cardToDiscard, Location dest){
        playerManager.removeCardFromHand(this, cardToDiscard, true);
        moveCompleted();
    }

    public void moveCompleted(){
        playerManager.incrementCompletedActions();
    }

    public IEnumerator otherMovement(Location dest, System.Action<bool> callback){
        if (role.nonStandardMove(this,dest)){
            if (role.getID() == Vals.OPERATIONS_EXPERT){
                List<PlayerCard> selectedCard = new List<PlayerCard>();
                yield return StartCoroutine(playerManager.requestUserSelectCard(getHandWithoutEvents(), selectedCard, 1, null));
                playerManager.removeCardFromHand(this, selectedCard[0], true);
            }
            moveCompleted();
            callback(true);
            yield break;
        }
        List<PlayerCard> availableCards = new List<PlayerCard>();
        PlayerCard currentLocCard = retrieveCardByLoc(curLoc);
        availableCards.Add(currentLocCard);
        PlayerCard clickedLocCard = retrieveCardByLoc(dest);
        availableCards.Add(clickedLocCard);

        Debug.Log("Other movement" + currentLocCard + clickedLocCard);
        if (currentLocCard != null && clickedLocCard != null){
            List<PlayerCard> selectedCard = new List<PlayerCard>();
            yield return StartCoroutine(playerManager.requestUserSelectCard(availableCards, selectedCard, 1, null));
            if(selectedCard[0].Equals(currentLocCard)){
                charterFlightAction(currentLocCard, dest);
            }
            else{
                commercialFlightAction(clickedLocCard, dest);
            }
            moveCompleted();
            callback(true);
            yield break;
        }
        else if (currentLocCard != null){
            charterFlightAction(currentLocCard, dest);
            callback(true);
            yield break;
            
        }
        else if (clickedLocCard != null){
            commercialFlightAction(clickedLocCard, dest);
            callback(true);
            yield break;
        }
        callback(false);
        yield break;
    }

    public IEnumerator buildAction(){
        if (curLoc.getResearchStationStatus()) yield break;
        if (role.buildAction(this)){
            yield return StartCoroutine(board.buildResearchStation(curLoc));
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
                    cardToTrade = selectedCards[0];
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
                    yield return StartCoroutine(playerManager.requestUserSelectPlayerToInteract(playerManager.nonCurrentPlayers(), message));
                }
                else{
                    if (localPlayers[0] == this){
                        playerManager.setUserSelectedPlayer(localPlayers[1]);
                    }
                    else {
                        playerManager.setUserSelectedPlayer(localPlayers[0]);
                    }
                }
                cardExchange(cardToTrade, this, playerManager.getUserSelectedPlayer());
                playerManager.setUserSelectedPlayer(null);
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
            if (cardLoc != null && cardLoc.Equals(loc)){
                return card;
            }
        }
        return null;
    }

    public void leaveLocation(){

    }

    public void enterLocation(Location dest){
        curLoc = dest;
        role.enterLocation(board, curLoc);
    }

    public void resetOncePerTurnActions(){
        role.resetOncePerTurnActions();
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

    public List<PlayerCard> getHandWithoutEvents(){
        List<PlayerCard> cards = new List<PlayerCard>();
        foreach(PlayerCard card in hand){
            if (card.getColour() != Vals.Colour.EVENT){
                cards.Add(card);
            }
        }
        return cards;
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

    public GameObject getBoardPawn(){
        return boardPawn;
    }
}
