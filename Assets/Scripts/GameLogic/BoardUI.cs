using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    public GameObject map;
    public GameObject infectionCard;
    public GameObject playerCard1;
    public GameObject playerCard2;
    public GameObject cubePrefab;

    public GameObject playerDiscard;
    public GameObject playerDeck;
    public GameObject infectionDeck;
    public GameObject infectionDiscard;

    public GameObject infectionMarker;

    public Sprite playerCardFace;
    public Sprite playerCardBack;
    public Sprite infectionCardface;
    public Sprite infectionCardBack;

    private Vector3 mapCentre;
    private Vector3 mapCentreLeft;
    private Vector3 mapCentreRight;
    private Vector3 infectionDiscardCentre;
    private Vector3 playerDiscardCentre;

    public void Start(){
        mapCentre = new Vector3(map.transform.position.x, map.transform.position.y,0);
        mapCentreLeft = new Vector3(-6, map.transform.position.y, 0);
        mapCentreRight = new Vector3(6, map.transform.position.y, 0);
        infectionDiscardCentre = new Vector3(infectionDiscard.transform.position.x, infectionDiscard.transform.position.y, 0);
        playerDiscardCentre = new Vector3(playerDiscard.transform.position.x, playerDiscard.transform.position.y,0);
    }    
    
    public IEnumerator playerDraw(){
        playerCard1.GetComponent<SpriteRenderer>().enabled = true;
        playerCard2.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(moveCardToCentre(playerCard1.transform, mapCentreLeft, playerCardFace, new Vector3(1.5f,1.5f,1.5f)));
        yield return StartCoroutine(moveCardToCentre(playerCard2.transform, mapCentreRight, playerCardFace, new Vector3(1.5f,1.5f,1.5f)));
        
        yield return new WaitForSeconds(ConstantVals.GENERIC_WAIT_TIME);
        yield return StartCoroutine(moveToDiscard(playerCard1.transform, playerDiscardCentre));
        yield return StartCoroutine(moveToDiscard(playerCard2.transform, playerDiscardCentre));
        
        resetPosition(playerCard1, playerDeck.transform, playerCardBack);
        resetPosition(playerCard2, playerDeck.transform, playerCardBack);
        yield break;
    } 

    public IEnumerator infectionDraw(){
        infectionCard.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.15f);
        //infectionCard.transform.SetParent(map.transform);
        yield return StartCoroutine(moveCardToCentre(infectionCard.transform, mapCentre, infectionCardface, new Vector3(1,1,1)));
        
        yield return new WaitForSeconds(ConstantVals.GENERIC_WAIT_TIME);
        yield return StartCoroutine(moveToDiscard(infectionCard.transform, infectionDiscardCentre));
        
        resetPosition(infectionCard, infectionDeck.transform, infectionCardBack);
        yield break;
    }

    public IEnumerator addCube(Location loc, ConstantVals.Colour colour){
        Debug.Log("adding cube ui");
        //cube.GetComponent<SpriteRenderer>().enabled = true;
        GameObject diseaseCube = Instantiate(cubePrefab, new Vector3(10,10,0), Quaternion.identity);
        diseaseCube.transform.SetParent(loc.transform);
        diseaseCube.transform.position = new Vector3(loc.transform.position.x, loc.transform.position.y - 2.0f, 0);
        
        float pos = -1;
        foreach (Transform child in loc.transform){
            child.transform.position = new Vector3(pos, child.transform.position.y, 0);
            pos *= -1;
        }
        yield return new WaitForSeconds(1);
    }

    public IEnumerator moveCardToCentre(Transform transform, Vector3 position, Sprite cardFace, Vector3 scale){
        var currentPos = transform.position;
        var t = 0f;
        while(t < 1)
        {
                t += Time.deltaTime / ConstantVals.GENERIC_WAIT_TIME;
                transform.position = Vector3.Lerp(currentPos, position, t);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0,180,0)), t);
                transform.localScale =  Vector3.Lerp(transform.localScale, scale, t);
                infectionCard.GetComponent<SpriteRenderer>().sprite = cardFace;  
                yield return null;
        }
    }
    public IEnumerator moveToDiscard(Transform transform, Vector3 position){ 
        var currentPos = transform.position;
        var currentScale = transform.localScale;
        var t = 0f;
        while(t < 1)
        {
                t += Time.deltaTime / ConstantVals.GENERIC_WAIT_TIME;
                transform.position = Vector3.Lerp(currentPos, position, t);
                transform.localScale =  Vector3.Lerp(currentScale, new Vector3(0,0,0), t);
                yield return null;
        }
    }

    public void resetPosition(GameObject card, Transform target, Sprite cardBack){
        card.GetComponent<SpriteRenderer>().enabled = false;
        card.GetComponent<SpriteRenderer>().sprite = cardBack;
        card.transform.position = target.position;
        card.transform.rotation = target.rotation;
        card.transform.localScale = new Vector3(1,1,1);
    }
}
