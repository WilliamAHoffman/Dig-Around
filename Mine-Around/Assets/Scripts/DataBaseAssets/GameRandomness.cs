public static class GameRandomness
{
    public static int StableHash(string text)
    {
        unchecked
        {
            int hash = 23;

            if (string.IsNullOrEmpty(text))
                return hash;

            for (int i = 0; i < text.Length; i++)
            {
                hash = hash * 31 + text[i];
            }

            return hash;
        }
    }

    public static int Hash(int seed, int x, int y, int salt)
    {
        unchecked
        {
            int hash = seed;

            hash ^= x * 73856093;
            hash ^= y * 19349663;
            hash ^= salt * 83492791;

            hash ^= hash >> 13;
            hash *= 1274126177;
            hash ^= hash >> 16;

            return hash;
        }
    }

    public static int HashSeed(int worldSeed, string id, int salt)
    {
        return Hash(worldSeed, StableHash(id), 0, salt);
    }

    public static int Range(int worldSeed, string id, int salt, int minInclusive, int maxExclusive)
    {
        int range = maxExclusive - minInclusive;

        if (range <= 0)
            return minInclusive;

        int hash = HashSeed(worldSeed, id, salt) & 0x7fffffff;

        return minInclusive + hash % range;
    }

    public static float Value01(int seed, int x, int y, int salt)
    {
        int hash = Hash(seed, x, y, salt);
        return (hash & 0x7fffffff) / (float)int.MaxValue;
    }

    public static bool Chance(int seed, int x, int y, int salt, float probability)
    {
        if (probability <= 0f)
            return false;

        if (probability >= 1f)
            return true;

        return Value01(seed, x, y, salt) < probability;
    }
}