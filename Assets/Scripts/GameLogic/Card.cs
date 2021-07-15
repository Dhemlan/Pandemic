using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    protected int id;
    protected string name;
    protected Location location;
    protected Vals.Colour colour;

    public Location Location { get => location; set => location = value; }
    public Vals.Colour Colour { get => colour; }
    public int ID { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
}
