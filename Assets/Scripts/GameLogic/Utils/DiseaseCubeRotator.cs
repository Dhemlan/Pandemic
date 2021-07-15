using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseCubeRotator : MonoBehaviour
{   
    private int speed = 45;
    private Vals.Colour colour;

    public Vals.Colour Colour { get => colour; set => colour = value; }

    void Update()
    {
        transform.Rotate(new Vector3(0,0, 75 + speed / 3) * Time.deltaTime);
        transform.RotateAround(transform.parent.transform.position, new Vector3(0,0,1), speed * Time.deltaTime);
    }

    public void setSpeed(int urgency){
        int speedFactor = 45;
        if (urgency > 2) speed = speedFactor * 3;
        else speed = speedFactor * urgency;
    }
}
