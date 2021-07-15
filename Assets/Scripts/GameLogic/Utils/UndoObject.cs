using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UndoObject : MonoBehaviour
{
    private int[] diseaseCubeSupply = new int[Vals.DISEASE_COUNT];
    private int[] diseaseStatus = new int[Vals.DISEASE_COUNT];
    private int researchStationSupply;

    public Board board;
    public PlayerManager playerManager;

    public Stack<InfectionCard> infectionDeck = new Stack<InfectionCard>();
    private List<PlayerCard> playerDiscard = new List<PlayerCard>();
    private List<InfectionCard> infectionDiscard = new List<InfectionCard>();
    private List<PlayerCard> availableEvents = new List<PlayerCard>();

    private List<Location> playerLocs = new List<Location>();
    private List<List<PlayerCard>> playerHands = new List<List<PlayerCard>>();
    private List<int[]> locCubes = new List<int[]>();
    private List<List<Player>> locPlayers = new List<List<Player>>();
    private List<bool> locResearchStations = new List<bool>();

    private PlayerCard eventCardStored;

    public void recordGameState(){
        clearData();
        Debug.Log("Recording state");
        // board elements
        for (int i = 0; i < diseaseStatus.Length; i++){
            diseaseCubeSupply[i] = board.DiseaseCubeSupply[i];
            diseaseStatus[i] = board.DiseaseStatus[i];
        }
        researchStationSupply = board.ResearchStationSupply;
        playerDiscard.AddRange(board.PlayerDiscardPile);
        infectionDiscard.AddRange(board.InfectionDiscardPile);
        availableEvents.AddRange(board.AvailableEventCards);
        //infectionDeck = board.InfectionDeck.Clone();

        foreach(Player player in playerManager.getPlayers()){
            playerLocs.Add(player.CurLoc);
           // Debug.Log(player.CurLoc);
            List<PlayerCard> hand = new List<PlayerCard>();
            hand.AddRange(player.Hand);
            playerHands.Add(hand);
            if (player.getRoleID() == Vals.CONTINGENCY_PLANNER){
                ContingencyPlanner planner = (ContingencyPlanner)player.getRole();
                eventCardStored = planner.StoredEventCard;
            }
        }

        foreach (Location loc in GameObject.Find("Locations").GetComponentsInChildren<Location>()){
            locResearchStations.Add(loc.ResearchStationStatus);
            List<Player> localPlayers = new List<Player>();
            localPlayers.AddRange(loc.LocalPlayers);
            locPlayers.Add(localPlayers);
            int[] copiedCubes = new int[Vals.DISEASE_COUNT];
            Array.Copy(loc.DiseaseCubes, copiedCubes, copiedCubes.Length);
            locCubes.Add(copiedCubes);
        }
        Debug.Log("Recording done");
    }

    public void undo(){
        // cna't simply pass back lists/arrays
        Debug.Log("undoing");
        // board elements
        for (int i = 0; i < diseaseCubeSupply.Length; i++){
            board.DiseaseCubeSupply[i] = diseaseCubeSupply[i];
            board.DiseaseStatus[i] = diseaseStatus[i];
        }
        board.ResearchStationSupply = researchStationSupply;
        //board.InfectionDeck = infectionDeck;
        List<PlayerCard> copiedPlayerDiscard = new List<PlayerCard>();
        copiedPlayerDiscard.AddRange(playerDiscard);
        board.PlayerDiscardPile = copiedPlayerDiscard;

        List<InfectionCard> copiedInfectionDiscard = new List<InfectionCard>();
        copiedInfectionDiscard.AddRange(infectionDiscard);
        board.InfectionDiscardPile = copiedInfectionDiscard;

        List<PlayerCard> copiedEvents = new List<PlayerCard>();
        copiedEvents.AddRange(availableEvents);
        board.AvailableEventCards = copiedEvents;
        

        Location[] locs = GameObject.Find("Locations").GetComponentsInChildren<Location>();
        for (int i = 0; i < locs.Length; i++){
            locs[i].ResearchStationStatus = locResearchStations[i];
            Array.Copy(locCubes[i], locs[i].DiseaseCubes, locCubes[i].Length);

            List<Player> copiedPlayers = new List<Player>();
            copiedPlayers.AddRange(locPlayers[i]);
            locs[i].LocalPlayers = copiedPlayers;
        }

        playerManager.CompletedActions = 0;
        playerManager.getCurPlayer().resetOncePerTurnActions();
        
        int j = 0;
        foreach (Player player in playerManager.getPlayers()){
            player.CurLoc = playerLocs[j];
           // Debug.Log(player.Hand.Count);
            List<PlayerCard> newHand = new List<PlayerCard>();
            newHand.AddRange(playerHands[j]);
            player.Hand = newHand;
            if (player.getRoleID() == Vals.CONTINGENCY_PLANNER){
                ContingencyPlanner planner = (ContingencyPlanner)player.getRole();
                planner.StoredEventCard = eventCardStored;
            }
           // Debug.Log(player.Hand.Count);
            j++;
        }
        board.reloadBoard();
        playerManager.reloadPlayerUI();
    }

    public void clearData(){
        playerDiscard.Clear();
        availableEvents.Clear();
        infectionDiscard.Clear();
        playerLocs.Clear();
        playerHands.Clear();
        locCubes.Clear();
        locPlayers.Clear();
        locResearchStations.Clear();

    }
}
