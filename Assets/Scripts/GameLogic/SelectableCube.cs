using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableCube : MonoBehaviour, ISelectable<Vals.Colour>
{
    public Sprite[] sprites;
    private Vals.Colour colour;

    public void OnMouseDown(){
        GameObject.Find("ActionManager").GetComponent<ActionManager>().setPlayerSelectedCube(colour);
        Vals.continueAfterSelect = true;
    }

    public void populateItemData(Vals.Colour colour){
        this.colour = colour;
    }

    public Sprite getSprite(){
        return sprites[(int) colour];
    }

    public bool isSelected(){
       return false;
    }

    public Vals.Colour getSelectedValue(){
       return colour;
    }
}


