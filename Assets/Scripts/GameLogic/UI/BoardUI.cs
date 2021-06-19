using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{  
    public GameObject diseaseCubePrefab;
    public Sprite[] diseaseCubeSprites;

    public Text[] cubeReserveText;
    public Text outbreaksCounterText, playerDeckCountText;

    public GameObject infectionRateMarker, curInfectionRateCircle;
    public GameObject[] cureTokens;
    public Sprite[] curedSprites;
    public Sprite[] eradicatedSprites;

    public Button drawButton;
    public GameObject continueButton;
    public Slider continueInfectionSlider;

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
        diseaseCube.GetComponent<DiseaseCubeRotator>().setColour(colour);
        SpriteRenderer renderer = diseaseCube.GetComponent<SpriteRenderer>();
        renderer.sprite = diseaseCubeSprites[(int)colour];
        //renderer.sortingOrder = 4;
        
        initiateCubeMovement(loc);
        yield return new WaitForSeconds(.3f);
    }

    public void initiateCubeMovement(Location loc){
        DiseaseCubeRotator[] cubes = loc.transform.GetComponentsInChildren<DiseaseCubeRotator>();
        int i = 0;
        foreach (DiseaseCubeRotator cube in cubes){
            float theta = (2 * Mathf.PI / cubes.Length) * i;
            float bufferDistance = 1.8f;
            cube.transform.position = new Vector3(loc.transform.position.x - bufferDistance * Mathf.Cos(theta), loc.transform.position.y - bufferDistance * Mathf.Sin(theta), 0);
            i++;
            adjustCubeSpeed(cube, cubes.Length);
        }
    }

    public void adjustSpeedForAllCubes(Location loc){
        DiseaseCubeRotator[] cubes = loc.transform.GetComponentsInChildren<DiseaseCubeRotator>();
        foreach(DiseaseCubeRotator cube in cubes){
            adjustCubeSpeed(cube, cubes.Length);
        }
    }

    public void adjustCubeSpeed(DiseaseCubeRotator cube, int count){
        cube.setSpeed(count);
    }


    public void removeCube(Location loc, Vals.Colour colour){
        foreach(DiseaseCubeRotator cube in loc.transform.GetComponentsInChildren<DiseaseCubeRotator>()){
            Debug.Log(loc.transform.GetComponentsInChildren<DiseaseCubeRotator>().Length);
            if (cube.getColour() == colour){
                cube.transform.SetParent(this.transform); //to ensure next Destroy isn't called on same object before frame updates
                Debug.Log("Destroying UI cube " + colour);
                Destroy(cube.gameObject);
                break;
            }
        }
        adjustSpeedForAllCubes(loc);
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

    public bool confirmInfectionPhase(){
        if (continueInfectionSlider.value == 0) return false;
        return true;
    }
}
