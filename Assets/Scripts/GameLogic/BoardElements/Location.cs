using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public ActionManager actionManager;
    public ConstantVals.Colour colour;
    int[] diseaseCubes = {0,0,0,0};
    public List<Location> neighbours = new List<Location>();

    private bool researchStation = false;

    public void OnMouseDown(){
        actionManager.handleLocClick(this);
    }

    public void addCube(ConstantVals.Colour colour){
        diseaseCubes[(int)colour]++;
        //locUi.addCube(Colour);
        //Debug.Log(colour + " Cube added in " + gameObject.name + ": yellow " + diseaseCubes[0] +" blue " + diseaseCubes[1]);
    }

    public bool removeCube(){
        if (diseaseCubes[(int) colour] == 0){
            return false;
        } 
        diseaseCubes[(int) colour]--;
        return true;
    }

    public bool checkOutbreak(ConstantVals.Colour colour){
        if (diseaseCubes[(int)colour] == ConstantVals.OUTBREAK_THRESHOLD){
            return true;
        }
        return false;
    }

    public string getName(){
        return gameObject.name;
    }

    public ConstantVals.Colour getColour(){
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
}
