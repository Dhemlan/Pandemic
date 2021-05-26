using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionManager : MonoBehaviour
{
    public GameFlowManager gameFlowManager;
    public PlayerManager playerManager;
    public PlayerUI playerUI;
    public Board board;
    public CardUI cardUI;
    
    public void handleLocClick(Location loc){
        if (gameFlowManager.getPhase() == Vals.Phase.ACTION && playerManager.actionAvailable()){
            Debug.Log(loc.getName() + " clicked");
            Player player = playerManager.getCurPlayer();
            if (player.getLocation().Equals(loc)){
                // request disease colour if required
                Debug.Log("at location");
                //request cure status and cube status
                // need to bypass no cubes scenario
                Debug.Log("Treat");
                //Debug.Log(loc.hasMultipleDiseases());
                player.treatAction(loc);
            }
    
            else {
                Debug.Log("not here");
                if (player.getLocation().getResearchStationStatus() && loc.getResearchStationStatus()){
                    Debug.Log("shuttle flight");
                    player.getLocation().playerLeaves(player);
                    player.shuttleFlightAction(loc);
                    loc.playerEnters(player);

                }
                else if (player.isDriveFerryValid(loc)){
                    Debug.Log("drive/ferry");
                    player.getLocation().playerLeaves(player);
                    player.driveFerryAction(loc);
                    loc.playerEnters(player);
                }
                else {
                    Location curLoc = player.getLocation();
                    if (player.otherMovement(loc)){
                        curLoc.playerLeaves(player);
                        playerUI.updateHand(player, player.getHand());
                        loc.playerEnters(player);  
                    }
                }
                playerManager.placePawn(player);
            }
            Debug.Log("action handled");  
        }
    }

    public void handleActionButtonClick(string action){
        if (gameFlowManager.getPhase() == Vals.Phase.ACTION && playerManager.actionAvailable()){
            Player actionTaker = playerManager.playerPerformingAction();
            switch (action)
            {
                case "MoveAction" :
                    Debug.Log("Move");     
                    break;
                case "TreatAction":
                    Debug.Log("Treat");
                    
                    actionTaker.treatAction(actionTaker.getLocation());
                    break;
                case "ShareAction":
                    Debug.Log("share");
                    Player otherPlayer = actionTaker.shareAction();
                    if (otherPlayer != null){
                        playerUI.updateHand(actionTaker, actionTaker.getHand());
                        //yield return StartCoroutine(playerManager.checkHandLimit(actionTaker));
                        playerUI.updateHand(otherPlayer, otherPlayer.getHand());
                        //yield return StartCoroutine(playerManager.checkHandLimit(otherPlayer));
                    }
                    
                    break;
                case "BuildAction":
                    Debug.Log("Build");
                    actionTaker.buildAction();
                    playerUI.updateHand(actionTaker, actionTaker.getHand());
                    break;
                case "CureAction":
                    Debug.Log("Cure");
                    Nullable<Vals.Colour> colourToDiscard = actionTaker.determineCureActionAvailable(board.retrieveCureRequirements(), actionTaker.getLocation().getResearchStationStatus());
                    
                    if (colourToDiscard != null){
                        int cardsToDiscard = actionTaker.roleModifiedCardsToCure(board.retrieveCureRequirements()[(int)colourToDiscard]);
                        StartCoroutine(board.cure(cardsToDiscard, actionTaker, colourToDiscard.Value));
                    }

                    break;
                case "PassAction":
                    playerManager.endActionPhase();
                    break;
            }
        }
        //yield break;
    }

    public void handlePresentedPlayerCardClick (bool selected, PlayerCard card){
        if(selected){
            cardUI.adjustDiscardRequiredCount(1, card);
        }
        else{
            cardUI.adjustDiscardRequiredCount(-1, card);
        }
    }
}
