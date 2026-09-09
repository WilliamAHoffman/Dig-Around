using System;
using System.Collections.Generic;

[Serializable]
public class RandomSelector<T> : Selector<T>
{
    public List<T> items;
    public override T SelectItem(int seedNumber)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("The list cannot be empty.");
        
        if(items.Count == 1) return items[0];

        return items[seedNumber%items.Count];
    }
}