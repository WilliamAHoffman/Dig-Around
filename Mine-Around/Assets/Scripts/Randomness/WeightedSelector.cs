using System;
using System.Collections.Generic;

[Serializable]
public class WeightedItem<T>
{
    public int weight;
    public T item;
}

public static class WeightedRandomSelector
{
    public static T GetWeightedRandom<T>(List<WeightedItem<T>> items, int seedNumber)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("The list cannot be empty.");
        
        if(items.Count == 1) return items[0].item;

        int totalWeight = 0;

        foreach (var item in items)
            totalWeight += item.weight;

        int randomNumber =
            (int)((uint)seedNumber % (uint)totalWeight);

        foreach (var item in items)
        {
            int weight = item.weight;

            if (randomNumber < weight)
                return item.item;

            randomNumber -= weight;
        }

        return items[0].item;
    }
}