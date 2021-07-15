using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckGenerator
{
    private Board board;
    private PlayerManager playerManager;

    public PlayerDeckGenerator(Board board, PlayerManager playerManager){
        this.board = board;
        this.playerManager = playerManager;
    }
    public void preparePlayerCards(Location[] locations){
        List<PlayerCard> cityCards = new List<PlayerCard>();
        for (int i = 0; i < locations.Length; i++){
            cityCards.Add(new PlayerCard(locations[i], i, locations[i].gameObject.name));
        }

        cityCards.Add(new PlayerCard(null, Vals.ONE_QUIET_NIGHT, "One Quiet Night"));
        cityCards.Add(new PlayerCard(null, Vals.AIRLIFT, "Airlift"));
        cityCards.Add(new PlayerCard(null, Vals.GOVERNMENT_GRANT, "Government Grant"));
        cityCards.Add(new PlayerCard(null, Vals.FORECAST, "Forecast"));
        cityCards.Add(new PlayerCard(null, Vals.RESILIENT_POPULATION, "Resilient Population"));

        Stack<PlayerCard> shuffledPlayerCards = new Stack<PlayerCard>();
        Utils.ShuffleAndPlaceOnTop(cityCards, shuffledPlayerCards);
        dealInitialPlayerHands(shuffledPlayerCards);
        createPlayerDeck(shuffledPlayerCards);
    }

    public void dealInitialPlayerHands(Stack<PlayerCard> shuffledPlayerCards){
        int cardsRequired = playerManager.getInitialHandCount();
        List<PlayerCard> initialHandCards = new List<PlayerCard>();
        for (int i = 0; i < cardsRequired; i++){
            PlayerCard drawn = shuffledPlayerCards.Pop();
            initialHandCards.Add(drawn);
            if (drawn.Colour == Vals.Colour.EVENT){board.AvailableEventCards.Add(drawn);}
        }
        playerManager.setInitialHands(initialHandCards);
    }

    public void createPlayerDeck(Stack<PlayerCard> shuffledPlayerCards){
        // Create  and stack epidemic card piles with smallest piles at the bottom
        int decrementingPileCount = board.EpidemicCardCount;
        for (int k = 0; k < board.EpidemicCardCount; k++){
            int pileSize = shuffledPlayerCards.Count / decrementingPileCount;
            List<PlayerCard> curPile = new List<PlayerCard>();
            for (int j = 0; j < pileSize; j++){
                curPile.Add(shuffledPlayerCards.Pop());
            }
            curPile.Add(new PlayerCard(null, Vals.EPIDEMIC, "Epidemic"));
            Utils.ShuffleAndPlaceOnTop(curPile, board.PlayerDeck);
            decrementingPileCount--;     
        } 
    }
}
