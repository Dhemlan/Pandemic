using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelectablePlayerCard : MonoBehaviour, ISelectable<PlayerCard>
{
    private PlayerCard card;
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
    }
    
    public void populateItemData(PlayerCard card){
        this.card = card;
        label.text = card.Name;
    }

    public PlayerCard getSelectedValue(){
        return card;
    }

    public Sprite getSprite(){
        return sprites[(int)card.Colour];
    }

    public bool isSelected(){
        return selectedIcon.activeSelf;
    }
}
