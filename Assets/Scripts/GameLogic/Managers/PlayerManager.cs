using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerUI playerUI;
    public CardUI cardUI;
    public BoardUI boardUI;

    public Board board;
    
    private List<Player> activePlayerScripts = new List<Player>();
    private List<GameObject> activePlayerObjects = new List<GameObject>();
    private Player curPlayer;
    private int completedActions;
    private int maxActions;
    private bool proceedSwitch = false;
    
    public GameObject playerArea;
    public GameObject[] allPlayerObjects;

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

    public void generateCharacters(int count){
        for (int i = 0; i < count; i++){
            activePlayerScripts.Add(allPlayerObjects[i].GetComponent<Player>());
            activePlayerObjects.Add(allPlayerObjects[i]);
        }
        curPlayer = activePlayerScripts[0];

        playerUI.setPlayers(activePlayerObjects);
        playerUI.displayPlayers();
        playerUI.placePawn(0, activePlayerScripts[0].getLocation());
        playerUI.placePawn(1, activePlayerScripts[1].getLocation());
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
            yield return StartCoroutine(cardUI.allowSelectionToDiscard(player.getHand(), toDiscard, numberCardsToDiscard));
            player.discardCards(toDiscard);
            foreach (PlayerCard card in toDiscard){
                board.discardCard(card);
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
        proceedSwitch = true;
    }
}
