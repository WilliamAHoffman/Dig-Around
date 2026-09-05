using UnityEngine;

public abstract class GenerationCondition : ScriptableObject
{
    public abstract bool Evaluate(in GenerationContext context);
}
