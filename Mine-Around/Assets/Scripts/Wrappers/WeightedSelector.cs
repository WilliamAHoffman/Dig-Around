using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class WeightedItem<T>
{
    public int weight = 1;
    public T item;
}

[Serializable]
public class WeightedSelector<T> : Selector<T>
{
    [SerializeField] private List<WeightedItem<T>> items;
    public override T SelectItem(int seedNumber)
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