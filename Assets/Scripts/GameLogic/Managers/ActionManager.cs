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
    public BoardUI boardUI;

    private Vals.Colour playerSelectedCube;

    public delegate void TreatEventHandler();
    public event TreatEventHandler treatActionClicked;
    
    public IEnumerator handleLocClick(Location loc){
        if (Vals.removeResearchStation){
            if(loc.ResearchStationStatus){
                loc.removeResearchStation();
                boardUI.toggleResearchStation(loc);
                Vals.removeResearchStation = false;
            }   
        }
        else if (Vals.cardResolving == Vals.AIRLIFT){
            playerManager.airlift(loc);
            Vals.cardResolving = -1;
            Vals.continueGameFlow = true;
        }
        else if(Vals.cardResolving == Vals.GOVERNMENT_GRANT){
            if (!loc.ResearchStationStatus){
                yield return StartCoroutine(board.buildResearchStation(loc));
                Vals.cardResolving = -1;
                Vals.continueGameFlow = true;
            }
        }
        else if (gameFlowManager.getPhase() == Vals.Phase.ACTION && playerManager.actionAvailable()){
            Debug.Log(loc.retrieveLocName() + " clicked");
            Player player = playerManager.getCurPlayer();
            if (player.getRoleID() == Vals.DISPATCHER && playerManager.getUserSelectedPlayer() != null){
                StartCoroutine(playerManager.potentialPlayerMovement(player, loc));
            }
            else if (player.CurLoc.Equals(loc)){
               StartCoroutine(handleTreatAction(player, loc)); 
            }
            else {
                StartCoroutine(playerManager.potentialPlayerMovement(player, loc));
            }
        }
        yield break;
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
                    StartCoroutine(handleTreatAction(actionTaker, actionTaker.CurLoc));
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
                    Nullable<Vals.Colour> colourToDiscard = actionTaker.determineCureActionAvailable(actionTaker.CurLoc.ResearchStationStatus);    
                    if (colourToDiscard != null){
                        int cardsToDiscard = actionTaker.roleModifiedCardsToCure(Vals.DEFAULT_CARDS_TO_CURE);
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

    public IEnumerator handleCharacterActionClick(Player player){
        if (playerManager.getCurPlayer() == player && gameFlowManager.getPhase() == Vals.Phase.ACTION && playerManager.actionAvailable()){
            switch (player.getRoleID()){
                case Vals.CONTINGENCY_PLANNER:
                ContingencyPlanner role = (ContingencyPlanner) player.getRole();
                    if (role.eventCardStored()){
                        StartCoroutine(overlayUI.displayToast(Strings.EVENT_STORED_ALREADY, true));
                        yield break;
                    }
                    List<PlayerCard> selected = new List<PlayerCard>();
                    List<PlayerCard> discardedEvents = board.getEventsInDiscard();
                    if (discardedEvents.Count == 0){
                        StartCoroutine(overlayUI.displayToast(Strings.NO_EVENTS_IN_DISCARD, true));
                        yield break;
                    }
                    yield return StartCoroutine(playerManager.requestUserSelectCard(discardedEvents, selected, 1, null));
                    PlayerCard selectedEvent = selected[0];
                    board.removePlayerCardFromDiscard(selectedEvent);
                    board.eventCardDrawn(selectedEvent);
                    yield return StartCoroutine(role.characterAction(selectedEvent));

                    playerManager.incrementCompletedActions();
                    break;

                case Vals.DISPATCHER:
                    string message = Strings.SELECT_PLAYER_MOVE;
                    yield return StartCoroutine(playerManager.requestUserSelectPlayerToInteract(playerManager.nonCurrentPlayers(), message));
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
