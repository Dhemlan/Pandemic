using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gameOverFlag = false;

    public Board board;
    public PlayerManager playerManager;

    private int characterCount = 2;
    private int epidemicCardCount = 6;
    private Vals.Phase phase;

    void Start()
    {       
        StartCoroutine(gameFlow());
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator gameFlow(){
        
        phase = Vals.Phase.SET_UP;
        yield return StartCoroutine(board.boardSetUp(epidemicCardCount));  
        while (true){
            phase = Vals.Phase.ACTION;
            yield return StartCoroutine(playerManager.movePhase());
            phase = Vals.Phase.DRAW;
            yield return StartCoroutine(board.drawPhase());
            if (!Vals.oneQuietNightActive){
                phase = Vals.Phase.INFECTION;
                yield return StartCoroutine(board.infectionPhase());
            }
            else {
                Vals.oneQuietNightActive = false;
            }
            playerManager.endPlayerTurn();
        }
        yield break;
    }

    public Vals.Phase getPhase(){
        return phase;
    }


    public void gameOver(string message){
        Debug.Log(message);
        SceneManager.LoadScene("GameOver");
    }
}
