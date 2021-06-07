﻿using System.Collections;
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

        if (Vals.removeResearchStation){
            if(loc.getResearchStationStatus()){
                loc.removeResearchStation();
                Vals.removeResearchStation = false;
            }   
        }
        else if (Vals.cardResolving == Vals.AIRLIFT){
            playerManager.airlift(loc);
            Vals.cardResolving = -1;
        }
        else if(Vals.cardResolving == Vals.GOVERNMENT_GRANT){
            if (!loc.getResearchStationStatus()){
                board.buildResearchStation(loc);
                Vals.cardResolving = -1;
            }
        }
        else if (gameFlowManager.getPhase() == Vals.Phase.ACTION && playerManager.actionAvailable()){
            Debug.Log(loc.getName() + " clicked");
            Player player = playerManager.getCurPlayer();
            if (player.getLocation().Equals(loc)){
               StartCoroutine(handleTreatAction(player, loc)); 
            }
            else {
                StartCoroutine(playerManager.potentialPlayerMovement(player, loc));
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
                    yield return StartCoroutine(actionTaker.buildAction());
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

    public void handlePresentedCardClick (bool selected, Card card){
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
