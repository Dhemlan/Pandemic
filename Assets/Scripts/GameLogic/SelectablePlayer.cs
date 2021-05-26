using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectablePlayer : MonoBehaviour, ISelectable<Player>
{   

    private Player player;
    public Text label;
    public Sprite[] sprites;

    public void populateItemData(Player player){
        this.player = player;
        label.text = player.getRole().getName();
    }

   public Sprite getSprite(){
       return sprites[player.getRole().getID()];
   }

}
