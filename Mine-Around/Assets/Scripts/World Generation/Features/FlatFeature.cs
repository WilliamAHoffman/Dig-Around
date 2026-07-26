using UnityEngine;

[CreateAssetMenu(fileName = "FlatFeature", menuName = "World Generation/Features/Flat Feature")]
public class FlatFeature : GenerationFeature
{
    [SerializeField] GenerationRule rule;
    public override MapCell Apply(Vector2Int location, float strength, MapCell result)
    {
        return rule.Apply(result);
    }
}