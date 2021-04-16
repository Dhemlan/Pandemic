using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    
    public class TopCardDrawnOnInfectionDraw
    {
        // A Test behaves as an ordinary method
        InfectionDeck deck = GameObject.Find("InfectionDeck").GetComponent<InfectionDeck>();
              
        [Test]
        public void TopCardDrawnOnInfectionDrawSimplePasses()
        {    
            deck.Start();
            int count = deck.getInfectionDeck().Count;
            InfectionCard peek = deck.getInfectionDeck().Peek();
            InfectionCard drawn = deck.drawInfectionCard();
                      
            Assert.AreEqual(peek.getId(), drawn.getId());
            Assert.AreEqual(count - 1, deck.getInfectionDeck().Count);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TopCardDrawnOnInfectionDrawWithEnumeratorPasses()
        {
            InfectionDeck deck = GameObject.Find("InfectionDeck").GetComponent<InfectionDeck>();
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            InfectionCard peek = deck.getInfectionDeck().Peek();
            InfectionCard drawn = deck.drawInfectionCard();            
            Assert.AreEqual(peek.getId(), drawn.getId());
        }
    }
}
