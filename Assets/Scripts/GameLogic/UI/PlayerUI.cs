using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{   
    private List<GameObject> players;
    private List<Text[]> playerHandCardNames = new List<Text[]>();
    private List<SpriteRenderer[]> playerHandIcons = new List<SpriteRenderer[]>();

    public Sprite[] diseaseIconSprites;
    public GameObject[] pawns;
    public Sprite[] pawnSprites;

    public void preparePlayerUIObjects(List<GameObject> players){
        this.players = players;
        int i = 0;
        foreach (GameObject player in players){
            // Gather active hand resources
            playerHandCardNames.Add(player.GetComponentsInChildren<Text>());
            playerHandIcons.Add(player.GetComponentsInChildren<SpriteRenderer>());
            // Activate required number of players
            player.SetActive(true);
            pawns[i].SetActive(true);
            
            // Assign sprites
            int role = player.GetComponent<Player>().getRoleID();
            Transform pawn = player.transform.Find("Pawn");
            pawn.GetComponent<SpriteRenderer>().sprite = pawnSprites[role];
            if (role == Vals.DISPATCHER || role == Vals.CONTINGENCY_PLANNER){
                pawn.transform.Find("CharacterAction").gameObject.SetActive(true);
            }
            player.transform.Find("PlayerName").GetComponent<Text>().text = Vals.ROLES[role];
            pawns[i].GetComponent<SpriteRenderer>().sprite = pawnSprites[role];
            i++;
        }
    }

    public void updateHand(Player player, List<PlayerCard> hand){
        int i = 0;
        foreach (PlayerCard card in hand){
            playerHandCardNames[player.turnOrderPos - 1][i].text = card.getName();
            playerHandIcons[player.turnOrderPos - 1][i].sprite = diseaseIconSprites[(int)card.getColour()];
            i++;
        }
        for (; i < Vals.MAX_HAND_SIZE; i++){
            playerHandCardNames[player.turnOrderPos - 1][i].text = "";
            playerHandIcons[player.turnOrderPos - 1][i].sprite = null;
        }
        //Debug.Log("i " + i);
    }

    public void placePawn(int turnOrderPos, Location loc){
        pawns[turnOrderPos].transform.position = loc.transform.position;
    }
}
