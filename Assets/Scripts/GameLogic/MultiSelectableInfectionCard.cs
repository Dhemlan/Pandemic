using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelectableInfectionCard : MonoBehaviour, ISelectable<InfectionCard>
{
    private InfectionCard card;
    public GameObject selectedIcon;
    public Text label;
    private ActionManager actionManager;
    public Sprite[] sprites;

    void Awake(){
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
    }

    public void OnMouseDown(){
        selectedIcon.SetActive(!selectedIcon.activeSelf);
        actionManager.handlePresentedCardClick(selectedIcon.activeSelf, card);
        GameObject.Find("EventCardHandler").GetComponent<EventCardHandler>().infectionCardClicked(selectedIcon.activeSelf, card);   
    }
    
    public void populateItemData(InfectionCard card){
        this.card = card;
        label.text = card.Name;
    }

    public InfectionCard getSelectedValue(){
        return card;
    }

    public Sprite getSprite(){
        return sprites[(int)card.Colour];
    }

    public bool isSelected(){
        return selectedIcon.activeSelf;
    }
}