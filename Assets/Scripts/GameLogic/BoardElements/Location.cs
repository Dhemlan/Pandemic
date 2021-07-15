using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour
{
    public Vals.Colour colour;
    public List<Location> neighbours = new List<Location>();
    private List<Player> localPlayers = new List<Player>();
    private int id;
    private int[] diseaseCubes = {0,0,0,0};
    private bool researchStationStatus = false;

    public void addCube(Vals.Colour colour){  
        DiseaseCubes[(int)colour]++;
    }

    public bool removeCube(Vals.Colour colourToTreat){
        if (DiseaseCubes[(int)colourToTreat] == 0){
            return false;
        } 
        DiseaseCubes[(int)colourToTreat]--;
        return true;
    }

    public List<Vals.Colour> diseasesActiveHere(){
        List<Vals.Colour> activeDiseases = new List<Vals.Colour>();
        for (int i = 0; i < DiseaseCubes.Length; i++){
            if (DiseaseCubes[i] > 0){
                activeDiseases.Add((Vals.Colour)i);
            }
        }
        return activeDiseases;
    } 

    public int getDiseaseCubeCount(Vals.Colour colour){
        return diseaseCubes[(int)colour];
    }

    public string retrieveLocName(){
        return gameObject.name;
    }

    public void buildResearchStation(){
        researchStationStatus = true;
        Debug.Log("Building research station" + gameObject.name);
    }

    public void removeResearchStation(){
        researchStationStatus = false;
    }
    
    public bool specificRoleHere(int roleID){
        foreach (Player player in LocalPlayers){
            if (player.getRoleID() == roleID){
                return true;
            }
        }
        return false;
    }

    public bool hasNeighbour (Location loc){
        return Neighbours.Contains(loc);
    }
    
    public void playerEnters(Player player){
        LocalPlayers.Add(player);
    }

    public void playerLeaves(Player player){
        LocalPlayers.Remove(player);
    }
    
    public int[] DiseaseCubes { get => diseaseCubes; set => diseaseCubes = value; }
    public int Id { get => id; set => id = value; }
    public bool ResearchStationStatus { get => researchStationStatus; set => researchStationStatus = value; }
    public List<Player> LocalPlayers { get => localPlayers; set => localPlayers = value; }
    public Vals.Colour Colour { get => colour; set => colour = value; }
    public List<Location> Neighbours { get => neighbours; set => neighbours = value; }
}
