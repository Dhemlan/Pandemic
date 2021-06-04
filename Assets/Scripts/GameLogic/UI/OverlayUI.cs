using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OverlayUI : MonoBehaviour{

    private List<GameObject> displayedItems;

    private bool proceedSwitch = false;
    private int remainderToSelect;
    private Nullable<Vals.Colour> typeToSelect;

    private BoxCollider2D[] boxColliders;
    private CircleCollider2D[] circleColliders;
    
    public GameObject centreDisplayArea;
    public GameObject displayedPrefabsParent;
    public GameObject[] selectablePrefabs;
    public Text centreDisplayInstructionsText;
    public Button playerConfirmationButton;
    
    public void displayInteractables<T>(List<T> itemsToDisplay, List<GameObject> displayedObjects, int prefabCategory, string message){
        float prefabWidth = selectablePrefabs[prefabCategory].GetComponent<Renderer>().bounds.size.x;
        int gap = 2;
        int count = itemsToDisplay.Count;
        float position = (count % 2 == 0) ? -((count / 2 - 1) * (prefabWidth + gap) + (prefabWidth + gap)/2) : -(count / 2 * (prefabWidth + gap));
        centreDisplayArea.SetActive(true);
        centreDisplayInstructionsText.text = message;
        foreach (T itemToDisplay in itemsToDisplay){
            GameObject displayedObject = Instantiate(selectablePrefabs[prefabCategory], new Vector3(position,0,0), Quaternion.identity, displayedPrefabsParent.transform);
            ISelectable<T> selectableItem = displayedObject.GetComponent<ISelectable<T>>();
            selectableItem.populateItemData(itemToDisplay);
            displayedObject.GetComponent<SpriteRenderer>().sprite = selectableItem.getSprite();
            displayedObjects.Add(displayedObject);
            position += prefabWidth + gap;
        } 
    }

    public IEnumerator requestSimpleSelectionFromPlayer<T>(List<T> itemsToSelectFrom, int prefabCategory, string message){
        Debug.Log("overlay called");
        toggleBoardInteractions();
        displayedItems = new List<GameObject>();
        displayInteractables(itemsToSelectFrom, displayedItems, prefabCategory, message);
        Vals.proceed = false;
        yield return new WaitUntil(() => Vals.proceed);
        clearInteractables();
        Vals.proceed = false;
    }

    public IEnumerator displayItemsUntilClosed<T>(List<T> itemsToSelectFrom, int prefabCategory, string message){

        playerConfirmationButton.gameObject.SetActive(true);
        yield return StartCoroutine(requestSimpleSelectionFromPlayer(itemsToSelectFrom, prefabCategory, message));
        playerConfirmationButton.gameObject.SetActive(false);
    }

    public void clearInteractables(){
        Transform[] toDestroy = displayedPrefabsParent.GetComponentsInChildren<Transform>();
        // skip parent gameObject
        for (int i = 1; i < toDestroy.Length; i++){
            Destroy(toDestroy[i].gameObject);
        }
        centreDisplayArea.SetActive(false);
        toggleBoardInteractions();
    }

    public IEnumerator requestSelectableFromPlayer<T>(List<T> itemsToSelectFrom, List<T> selectedItems, int numberToSelect, Nullable<Vals.Colour> colourToDiscard){
        toggleBoardInteractions();
        remainderToSelect = numberToSelect;
        typeToSelect = colourToDiscard;
        displayedItems = new List<GameObject>();
        string message = "Discard cards: " + numberToSelect + " required";
        displayInteractables(itemsToSelectFrom, displayedItems, Vals.SELECTABLE_PLAYER_CARD, message);
        Vals.proceed = false;
        while (!Vals.proceed){
            yield return new WaitUntil(() => remainderToSelect == 0);
            playerConfirmationButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => Vals.proceed);
            
            if (remainderToSelect != 0){
                Debug.Log(remainderToSelect);
                Vals.proceed = false;
                continue;
            }
            else {
                selectedItems.Clear();
                getselectedItems(displayedItems, selectedItems);
                Debug.Log(selectedItems.Count);
                if (selectedItems.Count != numberToSelect){
                    Vals.proceed = false;
                    playerConfirmationButton.gameObject.SetActive(false);
                    continue;
                }
            }
        }
        playerConfirmationButton.gameObject.SetActive(false);
        clearInteractables();
        yield break;
    }

    public void getselectedItems<T>(List<GameObject> displayedItems, List<T> selectedItems){
        foreach (GameObject displayedItem in displayedItems){
            InteractableCard script = displayedItem.GetComponent<InteractableCard>();
            if (script.isSelected()){
               selectedItems.Add((T)(object)script.getSelectedValue());
            }
        }
    }
    
    public void adjustDiscardRequiredCount(int amount, PlayerCard card){
        if (typeToSelect == null){
            remainderToSelect -=amount;
        }
        else if (card.getColour() == typeToSelect){
            remainderToSelect -= amount;
        }
    }

    private void toggleBoardInteractions(){
        toggleBoxColliders();
        toggleCircleColliders();
    }

    private void toggleBoxColliders(){
        foreach(BoxCollider2D collider in boxColliders){
            collider.enabled = !collider.enabled;
        }
    }
    private void toggleCircleColliders(){
        foreach(CircleCollider2D collider in circleColliders){
            collider.enabled = !collider.enabled;
        }
    }

    public void gatherColliders(CircleCollider2D[] circleColliders, BoxCollider2D[] boxColliders){
        this.circleColliders = circleColliders;
        this.boxColliders = boxColliders;
    }
}
