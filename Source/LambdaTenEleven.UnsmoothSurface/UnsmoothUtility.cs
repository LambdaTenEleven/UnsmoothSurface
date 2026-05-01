using System.Collections.Generic;
using System.Linq;
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
            Log.Message($"[UnsmoothSurface] Cached floors: {FormatDefPairs(roughTerrainBySmoothedTerrain)}");
            Log.Message($"[UnsmoothSurface] Cached walls: {FormatDefPairs(roughWallBySmoothedWall)}");
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

        public static bool CanUnsmoothWall(IntVec3 cell, Map map)
        {
            return cell.InBounds(map) && RoughWallFor(cell.GetEdifice(map)) != null;
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
            var candidatesBySmoothedTerrain = new Dictionary<TerrainDef, List<TerrainDef>>();
            var terrainDefs = DefDatabase<TerrainDef>.AllDefsListForReading;

            // 1. load terrain patches
            foreach (var smoothedTerrain in terrainDefs)
            {
                var extension = smoothedTerrain.GetModExtension<UnsmoothSurfaceExtension>();
                if (IsUnsmoothDisabled(smoothedTerrain))
                {
                    continue;
                }

                var explicitRoughTerrain = extension?.unsmoothedTerrain;
                if (explicitRoughTerrain != null && explicitRoughTerrain != smoothedTerrain)
                {
                    roughTerrainBySmoothedTerrain[smoothedTerrain] = explicitRoughTerrain;
                }
            }

            // 2. load terrain defs
            foreach (var roughTerrain in terrainDefs)
            {
                var smoothedTerrain = roughTerrain.smoothedTerrain;
                if (smoothedTerrain != null && smoothedTerrain != roughTerrain && !IsUnsmoothDisabled(smoothedTerrain))
                {
                    AddCandidate(candidatesBySmoothedTerrain, smoothedTerrain, roughTerrain);
                }
            }

            // 3. resolve ambiguous terrain mappings
            foreach (var candidateGroup in candidatesBySmoothedTerrain)
            {
                if (roughTerrainBySmoothedTerrain.ContainsKey(candidateGroup.Key))
                {
                    continue;
                }

                var roughTerrain = ResolveTerrainCandidate(candidateGroup.Value);
                if (roughTerrain != null)
                {
                    roughTerrainBySmoothedTerrain[candidateGroup.Key] = roughTerrain;
                    continue;
                }

                Log.Warning($"[UnsmoothSurface] Skipping ambiguous floor mapping for {candidateGroup.Key.defName}. Candidates: {FormatDefs(candidateGroup.Value)}. Add {nameof(UnsmoothSurfaceExtension)} to choose an unsmoothed terrain.");
            }
        }

        private static TerrainDef ResolveTerrainCandidate(List<TerrainDef> candidates)
        {
            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            var roughHewnCandidates = candidates.Where(candidate => candidate.defName.EndsWith("_RoughHewn")).ToList();
            return roughHewnCandidates.Count == 1 ? roughHewnCandidates[0] : null;
        }

        private static void BuildWallLookup()
        {
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            var candidatesBySmoothedWall = new Dictionary<ThingDef, List<ThingDef>>();

            // 1. load wall patches
            foreach (var smoothedWall in thingDefs)
            {
                var extension = smoothedWall.GetModExtension<UnsmoothSurfaceExtension>();
                if (IsUnsmoothDisabled(smoothedWall))
                {
                    continue;
                }

                var explicitRoughWall = smoothedWall.building?.unsmoothedThing;
                if (explicitRoughWall?.building != null && explicitRoughWall != smoothedWall)
                {
                    roughWallBySmoothedWall[smoothedWall] = explicitRoughWall;
                    continue;
                }

                explicitRoughWall = extension?.unsmoothedThing;
                if (explicitRoughWall?.building != null && explicitRoughWall != smoothedWall)
                {
                    roughWallBySmoothedWall[smoothedWall] = explicitRoughWall;
                }
            }

            // 2. load wall defs
            foreach (var roughWall in thingDefs)
            {
                var smoothedWall = roughWall.building?.smoothedThing;
                if (smoothedWall?.building != null && smoothedWall != roughWall && !IsUnsmoothDisabled(smoothedWall))
                {
                    AddCandidate(candidatesBySmoothedWall, smoothedWall, roughWall);
                }
            }

            // 3. resolve ambiguous wall mappings
            foreach (var candidateGroup in candidatesBySmoothedWall)
            {
                if (roughWallBySmoothedWall.ContainsKey(candidateGroup.Key))
                {
                    continue;
                }

                if (candidateGroup.Value.Count == 1)
                {
                    roughWallBySmoothedWall[candidateGroup.Key] = candidateGroup.Value[0];
                    continue;
                }

                Log.Warning($"[UnsmoothSurface] Skipping ambiguous wall mapping for {candidateGroup.Key.defName}. Candidates: {FormatDefs(candidateGroup.Value)}. Add {nameof(UnsmoothSurfaceExtension)} to choose an unsmoothed wall.");
            }
        }

        private static bool IsUnsmoothDisabled<TDef>(TDef def) where TDef : Def
        {
            return def.GetModExtension<UnsmoothSurfaceExtension>()?.disableUnsmooth == true;
        }

        private static void AddCandidate<TDef>(Dictionary<TDef, List<TDef>> candidatesBySmoothedDef, TDef smoothedDef, TDef roughDef) where TDef : Def
        {
            if (!candidatesBySmoothedDef.TryGetValue(smoothedDef, out var candidates))
            {
                candidates = new List<TDef>();
                candidatesBySmoothedDef[smoothedDef] = candidates;
            }

            if (!candidates.Contains(roughDef))
            {
                candidates.Add(roughDef);
            }
        }

        private static string FormatDefPairs<TDef>(Dictionary<TDef, TDef> pairs) where TDef : Def
        {
            return pairs.Any()
                ? string.Join(", ", pairs.Select(pair => $"{pair.Key.defName} -> {pair.Value.defName}"))
                : "none";
        }

        private static string FormatDefs<TDef>(IEnumerable<TDef> defs) where TDef : Def
        {
            return string.Join(", ", defs.Select(def => def.defName));
        }
    }
}
