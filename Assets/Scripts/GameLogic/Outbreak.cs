using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outbreak
{
    private Location location;
    private Vals.Colour colour;

    public Outbreak(Location loc, Vals.Colour colour){
        this.location = loc;
        this.colour = colour;
    }

    public Vals.Colour Colour { get => colour;}
    public Location Location { get => location;}
}
