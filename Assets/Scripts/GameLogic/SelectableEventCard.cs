using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableEventCard : MonoBehaviour, ISelectable<PlayerCard>
{
    private PlayerCard card;
    public Text label;
    private EventCardHandler eventCardHandler;
    public Sprite sprite;

    void Awake(){
        eventCardHandler = GameObject.Find("EventCardHandler").GetComponent<EventCardHandler>();
    }

    public void OnMouseDown(){
        Vals.proceed = true;
        eventCardHandler.eventClicked(card);
    }
    
    public void populateItemData(PlayerCard card){
        this.card = card;
        label.text = card.Name;
    }

    public PlayerCard getSelectedValue(){
        return card;
    }

    public Sprite getSprite(){
        return sprite;
    }

    public bool isSelected(){
       return false;
   }
}
