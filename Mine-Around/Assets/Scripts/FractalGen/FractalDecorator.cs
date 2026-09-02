using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Fractal Decorator",
    menuName = "FractalGen/Decorator")]
public class FractalDecorator : ScriptableObject
{
    [SerializeField] private List<DecorationRule> rules = new();

    public void Apply(ref GenerationContext context)
    {
        if (rules == null)
            return;

        foreach (DecorationRule rule in rules)
        {
            if (rule == null)
                continue;

            rule.TryApply(ref context);
        }
    }
}
