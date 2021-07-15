using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role 
{   
    protected int cardsToCure = Vals.DEFAULT_CARDS_TO_CURE;
    private bool researchStationRequiredToCure = true;
    protected int id;
    protected string name;
    protected bool usedThisRound = false;
    protected PlayerManager playerManager;

    public int getCardsToCure(){
        return cardsToCure;
    }

    public bool getResearchStationRequiredToCure(){
        return researchStationRequiredToCure;
    }
    
    public virtual bool buildAction(Player player){
        Debug.Log("default build");
        foreach (PlayerCard card in player.getHand()){
            if (player.CurLoc.Equals(card.Location)){
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

    public void resetOncePerTurnActions(){
        usedThisRound = false; 
    }

    public virtual void findGiveableCards(Player player, List<PlayerCard> potentialCards){
        if (player.hasCardByLoc(player.CurLoc)){
            Debug.Log("can give " + player.CurLoc);
            potentialCards.Add(player.retrieveCardByLoc(player.CurLoc));
        }
    }

    public virtual bool nonStandardMove(Player player, Location loc){
        return false;
    }

    public virtual void enterLocation(Board board, Location loc){

    }

    public virtual IEnumerator characterAction<T>(T itemToActOn){
        Debug.Log(id + "character action");
        yield break;
    } 

    public int ID { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
}
