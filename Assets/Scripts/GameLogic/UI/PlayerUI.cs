using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{   
    private List<Text[]> playerHandCardNames = new List<Text[]>();
    private List<SpriteRenderer[]> playerHandIcons = new List<SpriteRenderer[]>();

    public Sprite[] diseaseIconSprites;
    public Sprite[] pawnSprites;

    public void preparePlayerUIObjects(List<GameObject> players){
        foreach (GameObject player in players){
            // Gather active hand resources
            playerHandCardNames.Add(player.GetComponentsInChildren<Text>());
            playerHandIcons.Add(player.GetComponentsInChildren<SpriteRenderer>());
            // Activate required number of players
            player.SetActive(true);
            
            // Assign sprites
            Player playerScript = player.GetComponent<Player>();
            int role = playerScript.getRoleID();
            Transform pawn = player.transform.Find("Pawn");
            pawn.GetComponent<SpriteRenderer>().sprite = pawnSprites[role];
            if (role == Vals.DISPATCHER || role == Vals.CONTINGENCY_PLANNER){
                pawn.transform.Find("CharacterAction").gameObject.SetActive(true);
            }
            player.transform.Find("PlayerName").GetComponent<Text>().text = Vals.ROLES[role];
            GameObject boardPawn = playerScript.getBoardPawn();
            boardPawn.GetComponent<SpriteRenderer>().sprite = pawnSprites[role];
            updatePawns(playerScript.getLocation());
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
    }

    public void updatePawns(Location loc){
        List<Player> localPlayers = loc.getLocalPlayers();
        float x = -.5f;
        switch (localPlayers.Count){
            case 1:   
                localPlayers[0].getBoardPawn().transform.position = loc.transform.position;

                break;
            case 2:
                foreach (Player player in localPlayers){
                    GameObject pawn = player.getBoardPawn();
                    pawn.transform.position = loc.transform.position;
                    pawn.transform.Translate(x,0,0);
                    x *= -1;
                }
                break;
            case 3:
                foreach (Player player in localPlayers){
                    GameObject pawn = player.getBoardPawn();
                    pawn.transform.position = loc.transform.position;
                    pawn.transform.Translate(x,0,0);
                    x += 1.0f;
                }
                break;
            case 4:
                x = -.75f;
                foreach (Player player in localPlayers){
                    GameObject pawn = player.getBoardPawn();
                    pawn.transform.position = loc.transform.position;
                    pawn.transform.Translate(x,0,0);
                    x += .75f;
                    
                }
                break; 
        }    
    }

    public void placePawn(Player player){
        List<Player> localPlayers = player.getLocation().getLocalPlayers();
        Debug.Log("local players: " + localPlayers.Count);
        if (player.getLocation().getLocalPlayers().Count > 1){
            updatePawns(player.getLocation());
        }
        else{
            player.getBoardPawn().transform.position = player.getLocation().transform.position;
        } 
        
    }
}
