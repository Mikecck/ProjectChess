using UnityEngine;

[System.Serializable]
public struct GridPosition
{
    public int x, y, z;
    
    public GridPosition(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public bool IsValid() => x >= 0 && x < 3 && y >= 0 && y < 3 && z >= 0 && z < 3;
    
    public override string ToString()
    {
        return $"({x},{y},{z})";
    }
    
    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }
    
    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }
    
    public override bool Equals(object obj)
    {
        if (!(obj is GridPosition))
            return false;
            
        GridPosition other = (GridPosition)obj;
        return this == other;
    }
    
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
    }
}