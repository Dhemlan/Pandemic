using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{   
    private List<GameObject> players;
    private List<Text[]> playerHands = new List<Text[]>();
    private List<SpriteRenderer[]> playerHandIcons = new List<SpriteRenderer[]>();

    public Sprite[] diseaseIconSprites;

    public void setPlayers(List<GameObject> players){
        this.players = players;
        foreach (GameObject player in players){
            playerHands.Add(player.GetComponentsInChildren<Text>());
            playerHandIcons.Add(player.GetComponentsInChildren<SpriteRenderer>());
        }
    }
    public void displayPlayers(){
        foreach (GameObject player in players){
            player.SetActive(true);
        }
    }

    public void updateHand(Player player, List<PlayerCard> hand){
        int i = 0;
        foreach (PlayerCard card in hand){
            playerHands[player.turnOrderPos - 1][i].text = card.getName();
            playerHandIcons[player.turnOrderPos - 1][i].sprite = diseaseIconSprites[(int)card.getColour()];
            i++;
        }
    }
}
