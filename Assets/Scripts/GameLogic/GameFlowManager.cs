﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gameOverFlag = false;

    public Board board;

    void Start()
    {
        
        StartCoroutine(gameFlow());
    }


    private IEnumerator gameFlow(){
        //yield return StartCoroutine(board.infectCities());
        for (int i = 0; i < 10; i++){
        board.movePhase();
        yield return StartCoroutine(board.drawPhase());
        yield return StartCoroutine(board.infectionPhase());
        }
        yield break;
    }

    public void gameOver(string message){
        Debug.Log(message);
       // SceneManager.LoadScene("GameOver");
        gameOverFlag = true;
    }
}
