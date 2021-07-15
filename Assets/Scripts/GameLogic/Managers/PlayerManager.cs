using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public PlayerUI playerUI;
    public OverlayUI overlayUI;
    public BoardUI boardUI;

    public Board board;
    
    private List<Player> activePlayerScripts = new List<Player>();
    private List<GameObject> activePlayerObjects = new List<GameObject>();
    private List<Role> availableRoles = new List<Role>();
    private Player curPlayer;
    private int completedActions;
    private int maxActions;
    private bool proceedSwitch = false;
    private Player userSelectedPlayer;
    
    public GameObject playerArea;
    public GameObject[] allPlayerObjects;

    private System.Random rand = new System.Random();

    public int CompletedActions { get => completedActions; set => completedActions = value; }

    public void generateCharacters(int count){
        populateAvailableRoles();
        for (int i = 0; i < count; i++){
            Player newPlayer = allPlayerObjects[i].GetComponent<Player>();
            activePlayerScripts.Add(newPlayer);
            activePlayerObjects.Add(allPlayerObjects[i]);
            activePlayerScripts[i].setRole(provideRandomRole());
        }
        curPlayer = activePlayerScripts[0];
        playerUI.toggleCurPlayer(curPlayer);

        playerUI.preparePlayerUIObjects(activePlayerObjects);
        foreach(Player player in activePlayerScripts){
            playerUI.placePawn(player);
        }
    }

    private void populateAvailableRoles(){
        availableRoles.Add(new ContingencyPlanner());
        availableRoles.Add(new Dispatcher());
        availableRoles.Add(new Medic());
        availableRoles.Add(new OperationsExpert());
        availableRoles.Add(new QuarantineSpecialist());
        availableRoles.Add(new Researcher());
        availableRoles.Add(new Scientist());
    }

    public Role provideRandomRole(){
        int index = rand.Next(availableRoles.Count);
        Role randomRole = availableRoles[index];
        Debug.Log(randomRole);
        availableRoles.Remove(randomRole);
        return randomRole;
    }

    public IEnumerator movePhase(){
        CompletedActions = 0;
        playerUI.toggleUndoActive(true);
        Debug.Log("Player " + curPlayer.getTurnOrderPos() + " in " + curPlayer.CurLoc.retrieveLocName());
        maxActions = curPlayer.getMaxActions();
        playerUI.updateActionCount(CompletedActions, maxActions);
        yield return new WaitUntil(() => CompletedActions == maxActions);
        board.promptDrawPhase();
        yield return new WaitUntil(() => proceedSwitch);
        proceedSwitch = false;
        playerUI.toggleUndoActive(false);
    }

    public void incrementCompletedActions(){
        CompletedActions++;
        playerUI.updateActionCount(CompletedActions, maxActions);
    }

    public void endPlayerTurn(){
        curPlayer.resetOncePerTurnActions();
        playerTurnOver();
    }

    public void drawPhaseAdd(PlayerCard card){
        curPlayer.addCardToHand(card);
        playerUI.updateHand(curPlayer, curPlayer.getHand());
    }

    public void addCardToHand(Player player, PlayerCard card){
        player.addCardToHand(card);
        playerUI.updateHand(player, player.getHand());
        StartCoroutine(checkHandLimit(player));
    }

    public void removeCardFromHand(Player player, PlayerCard card, bool discard){
        player.removeCardFromHand(card);
        if(discard) board.discardCard(card);
        playerUI.updateHand(player, player.getHand());
    }

    public bool removeCardFromHand(PlayerCard card, bool toDiscard){
        foreach (Player player in activePlayerScripts){
            if(player.hasCard(card)){
                removeCardFromHand(player, card, toDiscard);
                Debug.Log("Discarding " + card.Name + player);
                return true;
            }
        }
        return false;
    }

    public IEnumerator checkHandLimit(Player player){
        int numberCardsToDiscard = player.overHandLimit();
        int handSize = player.getHand().Count;
        int handWithoutEvents = player.getHandWithoutEvents().Count;

        while (numberCardsToDiscard > 0 && handSize != handWithoutEvents){
                board.requestUserAcceptContinue(Strings.PLAY_EVENT_OR_DISCARD);
                yield return new WaitUntil(() => Vals.continueGameFlow);
                numberCardsToDiscard = player.overHandLimit();
                // if hand size has not changed, user has selected continue (vs played event)
                handSize = player.getHand().Count;
                if(numberCardsToDiscard > 0 && handSize == player.getHand().Count){
                    break;
                }
        }
        if (numberCardsToDiscard > 0){
            List<PlayerCard> toDiscard = new List<PlayerCard>();
            string message = Strings.DISCARD_PREFIX + numberCardsToDiscard + Strings.DISCARD_SUFFIX;
            yield return StartCoroutine(overlayUI.requestMultiSelect(player.getHand(), toDiscard, Vals.SELECTABLE_PLAYER_CARD, numberCardsToDiscard, null, message));
            discardCards(player, toDiscard);
            foreach (PlayerCard card in toDiscard){
                // discard animation yield return StartCoroutine(cardUI.)
            }
        }
        yield break;
    }
    
    public void discardCards(Player player, List<PlayerCard> toDiscard){
        foreach (PlayerCard card in toDiscard){
            removeCardFromHand(player, card, true);
        }
    }

    public int getInitialHandCount(){
        return (activePlayerScripts.Count % 2 == 0) ? 8 : 9;
    }

    public void setInitialHands (List<PlayerCard> cards){
        foreach (PlayerCard card in cards){
            addCardToHand(curPlayer, card);
            nextPlayer();
        }
    }
    
    public IEnumerator potentialPlayerMovement(Player actionTaker, Location loc){
        Player playerMoving = actionTaker;
        if (userSelectedPlayer != null){
            playerMoving = userSelectedPlayer;
        }
        if (playerMoving.CurLoc == loc){
            // dispatcher attempts to move other to other's cur location
        }
        else if (playerMoving.CurLoc.ResearchStationStatus && loc.ResearchStationStatus){
            Debug.Log("shuttle flight");
            playerLeavesLocation(playerMoving);
            playerMoving.shuttleFlightAction(loc);
            playerEntersLocation(playerMoving, loc);
        }
        else if (playerMoving.isDriveFerryValid(loc)){
            Debug.Log("drive/ferry");
            playerLeavesLocation(playerMoving);
            playerMoving.driveFerryAction(loc);
            playerEntersLocation(playerMoving, loc);
        }
        else {
            Location curLoc = actionTaker.CurLoc;
            yield return StartCoroutine(actionTaker.otherMovement(loc, (movementCompleted) =>{
                if (movementCompleted){
                    playerLeavesLocation(playerMoving);
                    playerUI.updateHand(actionTaker, actionTaker.getHand());
                    playerEntersLocation(playerMoving, loc);
                }
            }));
        }
        userSelectedPlayer = null;
    }

    public void playerLeavesLocation(Player player){
        player.CurLoc.playerLeaves(player);
        playerUI.updatePawns(player.CurLoc);
        player.leaveLocation();
    }

    public void playerEntersLocation(Player player, Location dest){
        dest.playerEnters(player);
        player.enterLocation(dest);
        placePawn(player);
    }

    private void playerTurnOver(){
        playerUI.toggleCurPlayer(curPlayer);
        nextPlayer();
        playerUI.toggleCurPlayer(curPlayer);
    }

    private void nextPlayer(){
        if(curPlayer.turnOrderPos == activePlayerScripts.Count){
            curPlayer = activePlayerScripts[0];
        }
        else{
            curPlayer = activePlayerScripts[curPlayer.turnOrderPos];
        }
    }

    public bool actionAvailable(){
        if(CompletedActions < curPlayer.getMaxActions()){
            return true;
        }
        return false;
    }

    public Location getCurPlayerLocation(){
        return curPlayer.CurLoc;
    }

    public Player getCurPlayer(){
        return curPlayer;
    }

    public List<Player> getPlayers(){
        return activePlayerScripts;
    }

    public Player playerPerformingAction(){
        return curPlayer;
    }
    public void placePawn(Player player){
        playerUI.placePawn(player);
        //playerUI.updatePawns(player.CurLoc);
    }

    public void endActionPhase(){
        CompletedActions = maxActions;
        proceedSwitch = true;
    }

    public IEnumerator requestUserSelectPlayerToInteract(List<Player> players, string message){
        if (players == null) players = activePlayerScripts;
        yield return StartCoroutine(overlayUI.requestSimpleSelectionFromPlayer(players, Vals.SELECTABLE_PLAYER, message));
    }

    public IEnumerator requestUserSelectCard(List<PlayerCard> cardsToSelectFrom, List<PlayerCard> selectedCards, int numberToSelect, Nullable<Vals.Colour> colourToSelect){
        string message = Strings.SELECT_CARD;
        yield return StartCoroutine(overlayUI.requestMultiSelect(cardsToSelectFrom, selectedCards, Vals.SELECTABLE_PLAYER_CARD, numberToSelect, colourToSelect, message));
    }

    public void updateHand(Player player){
        playerUI.updateHand(player, player.getHand());
    }

    public void setUserSelectedPlayer(Player player){
        userSelectedPlayer = player;
    }

    public Player getUserSelectedPlayer(){
        return userSelectedPlayer;
    }

    public void airlift(Location dest){
        playerLeavesLocation(userSelectedPlayer);
        userSelectedPlayer.CurLoc = dest;
        playerEntersLocation(userSelectedPlayer, dest);
        userSelectedPlayer = null;  
    }

    public void storedEventCardPlayed(){
        foreach(Player player in activePlayerScripts){
            if (player.getRoleID() == Vals.CONTINGENCY_PLANNER){
                ContingencyPlanner planner = (ContingencyPlanner) player.getRole();
                planner.storedEventCardPlayed();
            }
        }    
    }

    public void cureOccurs(Vals.Colour colour){
        foreach(Player player in activePlayerScripts){
            if(player.getRoleID() == Vals.MEDIC){
                board.removeCubes(player.CurLoc, colour);
            }
        }
    }

    public List<Player> nonCurrentPlayers(){
        List<Player> otherPlayers = new List<Player>();
        foreach(Player player in activePlayerScripts){
            if (player != curPlayer){
                otherPlayers.Add(player);
            }
        }
        return otherPlayers;
    }

    public void reloadPlayerUI(){
        playerUI.updateActionCount(completedActions, curPlayer.getMaxActions());
        foreach(Player player in activePlayerScripts){
            Debug.Log(player + " updating hand and position");
            updateHand(player);
            placePawn(player);
        }
    }
}
