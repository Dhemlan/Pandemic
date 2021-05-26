using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role
{   
    protected int cardsToCure = Vals.DEFAULT_CARDS_TO_CURE;
    private bool researchStationRequiredToCure = true;
    protected int id;
    protected string name;

    public int getCardsToCure(){
        return cardsToCure;
    }

    public bool getResearchStationRequiredToCure(){
        return researchStationRequiredToCure;
    }

    public int getID(){
        return id;
    }

    public string getName(){
        return name;
    }
    
    public virtual bool buildAction(Player player){
        Debug.Log("default build");
        foreach (PlayerCard card in player.getHand()){
            if (player.getLocation().Equals(card.getLocation())){
                player.discardCard(card);
                return true;
            }
        }
        return false;
    }

    public virtual bool otherMovement(){
        return false;
    }

    public virtual bool treatAction(){
        return false;
    }

    public virtual void findGiveableCards(Player player, List<PlayerCard> potentialCards){
        if (player.hasCardByLoc(player.getLocation())){
            Debug.Log("can give " + player.getLocation());
            potentialCards.Add(player.retrieveCardByLoc(player.getLocation()));
        }
    }

    public virtual bool nonStandardMove(Player player){
        return false;
    }
}
