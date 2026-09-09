using UnityEngine;

public abstract class Selector<T>
{
    public abstract T SelectItem(int seedNumber);
}
