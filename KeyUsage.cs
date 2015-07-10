using System.Collections.Generic;

/// <summary>
/// A basic comparer to avoid Dictionary allocation during indexing.
/// 
/// Mono doesn't like having enum's as the key in a dictionary and allocates
/// memory that then needs to be cleared by the GC (damned DefaultComparer)
/// </summary>
public sealed class KeyUsageComparer : IEqualityComparer<KeyUsage>
{
    public bool Equals(KeyUsage a, KeyUsage b)
    {
        return a == b;
    }

    public int GetHashCode(KeyUsage obj)
    {
        return (int)obj;
    }
}

public enum KeyUsage
{
    Ability1,
    Ability2,
    Ability3,
    Ability4,
    Recall,
    Inventory1,
    Inventory2,
    Inventory3,
    Inventory4,
    Inventory5,
    Inventory6,
    Inventory7,
    Stop,
}
