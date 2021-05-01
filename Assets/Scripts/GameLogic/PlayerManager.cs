using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerUI playerUI;
    
    private List<Player> playerPool = new List<Player>();
    private Player curPlayer;
    private Player[] playerScripts;
    private List<GameObject> activePlayers = new List<GameObject>();

    public GameObject playerArea;
    public GameObject[] playerObjects;

    public void movePhase(){
        Debug.Log("Current player " + curPlayer.turnOrderPos);
    }

    public void endPlayerTurn(){
        nextPlayer();
    }

    public void drawPhaseAdd(PlayerCard card){
        addCardToHand(curPlayer, card);
    }

    public void generateCharacters(int count){
        
        for (int i = 0; i < count; i++){
            playerPool.Add(playerObjects[i].GetComponent<Player>());           
            activePlayers.Add(playerObjects[i]);
        }
        curPlayer = playerPool[0];
        playerUI.setPlayers(activePlayers);
        playerUI.displayPlayers();
    }

    public void addCardToHand(Player player, PlayerCard card){
        player.addCardToHand(card);
        playerUI.updateHand(player, player.getHand());
    }

    
    public int getInitialHandCount(){
        return (playerPool.Count % 2 == 0) ? 8 : 9;
    }

    public void setInitialHands (List<PlayerCard> cards){
        foreach (PlayerCard card in cards){
            addCardToHand(curPlayer, card);
            nextPlayer();
        }
    }

    private void nextPlayer(){
        if(curPlayer.turnOrderPos == playerPool.Count){
            curPlayer = playerPool[0];
        }
        else{
            curPlayer = playerPool[curPlayer.turnOrderPos];
        }
    }
}
