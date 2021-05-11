using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{   
    public GameObject map;
    public GameObject cardPrefab;
    public GameObject cardDisplayArea;
    public Text cardDisplayInstructionsText;

    public GameObject playerCard1, playerCard2, infectionCard;
    public GameObject playerDiscard, infectionDiscard;
    public GameObject playerDeck, infectionDeck;
    public Text playerCard1Title, playerCard2Title, infectionCardTitle;
    public Button playerConfirmationButton;

    public Sprite playerCardFace, playerCardBack, infectionCardBack;
    public Sprite[] infectionCardFaces;
    public Sprite[] playerCardFaces;

    public GameObject hand1, hand2, hand3, hand4;

    private Vector3 mapCentre, mapCentreLeft, mapCentreRight;
    private Vector3 playerDiscardCentre, infectionDiscardCentre;
    private Vector3[] handCentres = new Vector3[4];
    private bool proceedSwitch = false;

    public void Start(){
        mapCentre = new Vector3(map.transform.position.x, map.transform.position.y,0);
        mapCentreLeft = new Vector3(-6, map.transform.position.y, 0);
        mapCentreRight = new Vector3(6, map.transform.position.y, 0);
        infectionDiscardCentre = new Vector3(infectionDiscard.transform.position.x, infectionDiscard.transform.position.y, 0);
        playerDiscardCentre = new Vector3(playerDiscard.transform.position.x, playerDiscard.transform.position.y,0);
        handCentres[0] = new Vector3(hand1.transform.position.x, hand1.transform.position.y, 0);
        handCentres[1] = new Vector3(hand2.transform.position.x, hand2.transform.position.y, 0);
        handCentres[2] = new Vector3(hand3.transform.position.x, hand3.transform.position.y, 0);
        handCentres[3] = new Vector3(hand4.transform.position.x, hand4.transform.position.y, 0);

    }   

    public IEnumerator playerDraw(Player curPlayer, PlayerCard card1, PlayerCard card2){
        playerCard1.SetActive(true);
        playerCard2.SetActive(true);
        
        yield return StartCoroutine(moveAndFlipCard(playerCard1.transform, mapCentreLeft, playerCardFaces[(int)card1.getColour()], new Vector3(1.5f,1.5f,1.5f)));
        playerCard1Title.text = card1.getName();
        yield return StartCoroutine(moveAndFlipCard(playerCard2.transform, mapCentreRight, playerCardFaces[(int)card2.getColour()], new Vector3(1.5f,1.5f,1.5f)));
        playerCard2Title.text = card2.getName();
        yield return new WaitForSeconds(ConstantVals.GENERIC_WAIT_TIME);
        if (card1.getName().Equals("Epidemic")){
            yield return StartCoroutine(moveAndShrinkCard(playerCard1.transform, playerDiscardCentre));
        }
        else {
            yield return StartCoroutine(moveAndShrinkCard(playerCard1.transform, handCentres[curPlayer.getTurnOrderPos() - 1]));
        }
        if (card2.getName().Equals("Epidemic")){
            yield return StartCoroutine(moveAndShrinkCard(playerCard2.transform, playerDiscardCentre));
        }
        else {
            yield return StartCoroutine(moveAndShrinkCard(playerCard2.transform, handCentres[curPlayer.getTurnOrderPos() - 1]));
        }
    
        resetPosition(playerCard1, playerDeck.transform, playerCardBack, playerCard1Title);
        resetPosition(playerCard2, playerDeck.transform, playerCardBack, playerCard2Title);
        yield break;
    } 

    public IEnumerator infectionDraw(string name, ConstantVals.Colour colour){
        infectionCard.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        
        yield return StartCoroutine(moveAndFlipCard(infectionCard.transform, mapCentre, infectionCardFaces[(int)colour], new Vector3(1,1,1)));
        infectionCardTitle.text = name;
        yield return new WaitForSeconds(ConstantVals.GENERIC_WAIT_TIME);
        yield return StartCoroutine(moveAndShrinkCard(infectionCard.transform, infectionDiscardCentre));
        resetPosition(infectionCard, infectionDeck.transform, infectionCardBack, infectionCardTitle);
        yield break;
    }

    public IEnumerator moveAndFlipCard(Transform transform, Vector3 newPosition, Sprite cardFace, Vector3 scale){
        var currentPos = transform.position;
        var t = 0f;
        while(t < 1)
        {
                t += Time.deltaTime / ConstantVals.GENERIC_WAIT_TIME;
                transform.position = Vector3.Lerp(currentPos, newPosition, t);      
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0,180,0)), t);
                transform.localScale =  Vector3.Lerp(transform.localScale, scale, t);
                SpriteRenderer spriteRen= transform.GetComponent<SpriteRenderer>();
                spriteRen.sortingLayerName = "Default";
                spriteRen.sprite = cardFace;  
                yield return null;
        }
    }
    
    public IEnumerator moveAndShrinkCard(Transform transform, Vector3 newPosition){ 
        var currentPos = transform.position;
        var currentScale = transform.localScale;
        var t = 0f;
        while(t < 1)
        {
                t += Time.deltaTime / ConstantVals.GENERIC_WAIT_TIME;
                transform.position = Vector3.Lerp(currentPos, newPosition, t);
                transform.localScale =  Vector3.Lerp(currentScale, new Vector3(0,0,0), t);
                yield return null;
        }
    }

    public void resetPosition(GameObject card, Transform target, Sprite cardBack, Text title){
        card.SetActive(false);
        SpriteRenderer spriteRen= card.GetComponent<SpriteRenderer>();
        spriteRen.sortingLayerName = "TextLayer";
        spriteRen.sprite = cardBack;
        card.transform.position = target.position;
        card.transform.rotation = target.rotation;
        card.transform.localScale = new Vector3(1,1,1);
        title.text= "";
    }

    public IEnumerator allowSelectionToDiscard(List<PlayerCard> cards, List<PlayerCard> selectedCards, int numberToDiscard){
        List<GameObject> displayedCards = new List<GameObject>();
        displayInteractableCards(cards, displayedCards);
        cardDisplayInstructionsText.text = "Select " + numberToDiscard + " cards to discard";
        while (selectedCards.Count != numberToDiscard){
            selectedCards.Clear();
            yield return new WaitForSeconds(.2f);
            foreach (GameObject displayedCard in displayedCards){
                InteractableCard script = displayedCard.GetComponent<InteractableCard>();
                if (script.isSelected()){
                    selectedCards.Add(script.getCard());
                }
            }
        }
        playerConfirmationButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => proceedSwitch);
        playerConfirmationButton.gameObject.SetActive(false);
        clearCards(displayedCards);
        proceedSwitch = false;
    }

    public void clearCards(List<GameObject> cards){
        foreach (GameObject card in cards){
            Destroy(card);
        }
        cardDisplayArea.SetActive(false);
    }

    public void displayInteractableCards(List<PlayerCard> cards, List<GameObject> displayedCards){
        
        int i = -25;
        cardDisplayArea.SetActive(true);
        foreach (PlayerCard card in cards){
            GameObject displayedCard = Instantiate(cardPrefab, new Vector3(i,0,0), Quaternion.identity, cardDisplayArea.transform);
            displayedCard.GetComponent<InteractableCard>().populateCardInfo(card, card.getName());
            displayedCard.GetComponent<SpriteRenderer>().sprite = playerCardFaces[(int)card.getColour()];
            displayedCards.Add(displayedCard);
            i+=10;
        }
    }

    public void proceed(){
        proceedSwitch = true;
    }
}
