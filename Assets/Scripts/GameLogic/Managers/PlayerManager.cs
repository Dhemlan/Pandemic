using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public PlayerUI playerUI;
    public CardUI cardUI;
    public BoardUI boardUI;

    public Board board;
    
    private List<Player> activePlayerScripts = new List<Player>();
    private List<GameObject> activePlayerObjects = new List<GameObject>();
    private List<Role> availableRoles = new List<Role>();
    private Player curPlayer;
    private int completedActions;
    private int maxActions;
    private bool proceedSwitch = false;
    
    public GameObject playerArea;
    public GameObject[] allPlayerObjects;

    private System.Random rand = new System.Random();

    public void generateCharacters(int count){
        populateAvailableRoles();
        for (int i = 0; i < count; i++){
            Player newPlayer = allPlayerObjects[i].GetComponent<Player>();
            activePlayerScripts.Add(newPlayer);
            //newPlayer.getLocation().playerEnters(newPlayer);
            activePlayerObjects.Add(allPlayerObjects[i]);
            activePlayerScripts[i].setRole(provideRandomRole());
        }
        curPlayer = activePlayerScripts[0];

        playerUI.preparePlayerUIObjects(activePlayerObjects);
        int j = 0;
        foreach(Player player in activePlayerScripts){
            playerUI.placePawn(j, player.getLocation());
            j++;
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
        completedActions = 0;
        Debug.Log("Player " + curPlayer.getTurnOrderPos() + " in " + curPlayer.getLocation().getName());
        maxActions = curPlayer.getMaxActions();   
        yield return new WaitUntil(() => completedActions == maxActions);
        board.promptDrawPhase();
        yield return new WaitUntil(() => proceedSwitch);
        proceedSwitch = false;
    }

    public void incrementCompletedActions(){
        completedActions++;
        Debug.Log(completedActions + " /4 actions taken");
    }

    public void endPlayerTurn(){
        nextPlayer();
    }

    public void drawPhaseAdd(PlayerCard card){
        addCardToHand(curPlayer, card);
    }

    public void addCardToHand(Player player, PlayerCard card){
        player.addCardToHand(card);
        playerUI.updateHand(player, player.getHand());
    }

    public void removeCardFromHand(Player player, PlayerCard card){
        
    }

    public IEnumerator checkHandLimit(Player player){
        int numberCardsToDiscard = player.overHandLimit();
        if (numberCardsToDiscard > 0){
            List<PlayerCard> toDiscard = new List<PlayerCard>();
            yield return StartCoroutine(cardUI.requestSelectableFromPlayer(player.getHand(), toDiscard, numberCardsToDiscard, null));
            player.discardCards(toDiscard);
            foreach (PlayerCard card in toDiscard){
              //  board.discardCard(card);
                // discard animation yield return StartCoroutine(cardUI.)
            }
            playerUI.updateHand(player, player.getHand());
        }
        yield break;
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

    private void nextPlayer(){
        if(curPlayer.turnOrderPos == activePlayerScripts.Count){
            curPlayer = activePlayerScripts[0];
        }
        else{
            curPlayer = activePlayerScripts[curPlayer.turnOrderPos];
        }
    }

    public bool actionAvailable(){
        if(completedActions < curPlayer.getMaxActions()){
            return true;
        }
        return false;
    }

    public Location getCurPlayerLocation(){
        return curPlayer.getLocation();
    }

    public Player getCurPlayer(){
        return curPlayer;
    }

    public Player playerPerformingAction(){
        return curPlayer;
    }
    public void placePawn(Player player){
        playerUI.placePawn(player.getTurnOrderPos() - 1, player.getLocation());
    }

    public void endActionPhase(){
        completedActions = maxActions;
        proceedSwitch = true;
    }

    public List<Player> getLocalPlayers(Location loc){
        List<Player> localPlayers = new List<Player>();
        foreach (Player player in activePlayerScripts){
            if (player.getLocation().Equals(loc)){
                localPlayers.Add(player);
            }
        }
        return localPlayers;
    }

    public void requestUserSelectPlayerToInteract(List<Player> players){
        Debug.Log("more than 2 players");

    }

    public void updateHand(Player player){
        playerUI.updateHand(player, player.getHand());
    }
    
}
