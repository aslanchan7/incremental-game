using System;
using System.Collections.Generic;
using NUnit.Framework;
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
            }
            else
            {
                bag.Enqueue(false);
                failures--;
            }
        }
    }

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }
        return a;
    }

    public bool Pull(float chance)
    {
        if (IsEmpty)
            NewBag(chance);

        if (bag.TryDequeue(out bool returnVal))
        {
            return returnVal;
        }

        Debug.LogWarning("CHANCE BAG: SOMETHING WENT WRONG, RETURNING FALSE...");
        return false;
    }

    public void Clear()
    {
        bag.Clear();
    }
}
