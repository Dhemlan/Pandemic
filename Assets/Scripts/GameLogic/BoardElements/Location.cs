using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour, IPointerClickHandler 
{
    public ActionManager actionManager;
    public Vals.Colour colour;
    int[] diseaseCubes = {0,0,0,0};
    public List<Location> neighbours = new List<Location>();
    private List<Player> localPlayers = new List<Player>();

    private bool researchStation = false;

    public void OnPointerClick(PointerEventData data) {
         // This will only execute if the objects collider was the first hit by the click's raycast
         
     }

    public void OnMouseDown(){
        actionManager.handleLocClick(this);
    }

    public void addCube(Vals.Colour colour){  
        diseaseCubes[(int)colour]++;
    }

    public bool qSpecialistHere(){
        foreach (Player player in localPlayers){
            if (player.getRoleID() == Vals.QUARANTINE_SPECIALIST){
                return true;
            }
        }
        return false;
    }

    public bool removeCube(){
        if (diseaseCubes[(int) colour] == 0){
            return false;
        } 
        diseaseCubes[(int) colour]--;
        return true;
    }

    public bool checkOutbreak(Vals.Colour colour){
        if (diseaseCubes[(int)colour] == Vals.OUTBREAK_THRESHOLD){
            return true;
        }
        return false;
    }

    public string getName(){
        return gameObject.name;
    }

    public Vals.Colour getColour(){
        return colour;
    }

    public void buildResearchStation(){
        researchStation = true;
        Debug.Log("Building research station" + gameObject.name);
    }

    public bool getResearchStationStatus(){
        return researchStation;
    }

    public List<Location> getNeighbours(){
        return neighbours;
    }

    public bool hasNeighbour (Location loc){
        return neighbours.Contains(loc);
    }
    
    public void playerEnters(Player player){
        localPlayers.Add(player);
        Debug.Log(player.getTurnOrderPos() + " enters " + name + ": " + localPlayers.Count + " are here");
    }

    public void playerLeaves(Player player){
        localPlayers.Remove(player);
        Debug.Log(player.getTurnOrderPos() + " leaves " + name + ": " + localPlayers.Count + " remain");
    }

    public List<Player> getLocalPlayers(){
        return localPlayers;
    }
    /*
    public bool hasMultipleDiseases(){
        return diseaseCubes[0] ^ diseaseCubes[1] ^ diseaseCubes[2] ^ diseaseCubes[3];
    } */
}
