using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    private Location[] locations;
    public GameObject locationsParent;

    public Board board;
    public PlayerManager playerManager;
    public SetUpCityInfector setUpCityInfector;
    
    
    public IEnumerator generateBoard(){
        InfectionDeckGenerator infectionDeckGenerator = new InfectionDeckGenerator(board);
        infectionDeckGenerator.createInfectionDeck();
        infectionDeckGenerator.setAsideEpidemicInfectionCards();
        setUpLocations();
        yield return StartCoroutine(buildInitialResearchStations(board));
        yield return StartCoroutine(initialInfection());
    }

    private void setUpLocations(){
        locations = GameObject.Find("Locations").GetComponentsInChildren<Location>();
    }

    public IEnumerator buildInitialResearchStations(Board board){
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.ATLANTA]));
        //yield return StartCoroutine(board.buildResearchStation(locations[Vals.BANGKOK]));
        /*yield return StartCoroutine(board.buildResearchStation(locations[Vals.SYDNEY]));
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.LIMA]));
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.PARIS]));
        yield return StartCoroutine(board.buildResearchStation(locations[Vals.ESSEN]));*/
    }

    public IEnumerator initialInfection(){
        yield return StartCoroutine(setUpCityInfector.initialInfection());
    }

    public void preparePlayerCards(){
        PlayerDeckGenerator playerDeckGenerator = new PlayerDeckGenerator(board, playerManager);
        playerDeckGenerator.preparePlayerCards(locations);
    }

}
