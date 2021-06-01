using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableEventCards : MonoBehaviour
{
    public Board board;
    
    public void OnMouseDown(){
        StartCoroutine(board.displayAvailableEvents());
    }
}
