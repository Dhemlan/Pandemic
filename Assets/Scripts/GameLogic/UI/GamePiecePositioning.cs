using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiecePositioning : MonoBehaviour
{
    Renderer renderer;

    void Awake ()
    {
        renderer = gameObject.GetComponent<Renderer>();
    }

    void Start ()
    {
        renderer.sortingLayerID = SortingLayer.NameToID("GamePieces");
    } 

}
