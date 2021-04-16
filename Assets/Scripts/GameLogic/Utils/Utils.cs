using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    static System.Random rnd = new System.Random();

    static public void ShuffleAndPlaceOnTop<T>(List<T> toShuffle, Stack<T> existingDeck){
        for (int n = toShuffle.Count - 1; n >= 0; n--)
        {
            int k = rnd.Next(n + 1);
            existingDeck.Push(toShuffle[k]);
            toShuffle.RemoveAt(k);
        }
    }
}
