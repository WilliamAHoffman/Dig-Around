using UnityEngine;

[CreateAssetMenu(fileName = "WallData", menuName = "Scriptable Objects/WallData")]
public class WallData : ScriptableObject
{
    public RuleTile tile;
    public string displayName;
    public string nameID;
    public int maxHealth;
    public bool invincible;
}
[CreateAssetMenu(fileName = "FloorData", menuName = "Scriptable Objects/FloorData")]
public class FloorData : ScriptableObject
{
    public RuleTile tile;
    public string displayName;
    public string nameID;
    public int maxHealth;
    public bool invincible;
}