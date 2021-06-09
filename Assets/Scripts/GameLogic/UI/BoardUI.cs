using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    public CardUI cardUI;
    
    public GameObject exampleLocation;
    public GameObject diseaseCubePrefab;
    public Sprite[] diseaseCubeSprites;

    public Text[] cubeReserveText;
    public Text outbreaksCounterText, playerDeckCountText;

    public GameObject infectionRateMarker, curInfectionRateCircle;
    public GameObject[] cureTokens;
    public Sprite[] curedSprites;
    public Sprite[] eradicatedSprites;

    public Button drawButton;

    public void boardSetUp(int playerDeckCount){
        setPlayerDeckCount(playerDeckCount);
    }

    public void setPlayerDeckCount(int newCount){
        playerDeckCountText.text = newCount + "";
    }

    public void advanceInfectionRateTrack(){
        curInfectionRateCircle = curInfectionRateCircle.GetComponent<InfectionTrackLink>().next();
        infectionRateMarker.transform.SetParent(curInfectionRateCircle.transform);
        infectionRateMarker.transform.localPosition = Vector3.zero;
    }

    public IEnumerator addCube(Location loc, Vals.Colour colour){
        GameObject diseaseCube = Instantiate(diseaseCubePrefab, new Vector3(10,10,0), Quaternion.identity, loc.transform);
        SpriteRenderer renderer = diseaseCube.GetComponent<SpriteRenderer>();
        renderer.sprite = diseaseCubeSprites[(int)colour];
        renderer.sortingOrder = 4;
        
        DiseaseCubeRotator[] cubes = loc.transform.GetComponentsInChildren<DiseaseCubeRotator>();
        int i = 0;
        foreach (DiseaseCubeRotator cube in cubes){
            float theta = (2 * Mathf.PI / cubes.Length) * i;
            float bufferDistance = 1.9f;
            cube.transform.position = new Vector3(loc.transform.position.x - bufferDistance * Mathf.Cos(theta), loc.transform.position.y - bufferDistance * Mathf.Sin(theta), 0);
            i++;
            cube.setSpeed(cubes.Length);
        }
        yield return new WaitForSeconds(.3f);
    }

    public void removeCube(Location loc, Vals.Colour colour){
        Destroy(loc.transform.GetChild(1).gameObject);
    }

    public void increaseOutbreakCounter(int newCount){
        outbreaksCounterText.text = newCount + "/8";
    }

    public void setCubeCount(Vals.Colour colour, int count){
        cubeReserveText[(int)colour].text = count + "";
    }

    public void activateDrawButton(){
        drawButton.gameObject.SetActive(true);
    }

    public void diseaseCured(Vals.Colour colour){
        cureTokens[(int)colour].GetComponent<SpriteRenderer>().sprite = curedSprites[(int)colour];
    }

    public void diseaseEradicated(Vals.Colour colour){
        cureTokens[(int)colour].GetComponent<SpriteRenderer>().sprite = eradicatedSprites[(int)colour];
    }

    public void toggleResearchStation(Location loc){
        SpriteRenderer station = loc.transform.Find("ResearchStation").GetComponent<SpriteRenderer>();
        station.enabled = !station.enabled;
    }

}
