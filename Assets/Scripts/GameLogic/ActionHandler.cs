using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public ActionManager actionManager;

    public void OnMouseDown(){
        actionManager.handleActionButtonClick(this.name);
    }
}
