using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectablePlayer : MonoBehaviour, ISelectable<Player>
{   

    private Player player;
    public Text label;
    public Sprite[] sprites;

    public void OnMouseDown(){  
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().setUserSelectedPlayer(player);
        Vals.continueAfterSelect = true;
    }

    public void populateItemData(Player player){
        this.player = player;
        label.text = player.getRole().Name;
    }

   public Sprite getSprite(){
       return sprites[player.getRole().ID];
   }

   public bool isSelected(){
       return false;
   }

      public Player getSelectedValue(){
       return player;
   }

}
