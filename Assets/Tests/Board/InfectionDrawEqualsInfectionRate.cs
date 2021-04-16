using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class InfectionDrawEqualsInfectionRate
    {
        // A Test behaves as an ordinary method
        Board board = GameObject.Find("Board").GetComponent<Board>();

        [Test]
        public void InfectionDrawEqualsInfectionRateSimplePasses()
        {
            // Use the Assert class to test conditions
           // Board.infectionDeck.getInfectionDeck().Count();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator InfectionDrawEqualsInfectionRateWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
