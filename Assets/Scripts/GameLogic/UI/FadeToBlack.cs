using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public Image blackObject;

    public IEnumerator fadeToBlack(){
        blackObject.transform.gameObject.SetActive(true);
        float fadeSpeed = .5f;
        float fadeAmount;
        Color objectColour = blackObject.color;

        while(blackObject.color.a < 1){
            fadeAmount = objectColour.a + (fadeSpeed * Time.deltaTime);
            objectColour = new Color(objectColour.r, objectColour.g, objectColour.b, fadeAmount);
            blackObject.color = objectColour;
            yield return null;
        }
    }
}
