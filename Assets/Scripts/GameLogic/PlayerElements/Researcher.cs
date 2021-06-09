using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Researcher : Role
{
    public Researcher(){
        id = Vals.RESEARCHER;
        name = Vals.ROLES[id];
    }

    public override void findGiveableCards(Player player, List<PlayerCard> potentialCards){
        foreach(PlayerCard card in player.getHand()){
            if (card.getColour() != Vals.Colour.EVENT){
                potentialCards.Add(card);
            }
        }
    }
}
