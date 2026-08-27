using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
[CreateAssetMenu(fileName = "ChanceBag", menuName = "Chance Bag")]
public class ChanceBag : ScriptableObject
{
    private Queue<bool> bag = new();
    public bool IsEmpty => bag.Count == 0;

    public void NewBag(float chance)
    {
        int maxDecimalPlaces = 2;
        chance = (float)Math.Round(chance, maxDecimalPlaces);

        int denominator = (int)Math.Pow(10, maxDecimalPlaces);
        int numerator = Mathf.RoundToInt(chance * denominator);

        int gcd = GCD(numerator, denominator);
        int successes = numerator / gcd;
        int entries = denominator / gcd;
        int failures = entries - successes;

        for (int i = 0; i < entries; i++)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < ((float)successes / (successes + failures)))
            {
                bag.Enqueue(true);
                successes--;
            } else
            {
                bag.Enqueue(false);
                failures--;
            }
        }
    }

    private int GCD(int a, int b)
    {
        while (b != 0) {
            (a, b) = (b, a % b);
        }
        return a;        
    }

    public bool Pull()
    {
        if (bag.TryDequeue(out bool returnVal))
        {
            return returnVal;   
        }

        Debug.LogWarning("QUEUE WAS EMPTY, RETURNING FALSE...");
        return false;
    }

    public void Clear()
    {
        bag.Clear();
    }
}
