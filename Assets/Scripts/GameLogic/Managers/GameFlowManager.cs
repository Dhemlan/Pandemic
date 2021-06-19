using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
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
            yield return new WaitUntil(() => Vals.continueGameFlow);
            phase = Vals.Phase.INFECTION;
            yield return StartCoroutine(board.infectionPhase());
            
            playerManager.endPlayerTurn();
        }
        yield break;
    }

    public void continueGameFlow(){
        Vals.continueGameFlow = true;
    }

    public Vals.Phase getPhase(){
        return phase;
    }

    public IEnumerator gameOver(Vals.GameOver reason){
        yield return StartCoroutine(gameObject.GetComponent<FadeToBlack>().fadeToBlack());
        switch (reason){
            case Vals.GameOver.CUBES :
                SceneManager.LoadScene("GameOverCubes");
                break;
            case Vals.GameOver.OUTBREAKS :
                SceneManager.LoadScene("GameOverOutbreaks");
                break;
            case Vals.GameOver.CARDS :
                SceneManager.LoadScene("GameOverCards");
                break;
            case Vals.GameOver.WIN :
                SceneManager.LoadScene("GameOverWin");
                break;
        }
    }
}
