using System.Collections.Generic;
using Verse;

namespace LambdaTenEleven.UnsmoothSurface
{
    [StaticConstructorOnStartup]
    public static class UnsmoothUtility
    {
        private static readonly Dictionary<TerrainDef, TerrainDef> roughTerrainBySmoothedTerrain = new Dictionary<TerrainDef, TerrainDef>();
        private static readonly Dictionary<ThingDef, ThingDef> roughWallBySmoothedWall = new Dictionary<ThingDef, ThingDef>();

        static UnsmoothUtility()
        {
            BuildTerrainLookup();
            BuildWallLookup();
            Log.Message($"[UnsmoothSurface] Initialized with {roughTerrainBySmoothedTerrain.Count} terrain pairs and {roughWallBySmoothedWall.Count} wall pairs.");
        }

        public static bool CanUnsmooth(IntVec3 cell, Map map)
        {
            if (!cell.InBounds(map))
            {
                return false;
            }

            return RoughTerrainFor(map.terrainGrid.TerrainAt(cell)) != null ||
                   RoughWallFor(cell.GetEdifice(map)) != null;
        }

        public static bool CanUnsmoothWall(Thing thing)
        {
            return RoughWallFor(thing) != null;
        }

        public static bool TryUnsmooth(IntVec3 cell, Map map)
        {
            var edifice = cell.GetEdifice(map);
            var roughWall = RoughWallFor(edifice);
            if (roughWall != null)
            {
                edifice.Destroy(DestroyMode.WillReplace);
                GenSpawn.Spawn(roughWall, cell, map);
                return true;
            }

            var roughTerrain = RoughTerrainFor(map.terrainGrid.TerrainAt(cell));
            if (roughTerrain != null)
            {
                map.terrainGrid.SetTerrain(cell, roughTerrain);
                return true;
            }

            return false;
        }

        private static TerrainDef RoughTerrainFor(TerrainDef terrain)
        {
            return terrain != null && roughTerrainBySmoothedTerrain.TryGetValue(terrain, out var roughTerrain)
                ? roughTerrain
                : null;
        }

        private static ThingDef RoughWallFor(Thing edifice)
        {
            return edifice == null ? null : roughWallBySmoothedWall.GetValueOrDefault(edifice.def);
        }

        private static void BuildTerrainLookup()
        {
            foreach (var roughTerrain in DefDatabase<TerrainDef>.AllDefsListForReading)
            {
                var smoothedTerrain = roughTerrain.smoothedTerrain;
                if (smoothedTerrain != null && smoothedTerrain != roughTerrain)
                {
                    roughTerrainBySmoothedTerrain[smoothedTerrain] = roughTerrain;
                }
            }

            // manual lookup if a mod adds a smoothed terrain without a smoothed terrain def
            // foreach (var smoothedTerrain in terrainDefs)
            // {
            //     if (!smoothedTerrain.defName.EndsWith(UnsmoothConstants.DefNamePatterns.SmoothSuffix) || roughTerrainBySmoothedTerrain.ContainsKey(smoothedTerrain))
            //     {
            //         continue;
            //     }
            //
            //     var roughHewnDefName = smoothedTerrain.defName.Replace(
            //         UnsmoothConstants.DefNamePatterns.SmoothSuffix,
            //         UnsmoothConstants.DefNamePatterns.RoughHewnSuffix);
            //     var roughHewnTerrain = DefDatabase<TerrainDef>.GetNamedSilentFail(roughHewnDefName);
            //     if (roughHewnTerrain != null)
            //     {
            //         roughTerrainBySmoothedTerrain[smoothedTerrain] = roughHewnTerrain;
            //         continue;
            //     }
            //
            //     var roughTerrainDefName = smoothedTerrain.defName.Substring(
            //         0,
            //         smoothedTerrain.defName.Length - UnsmoothConstants.DefNamePatterns.SmoothSuffix.Length);
            //     var roughTerrain = DefDatabase<TerrainDef>.GetNamedSilentFail(roughTerrainDefName);
            //     if (roughTerrain != null)
            //     {
            //         roughTerrainBySmoothedTerrain[smoothedTerrain] = roughTerrain;
            //     }
            // }
        }

        private static void BuildWallLookup()
        {
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            foreach (var smoothedWall in thingDefs)
            {
                var roughWall = smoothedWall.building?.unsmoothedThing;
                if (roughWall?.building != null && roughWall != smoothedWall)
                {
                    roughWallBySmoothedWall[smoothedWall] = roughWall;
                }
            }

            foreach (var roughWall in thingDefs)
            {
                var smoothedWall = roughWall.building?.smoothedThing;
                if (smoothedWall?.building != null && smoothedWall != roughWall && !roughWallBySmoothedWall.ContainsKey(smoothedWall))
                {
                    roughWallBySmoothedWall[smoothedWall] = roughWall;
                }
            }

            // manual lookup if a mod adds a smoothed wall without a smoothed thing
            // foreach (var smoothedWall in thingDefs)
            // {
            //     if (!smoothedWall.defName.StartsWith(UnsmoothConstants.DefNamePatterns.SmoothedPrefix) || roughWallBySmoothedWall.ContainsKey(smoothedWall))
            //     {
            //         continue;
            //     }
            //
            //     var roughWallDefName = smoothedWall.defName.Substring(UnsmoothConstants.DefNamePatterns.SmoothedPrefix.Length);
            //     var roughWall = DefDatabase<ThingDef>.GetNamedSilentFail(roughWallDefName);
            //     if (roughWall?.building != null)
            //     {
            //         roughWallBySmoothedWall[smoothedWall] = roughWall;
            //     }
            // }
        }
    }
}
