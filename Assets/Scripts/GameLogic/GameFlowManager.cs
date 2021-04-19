﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Board board;

    void Start()
    {
        gameFlow();
    }

    private void gameFlow(){
        for (int i = 0; i < 5; i++){
            board.movePhase();
            board.drawPhase();
            board.infectionPhase();
        }
    }
}