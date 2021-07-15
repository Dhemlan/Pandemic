using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObject
{    
    //board elements
    public int[] diseaseCubeSupply;
    public int[] diseaseStatus;
    public int infectionRate;
    public int outbreaks;
    public int researchStationSupply;

    public SerializableLocation[] serializableLocations;

    public SaveObject(Board board, Location[] locs){//List<Location> locs){
        diseaseCubeSupply = board.DiseaseCubeSupply;
        diseaseStatus = board.DiseaseStatus;
        researchStationSupply = board.ResearchStationSupply;

        List<SerializableLocation> locations = new List<SerializableLocation>();
        foreach(Location loc in locs){
            locations.Add(new SerializableLocation(loc.Id, loc.ResearchStationStatus, loc.DiseaseCubes));
        }
        serializableLocations = locations.ToArray();

    }

    public void loadValues(Board board){
        board.DiseaseCubeSupply = diseaseCubeSupply;
        board.DiseaseStatus = diseaseStatus;
        board.ResearchStationSupply = researchStationSupply;

        board.reloadBoard();
    }
}
