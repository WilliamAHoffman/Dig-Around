using UnityEngine;

[CreateAssetMenu(fileName = "WallData", menuName = "Scriptable Objects/WallData")]
public class WallData : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Wall;
    public RuleTile tile;
    public int maxHealth;
    public bool invincible;
}
[CreateAssetMenu(fileName = "FloorData", menuName = "Scriptable Objects/FloorData")]
public class FloorData : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Floor;
    public RuleTile tile;
    public int maxHealth;
    public bool invincible;
}