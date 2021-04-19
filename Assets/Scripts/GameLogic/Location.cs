using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public LocationUI locUi;
    int[] diseaseCubes = {0,0,0,0};
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void addCube(ConstantVals.Colour Colour){
        diseaseCubes[(int)Colour]++;
        //locUi.addCube(Colour);
        Debug.Log("Cube added in " + gameObject.name + ": Count = " + diseaseCubes[0]);
    }

    public bool checkOutbreak(ConstantVals.Colour Colour){
        if (diseaseCubes[(int)Colour] == ConstantVals.OUTBREAK_THRESHOLD){
            Debug.Log("Outbreak");
            return true;
        }
        return false;
    }

    public string getName(){
        return gameObject.name;
    }
}
