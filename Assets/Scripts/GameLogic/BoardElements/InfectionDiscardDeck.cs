using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionDiscardDeck : MonoBehaviour
{
     public Board board;

    public void OnMouseDown(){
        StartCoroutine(board.displayInfectionDiscard());
    }
}
