using Verse;

namespace LambdaTenEleven.UnsmoothSurface;

public class UnsmoothSurfaceSettings : ModSettings
{
    public const float DefaultFloorWorkRequired = 1000f;
    public const float DefaultWallWorkRequired = 1500f;
    public const float MinWorkRequired = 250f;
    public const float MaxWorkRequired = 6500f;

    public float floorWorkRequired = DefaultFloorWorkRequired;
    public float wallWorkRequired = DefaultWallWorkRequired;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref floorWorkRequired, UnsmoothConstants.ScribeKeys.FloorWorkRequired, DefaultFloorWorkRequired);
        Scribe_Values.Look(ref wallWorkRequired, UnsmoothConstants.ScribeKeys.WallWorkRequired, DefaultWallWorkRequired);
        ClampValues();
    }

    public float WorkRequiredFor(IntVec3 cell, Map map)
    {
        return UnsmoothUtility.CanUnsmoothWall(cell, map) ? wallWorkRequired : floorWorkRequired;
    }

    public void ResetToDefaults()
    {
        floorWorkRequired = DefaultFloorWorkRequired;
        wallWorkRequired = DefaultWallWorkRequired;
    }

    public void ClampValues()
    {
        floorWorkRequired = ClampWorkAmount(floorWorkRequired);
        wallWorkRequired = ClampWorkAmount(wallWorkRequired);
    }

    private static float ClampWorkAmount(float value)
    {
        if (value < MinWorkRequired)
        {
            return MinWorkRequired;
        }

        return value > MaxWorkRequired ? MaxWorkRequired : value;
    }
}
