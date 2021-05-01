using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gameOverFlag = false;

    public Board board;
    public PlayerManager playerManager;

    private int characterCount = 2;

    void Start()
    {       
        StartCoroutine(gameFlow());
    }

    private IEnumerator gameFlow(){
        playerManager.generateCharacters(3);
        board.boardSetUp();
        //yield return StartCoroutine(board.infectCities());
        while (true){
            playerManager.movePhase();
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
