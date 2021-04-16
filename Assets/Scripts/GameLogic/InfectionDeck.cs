using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionDeck : MonoBehaviour
{
    private Stack<InfectionCard> infectionDeck = new Stack<InfectionCard>();

    public void  Start()
    {
        generateDeck();
    }

    public InfectionCard drawInfectionCard(){
        if (infectionDeck.Count == 0){
            generateDeck();
            Debug.Log("Empty infection");
        }
        return infectionDeck.Pop();
    }

    public Stack<InfectionCard> getInfectionDeck(){
        return infectionDeck;
    }

    private void generateDeck(){
        
        int[] nums = {1};
        Deck deck = GetComponent<Deck>();
        deck.shuffle(nums);
    }
}
