using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject diseaseCubePrefab;
    public Sprite[] diseaseCubeSprites;
    public RectTransform canvas;

    void Awake(){
        StartCoroutine(cubeSpawning());
    }

    private IEnumerator cubeSpawning(){
        List<GameObject> cubes = new List<GameObject>();
        for (int i = 0; i < Vals.INITIAL_DISEASE_CUBE_COUNT / 2; i++){
            cubes.Add(spawnCube(this.gameObject));
            foreach (GameObject cube in cubes){
                spawnCube(cube);
            }
            yield return new WaitForSeconds(1);
        }
        
    }

    public GameObject spawnCube(GameObject parent){
        GameObject diseaseCube = Instantiate(diseaseCubePrefab, new Vector3(Random.Range(-Screen.width, Screen.width), Random.Range(-Screen.height, Screen.height), 0), Quaternion.identity, parent.transform);
        diseaseCube.GetComponent<SpriteRenderer>().sprite = diseaseCubeSprites[0];
        return diseaseCube;
    }
}
