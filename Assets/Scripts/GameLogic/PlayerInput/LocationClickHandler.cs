using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationClickHandler : MonoBehaviour
{   
    public ActionManager actionManager;

    public void OnMouseDown(){
        StartCoroutine(actionManager.handleLocClick(gameObject.GetComponent<Location>()));
    }
}
