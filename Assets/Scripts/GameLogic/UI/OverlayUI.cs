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

    public void activateInteractableOverlay(){
        clearInteractables();
        centreDisplayArea.SetActive(true);
        setBoardInteractions(false);
        Vals.proceed = false;
    }

    public void deactivateInteractableOverlay(){
        centreDisplayArea.SetActive(false);
        setBoardInteractions(true);
        Vals.proceed = false;
        playerConfirmationButton.gameObject.SetActive(false);
    }

    public IEnumerator requestSimpleSelectionFromPlayer<T>(List<T> itemsToSelectFrom, int prefabCategory, string message){
        activateInteractableOverlay();      
        displayedItems = new List<GameObject>();
        displayInteractables(itemsToSelectFrom, displayedItems, prefabCategory, message);
        yield return new WaitUntil(() => Vals.proceed);
        deactivateInteractableOverlay();
    }

    public IEnumerator displayItemsUntilClosed<T>(List<T> itemsToSelectFrom, int prefabCategory, string message){
        playerConfirmationButton.gameObject.SetActive(true);
        yield return StartCoroutine(requestSimpleSelectionFromPlayer(itemsToSelectFrom, prefabCategory, message));
    }

    public void clearInteractables(){
        Transform[] toDestroy = displayedPrefabsParent.GetComponentsInChildren<Transform>();
        // skip parent gameObject
        for (int i = 1; i < toDestroy.Length; i++){
            Destroy(toDestroy[i].gameObject);
        }
    }

    public IEnumerator requestSelectableFromPlayer<T>(List<T> itemsToSelectFrom, List<T> selectedItems, int prefabCategory, int numberToSelect, Nullable<Vals.Colour> colourToDiscard){  
        remainderToSelect = numberToSelect;
        typeToSelect = colourToDiscard;
        displayedItems = new List<GameObject>();
        string message = "Discard cards: " + numberToSelect + " required";
        activateInteractableOverlay();
        displayInteractables(itemsToSelectFrom, displayedItems, prefabCategory, message);
        Debug.Log("starting loop " + Vals.proceed);
        while (!Vals.proceed){
            Debug.Log("remainder to select" + remainderToSelect);
            yield return new WaitUntil(() => remainderToSelect == 0);
            playerConfirmationButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => Vals.proceed);
            Debug.Log("remainder" + remainderToSelect);
            if (remainderToSelect != 0){
                Vals.proceed = false;
                playerConfirmationButton.gameObject.SetActive(false);
                Debug.Log("remainder not correct, relooping");
                continue;
            }
            else {
                selectedItems.Clear();
                getselectedItems(displayedItems, selectedItems);
                Debug.Log("Selected: " + selectedItems.Count);
                if (selectedItems.Count != numberToSelect){
                    Vals.proceed = false;
                    playerConfirmationButton.gameObject.SetActive(false);
                    Debug.Log("cards rejected");
                    continue;
                }
                Debug.Log("cards accepted");
            }
        }
        Debug.Log("deactivating overlay");
        deactivateInteractableOverlay();
        yield break;
    }

    public void getselectedItems<T>(List<GameObject> displayedItems, List<T> selectedItems){
        foreach (GameObject displayedItem in displayedItems){
            ISelectable<T> script = displayedItem.GetComponent<ISelectable<T>>();
            if (script.isSelected()){
               selectedItems.Add((T)(object)script.getSelectedValue());
            }
        }
    }

    public void adjustDiscardRequiredCount(int amount, Card card){
        if (typeToSelect == null){
            remainderToSelect -=amount;
        }
        else if (card.getColour() == typeToSelect){
            remainderToSelect -= amount;
        }
    }

    private void setBoardInteractions(bool value){
        setBoxColliders(value);
        setCircleColliders(value);
    }

    private void setBoxColliders(bool value){
        foreach(BoxCollider2D collider in boxColliders){
            collider.enabled = value;
        }
    }
    private void setCircleColliders(bool value){
        foreach(CircleCollider2D collider in circleColliders){
            collider.enabled = value;
        }
    }

    public void gatherColliders(CircleCollider2D[] circleColliders, BoxCollider2D[] boxColliders){
        this.circleColliders = circleColliders;
        this.boxColliders = boxColliders;
    }

    public void proceed(){
        Vals.proceed = true;
        Debug.Log("calling proceed");
    }
}
