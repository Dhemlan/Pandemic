using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractablesManager : MonoBehaviour
{
    public Button events;
    public Button playerDiscard;
    public Button infectionDiscard;
    public Board board;

    private BoxCollider2D[] boxColliders;
    private CircleCollider2D[] circleColliders;

    void Start(){
        boxColliders = board.GetComponentsInChildren<BoxCollider2D>();
        circleColliders = board.GetComponentsInChildren<CircleCollider2D>();
    }

    public void toggleAllInteractables(bool value){
        togglePermanentButtons(value);
        toggleColliders(value);
    }

    private void togglePermanentButtons(bool value){
        events.enabled = value;
        playerDiscard.enabled = value;
        infectionDiscard.enabled = value;
    }

    private void toggleColliders(bool value){
        toggleBoxColliders(value);
        toggleCircleColliders(value);
    }

    private void toggleBoxColliders(bool value){
        foreach(BoxCollider2D collider in boxColliders){
            collider.enabled = value;
        }
    }

    private void toggleCircleColliders(bool value){
        foreach(CircleCollider2D collider in circleColliders){
            collider.enabled = value;
        }
    }

    public BoxCollider2D[] BoxColliders { get => boxColliders; set => boxColliders = value; }
    public CircleCollider2D[] CircleColliders { get => circleColliders; set => circleColliders = value; }
}
