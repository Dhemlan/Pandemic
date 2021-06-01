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
    public OverlayUI overlayUI;

    private Vals.Colour playerSelectedCube;

    public delegate void TreatEventHandler();
    public event TreatEventHandler treatActionClicked;
    
    public void handleLocClick(Location loc){
        if (gameFlowManager.getPhase() == Vals.Phase.ACTION && playerManager.actionAvailable()){
            Debug.Log(loc.getName() + " clicked");
            Player player = playerManager.getCurPlayer();
            if (player.getLocation().Equals(loc)){
               StartCoroutine(handleTreatAction(player, loc)); 
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
        }
    }

    public IEnumerator handleTreatAction(Player player, Location loc){
        Debug.Log("at location");
        List<Vals.Colour> activeDiseases = loc.diseasesActiveHere();
        Vals.Colour colourToTreat;
        switch(activeDiseases.Count){
            case 0: 
                yield break;
            case 1:
                colourToTreat = activeDiseases[0];
                break;
            default:
                Debug.Log("multiple diseases here - select please");
                string message = "Select disease to treat";
                yield return StartCoroutine(overlayUI.requestSimpleSelectionFromPlayer(activeDiseases, Vals.SELECTABLE_DISEASE, message));
                colourToTreat = playerSelectedCube;
                break;
        }
        player.treatAction(loc, colourToTreat);
        Debug.Log("Treat: " + colourToTreat);
        yield break;
    }

    public IEnumerator handleActionButtonClick(string action){
        if (gameFlowManager.getPhase() == Vals.Phase.ACTION && playerManager.actionAvailable()){
            Player actionTaker = playerManager.playerPerformingAction();
            switch (action)
            {
                case "MoveAction" :
                    Debug.Log("Move");     
                    break;
                case "TreatAction":
                    StartCoroutine(handleTreatAction(actionTaker, actionTaker.getLocation()));
                    break;
                case "ShareAction":
                    Debug.Log("share");
                    StartCoroutine(actionTaker.shareAction());        
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
        yield break;
    }

    public void handlePresentedPlayerCardClick (bool selected, PlayerCard card){
        if(selected){
            overlayUI.adjustDiscardRequiredCount(1, card);
        }
        else{
            overlayUI.adjustDiscardRequiredCount(-1, card);
        }
    }

    public void setPlayerSelectedCube(Vals.Colour colour){
        playerSelectedCube = colour;
    }
}
