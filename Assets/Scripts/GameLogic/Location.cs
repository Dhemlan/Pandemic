using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public LocationUI locUi;
    public ConstantVals.Colour colour;
    int[] diseaseCubes = {0,0,0,0};
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void addCube(ConstantVals.Colour colour){
        diseaseCubes[(int)colour]++;
        //locUi.addCube(Colour);
        //Debug.Log(colour + " Cube added in " + gameObject.name + ": yellow " + diseaseCubes[0] +" blue " + diseaseCubes[1]);
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
}
