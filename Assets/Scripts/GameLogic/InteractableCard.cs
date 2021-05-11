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

    void Start(){
    }

    void OnMouseDown(){
        selectedIcon.SetActive(!selectedIcon.activeSelf);
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
