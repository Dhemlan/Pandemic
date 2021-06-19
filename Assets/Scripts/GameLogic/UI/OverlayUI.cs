using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OverlayUI : MonoBehaviour{

    private List<GameObject> displayedItems = new List<GameObject>();

    private bool proceedSwitch = false;
    private bool displayOverlayFinished = false;
    private int remainderToSelect;
    private Nullable<Vals.Colour> typeToSelect;

    private BoxCollider2D[] boxColliders;
    private CircleCollider2D[] circleColliders;
    
    public GameObject displayOverlayPrefabsParent;
    public GameObject selectionOverlayPrefabsParent;
    public GameObject[] displayablePrefabs;
    public GameObject displayOverlay;
    public GameObject selectionOverlay;
    public Text displayOverlayText;
    public Text selectionOverlayText;

    public GameObject continueButton;
    public Button confirmGameProgressionButton;
    public Button closeDisplayOverlayButton;
    public Image toastPanel;
    public Text toastMessage;

    public void displayInteractables<T>(GameObject itemLocation, List<T> itemsToDisplay, List<GameObject> displayedObjects, int prefabCategory){
        int gap = 2;
        int itemsCount = itemsToDisplay.Count;
        float prefabWidth = displayablePrefabs[prefabCategory].GetComponent<Renderer>().bounds.size.x;
        float position = (itemsCount % 2 == 0) ? -((itemsCount / 2 - 1) * (prefabWidth + gap) + (prefabWidth + gap)/2) : -(itemsCount / 2 * (prefabWidth + gap));

        foreach (T itemToDisplay in itemsToDisplay){
            GameObject displayedObject = Instantiate(displayablePrefabs[prefabCategory], new Vector3(position,0,0), Quaternion.identity, itemLocation.transform);
            ISelectable<T> selectableItem = displayedObject.GetComponent<ISelectable<T>>();
            selectableItem.populateItemData(itemToDisplay);
            displayedObject.GetComponent<SpriteRenderer>().sprite = selectableItem.getSprite();
            displayedObjects.Add(displayedObject);
            position += prefabWidth + gap;
        } 
    }

    public List<GameObject> displaySelectableItems<T>(List<T> itemsToDisplay, GameObject prefab, GameObject overlayLocation){
        List<GameObject> toReturn = new List<GameObject>();
        int scaleFactor = 15;
        float width = (prefab.GetComponent<Renderer>().bounds.size.x)/scaleFactor;
        float position = (itemsToDisplay.Count % 2 == 0) ? -width/2 : 0;
        Debug.Log(position);
        foreach (T itemToDisplay in itemsToDisplay){
            GameObject displayedObject = Instantiate(prefab, new Vector3(position,overlayLocation.transform.position.y,0), Quaternion.identity, overlayLocation.transform);
            ISelectable<T> selectable = displayedObject.GetComponent<ISelectable<T>>();
            selectable.populateItemData(itemToDisplay);
            displayedObject.GetComponent<SpriteRenderer>().sprite = selectable.getSprite();
            toReturn.Add(displayedObject);
            if (position < 0) position *= -1;
            else position = -(position + width);
        }
        overlayLocation.GetComponent<RectTransform>().sizeDelta = new Vector2(position * scaleFactor * 3, 100);
        return toReturn;
    }

    public void displaySimpleItems(){

    }

    public void activateOverlay(GameObject overlay){
        overlay.SetActive(true);
        clearInteractables(overlay.transform.Find("DisplayedItems"));
        setBoardInteractions(false);
        Vals.continueAfterMultiSelect = false;
        Vals.continueAfterSelect = false;
    }

    public void deactivateOverlay(GameObject overlay){
        overlay.SetActive(false);
        setBoardInteractions(true);
        confirmGameProgressionButton.gameObject.SetActive(false);
        Vals.continueAfterSelect = false;
    }

    public IEnumerator requestSimpleSelectionFromPlayer<T>(List<T> itemsToSelectFrom, int prefabCategory, string message){
        activateOverlay(selectionOverlay);      
        //displayedItems = new List<GameObject>();
        selectionOverlayText.text = message;
        displaySelectableItems(itemsToSelectFrom, displayablePrefabs[prefabCategory], selectionOverlayPrefabsParent);
        yield return new WaitUntil(() => Vals.continueAfterSelect);
        deactivateOverlay(selectionOverlay);
    }

    // Used for elements that may be viewed at any time (e.g. discard, events)
    public IEnumerator displayItemsUntilClosed<T>(List<T> itemsToSelectFrom, int prefabCategory, string message){
        bool selectionOverlayActive = selectionOverlay.activeSelf;
        selectionOverlay.SetActive(false);

        closeDisplayOverlayButton.gameObject.SetActive(true);
        activateOverlay(displayOverlay);

        displayedItems = new List<GameObject>();
        displayOverlayText.text = message;
        displaySelectableItems(itemsToSelectFrom, displayablePrefabs[prefabCategory], displayOverlayPrefabsParent);
        yield return new WaitUntil(() => displayOverlayFinished);
        
        displayOverlayFinished = false;
        closeDisplayOverlayButton.gameObject.SetActive(false);
        if(selectionOverlayActive){
            displayOverlay.SetActive(false);
            selectionOverlay.SetActive(selectionOverlayActive);
        }
        else {
            deactivateOverlay(displayOverlay);
        }    
    }

    public void clearInteractables(Transform itemsLocation){
        Transform[] toDestroy = itemsLocation.GetComponentsInChildren<Transform>();
        // skip parent gameObject
        for (int i = 1; i < toDestroy.Length; i++){
            Destroy(toDestroy[i].gameObject);
        }
    }

    public IEnumerator requestMultiSelect<T>(List<T> itemsToSelectFrom, List<T> selectedItems, int prefabCategory, int numberToSelect, Nullable<Vals.Colour> colourToDiscard, string message){  
        remainderToSelect = numberToSelect;
        typeToSelect = colourToDiscard;
        activateOverlay(selectionOverlay);
        selectionOverlayText.text = message;
        displayedItems = displaySelectableItems(itemsToSelectFrom, displayablePrefabs[prefabCategory], selectionOverlayPrefabsParent);
        while (!Vals.continueAfterMultiSelect){
            yield return new WaitUntil(() => remainderToSelect == 0);
            confirmGameProgressionButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => Vals.continueAfterMultiSelect);
            if (remainderToSelect != 0){
                Vals.continueAfterMultiSelect = false;
                confirmGameProgressionButton.gameObject.SetActive(false);
                continue;
            }
            else {
                selectedItems.Clear();
                getselectedItems(displayedItems, selectedItems);
                if (selectedItems.Count != numberToSelect){
                    Vals.continueAfterMultiSelect = false;
                    confirmGameProgressionButton.gameObject.SetActive(false);
                    continue;
                }
            }
        }
        deactivateOverlay(selectionOverlay);
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

    public IEnumerator displayToast(string message, bool timeout){
        toastMessage.text = message;
        toastPanel.gameObject.SetActive(true);
        if (!timeout) yield break;
        yield return new WaitForSeconds(3);
        toastPanel.gameObject.SetActive(false);
    }

    public void closeToast(){
        toastPanel.gameObject.SetActive(false);
        continueButton.SetActive(false);
    }

    public void requestPlayerContinue(string message){
        StartCoroutine(displayToast(message, false));
        continueButton.SetActive(true);
    }

    public void continueAfterSelect(){
        Vals.continueAfterSelect = true;
    }

    public void finishedWithDisplayOverlay(){
        displayOverlayFinished = true;
    }

    public void proceed(){
        Vals.proceed = true;
    }

    public void continueAfterMultiSelect(){
        Vals.continueAfterMultiSelect = true;
        Debug.Log("continue gameflow");
    }
}
