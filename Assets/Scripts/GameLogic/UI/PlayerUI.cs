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
    public Sprite[] actionIconSprites;
    public Text actionsCountText;

    public GameObject undoButton;

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
                GameObject actionIcon = pawn.transform.Find("CharacterAction").gameObject;
                actionIcon.SetActive(true);
                actionIcon.GetComponent<SpriteRenderer>().sprite = actionIconSprites[role];
                actionIcon.GetComponentInChildren<Text>().text = Vals.actionIconLabels[role];
            }
            player.transform.Find("PlayerName").GetComponent<Text>().text = Vals.ROLES[role];
            GameObject boardPawn = playerScript.getBoardPawn();
            boardPawn.GetComponent<SpriteRenderer>().sprite = pawnSprites[role];
            updatePawns(playerScript.CurLoc);
        }
    }

    public void updateHand(Player player, List<PlayerCard> hand){
        int i = 0;
        foreach (PlayerCard card in hand){
            playerHandCardNames[player.turnOrderPos - 1][i].text = card.Name;
            playerHandIcons[player.turnOrderPos - 1][i].sprite = diseaseIconSprites[(int)card.Colour];
            i++;
        }
        for (; i < Vals.MAX_HAND_SIZE; i++){
            playerHandCardNames[player.turnOrderPos - 1][i].text = "";
            playerHandIcons[player.turnOrderPos - 1][i].sprite = null;
        }
    }

    public void updatePawns(Location loc){
        List<Player> localPlayers = loc.LocalPlayers;
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
        List<Player> localPlayers = player.CurLoc.LocalPlayers;
        if (localPlayers.Count > 1){
            updatePawns(player.CurLoc);
        }
        else{
            player.getBoardPawn().transform.position = player.CurLoc.transform.position;
        } 
        
    }

    public void updateActionCount(int count, int max){
        actionsCountText.text = count + "/" + max;
    }

    public void toggleCurPlayer(Player player){

        Image curPlayerIndicator = player.transform.GetComponentInChildren<Image>();
        curPlayerIndicator.enabled = !curPlayerIndicator.enabled;
    }

    public void toggleUndoActive(bool value){
        undoButton.SetActive(value);
    }
}
