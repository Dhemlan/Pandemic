using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    public CardUI cardUI;
    
    public GameObject exampleLocation;
    public GameObject[] cubePrefabs;

    public Text[] cubeReserveText;
    public Text outbreaksCounterText;
    public Text playerDeckCountText;

    public GameObject infectionRateMarker;
    public GameObject curInfectionRateCircle;

    public void setPlayerDeckCount(int newCount){
        playerDeckCountText.text = newCount + "";
    }

    public void advanceInfectionRateTrack(){
        curInfectionRateCircle = curInfectionRateCircle.GetComponent<InfectionTrackLink>().next();
        infectionRateMarker.transform.SetParent(curInfectionRateCircle.transform);
        infectionRateMarker.transform.localPosition = Vector3.zero;
    }

    public IEnumerator addCube(Location loc, ConstantVals.Colour colour){
        GameObject diseaseCube = Instantiate(cubePrefabs[(int)colour], new Vector3(10,10,0), Quaternion.identity);
        diseaseCube.transform.SetParent(loc.transform);
        float width = exampleLocation.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        
        Transform[] cubes = loc.transform.GetComponentsInChildren<Transform>();
        float space = 2 * width / cubes.Length;
        float pos = space * (cubes.Length - 1) / 2f;
        
        for (int i = 1; i < cubes.Length; i++){
            cubes[i].transform.position = new Vector3(loc.transform.position.x - pos, loc.transform.position.y - width, 0);
            pos -= space;
        }
        yield return new WaitForSeconds(1);
    }

    public void increaseOutbreakCounter(int newCount){
        outbreaksCounterText.text = newCount + "/8";
    }

    public void setCubeCount(ConstantVals.Colour colour, int count){
        cubeReserveText[(int)colour].text = count + "";
    }

}
