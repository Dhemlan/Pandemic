using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayableInfectionCard : MonoBehaviour, ISelectable<InfectionCard>
{
    private InfectionCard card;
    public Text label;
    public Sprite[] sprites;

    public void populateItemData(InfectionCard card){
        this.card = card;
        label.text = card.Name;
    }

    public Sprite getSprite(){
        return sprites[(int)card.Colour];
    }

    public bool isSelected(){
       return false;
    }

    public InfectionCard getSelectedValue(){
       return card;
    }
}
