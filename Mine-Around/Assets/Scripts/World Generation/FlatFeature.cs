using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FlatFeature", menuName = "World Generation/Features/Flat Feature")]
public class FlatFeature : GenerationFeature
{
    [SerializeField] GenerationRule rule;
    public override GenerationResult Apply(Vector2Int location, float strength, GenerationResult result)
    {
        return rule.Apply(result);
    }
}