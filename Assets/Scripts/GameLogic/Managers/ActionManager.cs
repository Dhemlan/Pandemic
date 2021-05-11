using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public GameFlowManager gameFlowManager;
    public PlayerManager playerManager;
    public Board board;
    
    public void handleLocClick(Location loc){
        if (gameFlowManager.getPhase() == ConstantVals.Phase.ACTION && playerManager.actionAvailable()){
            Debug.Log(loc.getName() + " clicked");
            Player player = playerManager.getCurPlayer();
            if (player.getLocation().Equals(loc)){
                Debug.Log("at location");
                board.removeCube(loc);
                playerManager.incrementCompletedActions();
            }
    
            else {
                Debug.Log("not here");
                if (player.getLocation().getResearchStationStatus() && loc.getResearchStationStatus()){
                    Debug.Log("shuttle flight");
                    player.setCurLoc(loc);
                    playerManager.incrementCompletedActions();
                }
                else if (player.getLocation().hasNeighbour(loc)){
                    Debug.Log("drive/ferry");
                    player.setCurLoc(loc);
                    playerManager.incrementCompletedActions();
                }
                else {
                    List<Location> handLocs = new List<Location>();
                    player.getLocationsInHand(handLocs);
                    bool hasCurrentLoc = handLocs.Contains(player.getLocation());
                    bool hasClickedLoc = handLocs.Contains(loc);
                    if (hasCurrentLoc && hasClickedLoc){
                        // request choice
                        Debug.Log("choose to discard");
                        player.setCurLoc(loc);
                        playerManager.incrementCompletedActions();
                    }
                    else if (hasCurrentLoc){
                        // discard src
                        Debug.Log("charter");

                        player.setCurLoc(loc);
                        playerManager.incrementCompletedActions();

                    }
                    else if (hasClickedLoc){
                        //discard dest
                        Debug.Log("commerical");
                        player.setCurLoc(loc);
                        playerManager.incrementCompletedActions();
                    }
                }
                playerManager.placePawn(player);
            }
            Debug.Log("action handled");
            
        }
    }

    public void handleActionButtonClick(string action){
        if (gameFlowManager.getPhase() == ConstantVals.Phase.ACTION && playerManager.actionAvailable()){
            Player actionTaker = playerManager.playerPerformingAction();
            switch (action)
            {
                case "MoveAction" :
                    Debug.Log("Move");
                    playerManager.incrementCompletedActions();
                    break;
                case "TreatAction":
                    Debug.Log("Treat");
                    playerManager.incrementCompletedActions();
                    break;
                case "ShareAction":
                    break;
                case "BuildAction":
                    Debug.Log("Build");
                    playerManager.incrementCompletedActions();
                    break;
                case "CureAction":
                    Debug.Log("Cure");
                    int cardsToDiscard = actionTaker.attemptCure(board.retrieveCureRequirements(), true );//actionTaker.getLocation().getResearchStation());
                    if (cardsToDiscard > 0){
                        StartCoroutine(board.cure(cardsToDiscard, actionTaker));
                    }
                    playerManager.incrementCompletedActions();

                    break;
                case "PassAction":
                    playerManager.endActionPhase();
                    break;
            }
        }
    }
}
