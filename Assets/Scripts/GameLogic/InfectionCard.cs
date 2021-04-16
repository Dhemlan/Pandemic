using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionCard : Card
{

    public InfectionCard(int id, string name){
        this.id = id;
        this.name = name;
    }

    public string getName(){
        return name;
    }

    public int getId(){
        return id;
    }

    public string toString(){
        return name;
    }

}
