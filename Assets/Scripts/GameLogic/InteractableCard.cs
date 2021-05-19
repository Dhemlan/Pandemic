using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableCard : MonoBehaviour
{
    private PlayerCard card;
    private SpriteRenderer spriteRen;
    public GameObject selectedIcon;
    public Text label;
    public ActionManager actionManager;

    void Awake(){
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
    }

    void OnMouseDown(){
        selectedIcon.SetActive(!selectedIcon.activeSelf);
        actionManager.handlePresentedPlayerCardClick(selectedIcon.activeSelf, card);
    }
    
    public void populateCardInfo(PlayerCard card, string name){
        this.card = card;
        label.text = name;
    }

    public PlayerCard getCard(){
        return card;
    }

    public bool isSelected(){
        return selectedIcon.activeSelf;
    }
}
