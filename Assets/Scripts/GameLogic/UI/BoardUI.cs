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
    public Sprite[] uncuredSprites;

    public Button drawButton;
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

    public void addCube(Location loc, Vals.Colour colour){
        GameObject diseaseCube = Instantiate(diseaseCubePrefab, new Vector3(10,10,0), Quaternion.identity, loc.transform);
        diseaseCube.GetComponent<DiseaseCubeRotator>().Colour = colour;
        SpriteRenderer renderer = diseaseCube.GetComponent<SpriteRenderer>();
        renderer.sprite = diseaseCubeSprites[(int)colour];
        //renderer.sortingOrder = 4;
        
        initiateCubeMovement(loc);
    }


    public void initiateCubeMovement(Location loc){
        DiseaseCubeRotator[] cubes = loc.transform.GetComponentsInChildren<DiseaseCubeRotator>();
        int i = 0;
        foreach (DiseaseCubeRotator cube in cubes){
            float theta = (2 * Mathf.PI / cubes.Length) * i;
            float bufferDistance = 1.8f;
            cube.transform.position = new Vector3(loc.transform.position.x - bufferDistance * Mathf.Cos(theta), loc.transform.position.y - bufferDistance * Mathf.Sin(theta), 0);
            i++;
        }
        adjustSpeedForAllCubes(loc); // outside loop (instead of per cube) to maintain appropriate cube speed urgency with mixed cube colours
    }

    public void adjustSpeedForAllCubes(Location loc){
        int[] cubes = loc.DiseaseCubes;
        int count = 0;
        for (int i = 0; i < cubes.Length; i++){
            if (cubes[i] > count){
                count = cubes[i];
            }
        }
        DiseaseCubeRotator[] cubeObjects = loc.transform.GetComponentsInChildren<DiseaseCubeRotator>();
        foreach(DiseaseCubeRotator cube in cubeObjects){
            adjustCubeSpeed(cube, count);
        }
    }

    public void adjustCubeSpeed(DiseaseCubeRotator cube, int count){
        cube.setSpeed(count);
    }


    public void removeCube(Location loc, Vals.Colour colour){
        foreach(DiseaseCubeRotator cube in loc.transform.GetComponentsInChildren<DiseaseCubeRotator>()){
            if (cube.Colour == colour){
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

    public void updateDiseaseStatus(Vals.Colour colour, int status){
        if(status == Vals.DISEASE_CURED){
            cureTokens[(int)colour].GetComponent<SpriteRenderer>().sprite = curedSprites[(int)colour];
        }
        else if (status == Vals.DISEASE_ERADICATED) {
            cureTokens[(int)colour].GetComponent<SpriteRenderer>().sprite = eradicatedSprites[(int)colour];
        }
        else {
            cureTokens[(int)colour].GetComponent<SpriteRenderer>().sprite = uncuredSprites[(int)colour];
        }

    }

    public void toggleResearchStation(Location loc){
        SpriteRenderer station = loc.transform.Find("ResearchStation").GetComponent<SpriteRenderer>();
        station.enabled = loc.ResearchStationStatus;
    }

    public bool confirmInfectionPhase(){
        if (continueInfectionSlider.value == 0) return false;
        return true;
    }

    public void updateDiseaseCubeSupply(int[] supply){
        for (int i = 0; i < cubeReserveText.Length; i++){
            cubeReserveText[i].text = supply[i] + "";
        }
    }

    public void updateCubes(Location loc){
        int[] currentDisplayedCubes = new int[Vals.DISEASE_COUNT];
        foreach (DiseaseCubeRotator cube in loc.transform.GetComponentsInChildren<DiseaseCubeRotator>()){
             currentDisplayedCubes[(int)cube.Colour]++;
        }
        int[] locCubes = loc.DiseaseCubes;
        for (int i = 0; i < currentDisplayedCubes.Length; i++){
            int j = currentDisplayedCubes[i];
            while (j > locCubes[i]){
                removeCube(loc, (Vals.Colour)i);
                j--;
            }
            while (j < locCubes[i]){
                addCube(loc, (Vals.Colour)i);
                j++;
            }
        }
    }
}
