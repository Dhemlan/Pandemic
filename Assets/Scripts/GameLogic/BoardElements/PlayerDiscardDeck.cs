using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiscardDeck : MonoBehaviour
{   
    public Board board;

    public void OnMouseDown(){
        StartCoroutine(board.displayPlayerDiscard());
    }
}
