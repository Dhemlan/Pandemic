using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public BoardUI boardUI;
    public OverlayUI overlayUI;
    public GameFlowManager gameFlowManager;
    public PlayerManager playerManager;
    public InteractablesManager interactablesManager;
    public OutbreakHandler outbreakHandler;
    
    private Stack<InfectionCard> infectionDeck = new Stack<InfectionCard>();
    private List<InfectionCard> infectionDiscardPile = new List<InfectionCard>();
    private Stack<InfectionCard> epidemicInfectionCards = new Stack<InfectionCard>();
    
    private Stack<PlayerCard> playerDeck = new Stack<PlayerCard>();
    private List<PlayerCard> playerDiscardPile = new List<PlayerCard>();
    private List<PlayerCard> availableEventCards = new List<PlayerCard>();

    private int[] infectionRateTrack = Vals.INFECTION_RATE_TRACK;
    private int epidemicCardCount;
    private bool gameOver = false;
    
    private int infectionRateTrackIndex = 0;
    private int outbreakCount = 0;
    private int[] diseaseCubeSupply = new int[] {Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT,Vals.INITIAL_DISEASE_CUBE_COUNT};
    private int[] diseaseStatus = new int[] {Vals.DISEASE_UNCURED,Vals.DISEASE_UNCURED,Vals.DISEASE_UNCURED,Vals.DISEASE_UNCURED};
    private int researchStationSupply = Vals.DEFAULT_RESEARCH_STATION_SUPPLY;

    public IEnumerator boardSetUp(int epidemicCardCount){
        this.epidemicCardCount = epidemicCardCount;
        BoardGenerator generator = gameObject.GetComponent<BoardGenerator>();
        yield return StartCoroutine(generator.generateBoard());
        
        //yield return StartCoroutine(infectCities());
        playerManager.generateCharacters(3);
        generator.preparePlayerCards();
        boardUI.boardSetUp(PlayerDeck.Count);
        overlayUI.gatherColliders(GetComponentsInChildren<CircleCollider2D>(), GetComponentsInChildren<BoxCollider2D>());
        yield break;
    }

    public void requestUserAcceptContinue(string message){
        Vals.continueGameFlow = false;
        overlayUI.requestPlayerContinue(message);
    }

    public IEnumerator addCube(InfectionCard card){
        Location loc = card.Location;
        yield return StartCoroutine(addCube(loc, loc.Colour));
    }

    public IEnumerator addCube(Location loc, Vals.Colour colour){
        if(loc.specificRoleHere(Vals.QUARANTINE_SPECIALIST) || (loc.specificRoleHere(Vals.MEDIC) && isDiseaseCured(colour))){
            yield break;
        }
        foreach(Location neighbour in loc.Neighbours){
            if(neighbour.specificRoleHere(Vals.QUARANTINE_SPECIALIST)){
                yield break;
            }
        }
        yield return StartCoroutine(outbreakHandler.checkForOutbreak(loc, colour, (outbreakOccurs) =>{
            if(!outbreakOccurs){
                if (DiseaseCubeSupply[(int)colour] == 0){
                   gameOver = true;       
                }
                else{
                    loc.addCube(colour);
                    Debug.Log("Adding cube in " + loc.retrieveLocName());
                    DiseaseCubeSupply[(int)colour]--;
                    boardUI.setCubeCount(colour, DiseaseCubeSupply[(int)colour]);
                    boardUI.addCube(loc, colour);
                }
            }
        }));
        if (gameOver) yield return StartCoroutine(gameFlowManager.gameOver(Vals.GameOver.CUBES));
    }

    public bool removeCube(Location loc, Vals.Colour colour){
        if (loc.removeCube(colour)){
            DiseaseCubeSupply[(int)colour]++;
            if(DiseaseStatus[(int)colour] == Vals.DISEASE_CURED && DiseaseCubeSupply[(int)colour] == Vals.INITIAL_DISEASE_CUBE_COUNT){
                eradicateDisease(colour);
            }
            Debug.Log("Removing cube in " + loc.retrieveLocName());
            boardUI.setCubeCount(colour, DiseaseCubeSupply[(int)colour]);
            boardUI.removeCube(loc, colour);
            return true;
        }
        Debug.Log("no cubes to remove");
        return false;
    }

    public void removeCubes(Location loc, Vals.Colour colour){
        while (removeCube(loc, colour)){};
    }

    public IEnumerator cure(int numberOfCardsRequired, Player player, Vals.Colour colourToDiscard){
        List<PlayerCard> discarded = new List<PlayerCard>();
        string message = Strings.DISCARD_PREFIX + numberOfCardsRequired + Strings.DISCARD_SUFFIX;
        yield return StartCoroutine(overlayUI.requestMultiSelect(player.getHand(), discarded, Vals.SELECTABLE_PLAYER_CARD, numberOfCardsRequired, colourToDiscard, message));
        DiseaseStatus[(int)colourToDiscard] = Vals.DISEASE_CURED;
        boardUI.updateDiseaseStatus(colourToDiscard, Vals.DISEASE_CURED);
        playerManager.cureOccurs(colourToDiscard);
        if(DiseaseCubeSupply[(int)colourToDiscard] == Vals.INITIAL_DISEASE_CUBE_COUNT){
            eradicateDisease(colourToDiscard);
        }
        playerManager.discardCards(player, discarded);
        playerManager.updateHand(player);
        playerManager.incrementCompletedActions();
        yield return StartCoroutine(checkCureVictoryCondition());
    }

    public void discardCard(PlayerCard card){
        PlayerDiscardPile.Add(card);
    }

    public void displayPlayerDiscard(){
        if (PlayerDiscardPile.Count == 0) StartCoroutine(overlayUI.displayToast(Strings.PLAYER_DISCARD_EMPTY, true));
        else StartCoroutine(overlayUI.displayItemsUntilClosed(PlayerDiscardPile, Vals.PLAYER_CARD, Strings.PLAYER_DISCARD));
    }

    public void displayInfectionDiscard(){
        if (InfectionDiscardPile.Count == 0) StartCoroutine(overlayUI.displayToast(Strings.INFECTION_DISCARD_EMPTY, true));
        else StartCoroutine(overlayUI.displayItemsUntilClosed(InfectionDiscardPile, Vals.INFECTION_CARD, Strings.INFECTION_DISCARD));
    }

    public void displayAvailableEvents(){
        if (AvailableEventCards.Count == 0) StartCoroutine(overlayUI.displayToast(Strings.AVAILABLE_EVENTS_EMPTY, true));
        else StartCoroutine(overlayUI.displayItemsUntilClosed(AvailableEventCards, Vals.EVENT_CARD, Strings.AVAILABLE_EVENTS));
    }

    public bool isDiseaseCured(Vals.Colour colour){
        return DiseaseStatus[(int)colour] > 0 ? true : false;
    }

    public IEnumerator buildResearchStation(Location loc){
        if (ResearchStationSupply == 0){
            yield return StartCoroutine(removeResearchStation());
        }
        loc.buildResearchStation();
        boardUI.toggleResearchStation(loc);
        ResearchStationSupply--;
        Debug.Log("research stations in supply "  + ResearchStationSupply);
        yield break;
    }

    public IEnumerator removeResearchStation(){
        Debug.Log("Remove a station");
        StartCoroutine(overlayUI.displayToast(Strings.REPLACE_RESEARCH_STATION, true));
        Vals.removeResearchStation = true;
        yield return new WaitUntil(() => !Vals.removeResearchStation);  
        //boardUI.toggleResearchStation()
        ResearchStationSupply++;
    }

    public void promptDrawPhase(){
        boardUI.activateDrawButton();
    }

    public void eventCardPlayed(PlayerCard eventCard){
        AvailableEventCards.Remove(eventCard);
        if (!eventCard.getRemoveAfterPlaying()){
            PlayerDiscardPile.Add(eventCard);
        }
        if(!playerManager.removeCardFromHand(eventCard, false)){
            playerManager.storedEventCardPlayed();
        }    
    }

    private void eradicateDisease(Vals.Colour colour){
        DiseaseStatus[(int)colour] = Vals.DISEASE_ERADICATED;
        boardUI.updateDiseaseStatus(colour, Vals.DISEASE_ERADICATED);
    }

    private IEnumerator checkCureVictoryCondition(){
        for (int i = 0; i < DiseaseStatus.Length; i++){
            if(DiseaseStatus[i] == Vals.DISEASE_UNCURED){
                yield break;
            }
        }
        yield return StartCoroutine(gameFlowManager.gameOver(Vals.GameOver.WIN));
    }

    public void removeInfectionCardFromDiscard(InfectionCard toRemove){
        InfectionDiscardPile.Remove(toRemove);
    }

    public List<PlayerCard> getEventsInDiscard(){
        List<PlayerCard> playerCards = new List<PlayerCard>();
        foreach(PlayerCard card in PlayerDiscardPile){
            if (card.Colour == Vals.Colour.EVENT){
                playerCards.Add(card);
            }
        }
        return playerCards;
    }

    public void removePlayerCardFromDiscard(PlayerCard toRemove){
        PlayerDiscardPile.Remove(toRemove);
    }

    public bool specificEventAvailable(string timing){
        List<int> eventIDs = new List<int>();
        switch(timing){
            case "intensify":
                eventIDs.Add(Vals.RESILIENT_POPULATION);
                break;
            case "infection":
                if (!boardUI.confirmInfectionPhase()){
                    return false;
                }
                eventIDs.Add(Vals.RESILIENT_POPULATION);
                eventIDs.Add(Vals.ONE_QUIET_NIGHT);
                // if medic or qspecialist
                eventIDs.Add(Vals.FORECAST);
                break;
        }
        foreach(PlayerCard card in AvailableEventCards){
            if (eventIDs.Contains(card.ID)) return true;
        }
        return false;
    }

    public void reloadBoard(){
        boardUI.updateDiseaseCubeSupply(diseaseCubeSupply);
        for(int i = 0; i < diseaseStatus.Length; i++){
            boardUI.updateDiseaseStatus((Vals.Colour)i, diseaseStatus[i]);
        }
        foreach (Location loc in GameObject.Find("Locations").GetComponentsInChildren<Location>()){
            boardUI.toggleResearchStation(loc);
            boardUI.updateCubes(loc);
        }
    }

    public InfectionCard drawInfectionCard(){
        return infectionDeck.Pop();
    }

    public InfectionCard drawInfectionCardFromBottom(){
        return epidemicInfectionCards.Pop();
    }

    public void discardInfectionCard(InfectionCard card){
        infectionDiscardPile.Add(card);
    }

    public PlayerCard drawPlayerCard(){
        try {
            return playerDeck.Pop();
        }
        catch(InvalidOperationException){
            StartCoroutine(gameFlowManager.gameOver(Vals.GameOver.CARDS));
            return null;
        }
    }

    public void eventCardDrawn(PlayerCard eventCard){
        availableEventCards.Add(eventCard);
    }

    public void intensifyEpidemicStep(){
        Utils.ShuffleAndPlaceOnTop(InfectionDiscardPile, InfectionDeck);
    }

    public bool isEradicated(Vals.Colour colour){
        return (diseaseStatus[(int)colour] < Vals.DISEASE_ERADICATED) ? false : true;
    }

    public int currentInfectionRate(){
        return infectionRateTrack[infectionRateTrackIndex];
    }

    public int InfectionRateTrackIndex { get => infectionRateTrackIndex; set => infectionRateTrackIndex = value; }
    public int OutbreakCount { get => outbreakCount; set => outbreakCount = value; }
    public int[] DiseaseCubeSupply { get => diseaseCubeSupply; set => diseaseCubeSupply = value; }
    public int[] DiseaseStatus { get => diseaseStatus; set => diseaseStatus = value; }
    public int ResearchStationSupply { get => researchStationSupply; set => researchStationSupply = value; }
    public List<PlayerCard> PlayerDiscardPile { get => playerDiscardPile; set => playerDiscardPile = value; }
    public List<InfectionCard> InfectionDiscardPile { get => infectionDiscardPile; set => infectionDiscardPile = value; }
    public List<PlayerCard> AvailableEventCards { get => AvailableEventCards1; set => AvailableEventCards1 = value; }
    public int EpidemicCardCount { get => epidemicCardCount; set => epidemicCardCount = value; }
    public Stack<InfectionCard> InfectionDeck { get => infectionDeck; set => infectionDeck = value; }
    public Stack<InfectionCard> EpidemicInfectionCards { get => epidemicInfectionCards; set => epidemicInfectionCards = value; }
    public Stack<PlayerCard> PlayerDeck { get => playerDeck; set => playerDeck = value; }
    public List<PlayerCard> AvailableEventCards1 { get => availableEventCards; set => availableEventCards = value; }
}
