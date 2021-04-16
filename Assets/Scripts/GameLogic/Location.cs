using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    
    int[] diseaseCubes = {0,0,0,0};
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void addCube(int colour){
        diseaseCubes[colour]++;
    }

    public boolean checkOutbreak(int colour){
        if (diseaseCubes[colour] == ConstantVals.OUTBREAK_THRESHOLD){
            return true;
        }
        return false;
    }

    public string getName(){
        return gameObject.name;
    }
}
