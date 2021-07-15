using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutbreakHandler : MonoBehaviour
{
    private Queue<Outbreak> outbreaksToResolve = new Queue<Outbreak>();
    private List<Location> outbreakCitiesThisMove = new List<Location>();

    public Board board;
    public GameFlowManager gameFlowManager;

    public IEnumerator checkForOutbreak(Location loc, Vals.Colour colour, System.Action<bool> outbreakOccursCallback){
        if (loc.getDiseaseCubeCount(colour) == Vals.OUTBREAK_THRESHOLD){
            outbreakOccursCallback(true);
            Debug.Log("Outbreak identified - cube not added");
            if (outbreakCitiesThisMove.Count == 0){
                yield return StartCoroutine(outbreakOccurs(new Outbreak(loc, colour)));
            }
            else {
                outbreaksToResolve.Enqueue(new Outbreak(loc, colour));
                Debug.Log("Chain reaction - delyaing resolution");
            }
        }
        else {
            outbreakOccursCallback(false);
        }
    }

    public IEnumerator outbreakOccurs(Outbreak outbreak){
        Location loc = outbreak.Location;
        if (!outbreakCitiesThisMove.Contains(loc)){
            board.OutbreakCount++;
            Debug.Log("Outbreak #" + board.OutbreakCount + " in " + loc.retrieveLocName());
            //boardUI.increaseOutbreakCounter(outbreakCount);
            if (board.OutbreakCount == Vals.OUTBREAK_COUNT_LIMIT) {
                yield return StartCoroutine(gameFlowManager.gameOver(Vals.GameOver.OUTBREAKS));         
            }
            outbreakCitiesThisMove.Add(loc);
            foreach (Location neighbour in loc.Neighbours){
                yield return StartCoroutine(board.addCube(neighbour, outbreak.Colour));
                yield return new WaitForSeconds(Vals.CUBE_WAIT_TIME);
                Debug.Log("Request to add cube in " + neighbour.retrieveLocName());
            }
        }
        yield break;
    }

    public IEnumerator resolveAllOutbreaks(){
        while (outbreaksToResolve.Count != 0){
            Outbreak outbreak = outbreaksToResolve.Dequeue();
            yield return StartCoroutine(outbreakOccurs(outbreak));
        }
        outbreakCitiesThisMove.Clear();
    }
}
