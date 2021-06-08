using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionHandler : MonoBehaviour
{
    private ActionManager actionManager;
    public GameObject parent;
    public GameObject playerObject;
    
    void Awake()
    {
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();    
    }

    public void OnMouseDown(){
        StartCoroutine(actionManager.handleCharacterActionClick(playerObject.GetComponent<Player>()));
    }

    public void activateCharacterActionButton(){
        parent.SetActive(true);
    }
}
