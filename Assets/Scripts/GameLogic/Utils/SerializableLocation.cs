using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializableLocation
{   
    private int id;
    //private bool researchStation;
    private int[] cubes;

    public SerializableLocation(int id, bool reseachStation, int[] cubes){
        this.id = id;
       // this.researchStation = ResearchStation;
        this.cubes = cubes;
    }

    public int Id { get => id; set => id = value; }
    //public bool ResearchStation { get => researchStation; set => researchStation = value; }
    public int[] Cubes { get => cubes; set => cubes = value; }
}
