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
    public GameObject[] pawns;

    public void setPlayers(List<GameObject> players){
        this.players = players;
        foreach (GameObject player in players){
            playerHands.Add(player.GetComponentsInChildren<Text>());
            playerHandIcons.Add(player.GetComponentsInChildren<SpriteRenderer>());
            // trim pawn sprite from end of array
        }
    }
    public void displayPlayers(){
        int i = 0;
        foreach (GameObject player in players){
            player.SetActive(true);
            pawns[i].SetActive(true);
            i++;
        }
    }

    public void updateHand(Player player, List<PlayerCard> hand){
        int i = 0;
        foreach (PlayerCard card in hand){
            playerHands[player.turnOrderPos - 1][i].text = card.getName();
            playerHandIcons[player.turnOrderPos - 1][i].sprite = diseaseIconSprites[(int)card.getColour()];
            i++;
        }
        //Debug.Log("hand size " + hand.Count +" : i " + i);
        for (; i < ConstantVals.MAX_HAND_SIZE; i++){
            playerHands[player.turnOrderPos - 1][i].text = "";
            playerHandIcons[player.turnOrderPos - 1][i].sprite = null;
        }
        //Debug.Log("i " + i);
    }

    public void placePawn(int turnOrderPos, Location loc){
        pawns[turnOrderPos].transform.position = loc.transform.position;
    }
}
