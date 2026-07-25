public enum AssetCategory
{
    tile,
    noise
}
public struct AssetReference
{
    public AssetCategory Category;
    public int ID;

    public AssetReference(AssetCategory Category, int ID)
    {
        this.ID = ID;
        this.Category = Category;
    }
}