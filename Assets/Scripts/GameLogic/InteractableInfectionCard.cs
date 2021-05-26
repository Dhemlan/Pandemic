using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableInfectionCard : MonoBehaviour, ISelectable<InfectionCard>
{
    private InfectionCard card;
    private SpriteRenderer spriteRen;
    public GameObject selectedIcon;
    public Text label;
    private ActionManager actionManager;
    public Sprite[] sprites;

    void Awake(){
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
    }

    public void OnMouseDown(){
        //selectedIcon.SetActive(!selectedIcon.activeSelf);
       // actionManager.handlePresentedPlayerCardClick(selectedIcon.activeSelf, card);
    }
    
    public void populateItemData(InfectionCard card){
        this.card = card;
        label.text = card.getName();
    }

    public InfectionCard getSelectedValue(){
        return card;
    }

    public Sprite getSprite(){
        return sprites[(int)card.getColour()];
    }

    public bool isSelected(){
        return selectedIcon.activeSelf;
    }
}