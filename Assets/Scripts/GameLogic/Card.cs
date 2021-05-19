using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    protected int id;
    protected string name;
    protected Location loc;
    protected Vals.Colour colour;

    public string getName(){
        return name;
    }

    public int getId(){
        return id;
    }

    public Location getLocation(){
        return loc;
    }

    public Vals.Colour getColour(){
        return colour;
    }
}
