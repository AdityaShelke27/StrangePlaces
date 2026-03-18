using System.Collections.Generic;

public enum SurfaceNode
{
    Ore,
    Plant,
    Gravitational_Anomaly,
    Alien_Artifact
}
public enum SurfaceNodeAmount
{
    Small,
    Medium,
    Large,
    Very_Large
}
public static class ResourceData
{
    public static readonly Dictionary<SurfaceNodeAmount, int> ResourceAmount = new()
    {
        { SurfaceNodeAmount.Small, 10 },
        { SurfaceNodeAmount.Medium, 25 },
        { SurfaceNodeAmount.Large, 50 },
        { SurfaceNodeAmount.Very_Large, 100 },
    };
}