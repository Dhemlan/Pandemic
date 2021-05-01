using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int turnOrderPos;
    private Role role;
    private Location curLoc;
    public List<PlayerCard> hand = new List<PlayerCard>();
    public GameObject pawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addCardToHand(PlayerCard card){
        hand.Add(card);
    }

    public void hi(){
        Debug.Log("hi");
    }

    public List<PlayerCard> getHand(){
        return hand;
    }
}
