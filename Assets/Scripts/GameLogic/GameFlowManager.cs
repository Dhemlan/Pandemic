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
        Debug.Log("starting");
        for (int i = 0; i < 5; i++){
        board.movePhase();
        board.drawPhase();
        yield return StartCoroutine(board.infectionPhase());
        Debug.Log("successful");
        }
        yield break;
    }

    public void gameOver(string message){
        Debug.Log(message);
       // SceneManager.LoadScene("GameOver");
        gameOverFlag = true;
    }
}
