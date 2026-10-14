# Immediate Cleanup For Unsmooth Orders On Terrain Change

## Summary

Use a Harmony patch on `TerrainGrid.SetTerrain` to remove `UnsmoothSurfaceDes` designations immediately when the target cell stops being unsmoothable. This matches the user-visible vanilla behavior for overlays without adding a constant map scan.

## Key Changes

- Add Harmony as a compile-time dependency only, relying on RimWorld's loaded Harmony at runtime.
- Initialize patches once from `UnsmoothSurfaceMod` with a stable Harmony ID such as `LambdaTenEleven.UnsmoothSurface`.
- Add a postfix patch for `Verse.TerrainGrid.SetTerrain(IntVec3 c, TerrainDef newTerr)`.
- In the postfix:
  - Get the `map` from the patched `TerrainGrid` instance.
  - Check for this mod's designation at `c`.
  - If present and `UnsmoothUtility.CanUnsmooth(c, map)` is now false, remove the designation.
- Keep the existing job `FailOn(() => !UnsmoothUtility.CanUnsmooth(TargetCell, Map))` so active jobs stop cleanly when the surface changes mid-job.
- Optionally add a helper like `UnsmoothUtility.RemoveUnsmoothDesignationIfInvalid(IntVec3 cell, Map map)` so the patch and any future cleanup path share the same logic.

## Test Plan

- Build with `dotnet build Source\LambdaTenEleven.UnsmoothSurface\LambdaTenEleven.UnsmoothSurface.csproj --configuration Release`.
- Manual in-game checks:
  - Mark a smoothed rock floor for unsmoothing.
  - Change that floor to another terrain, including vanilla smoothing/desmoothing flows and archean tree soil conversion.
  - Confirm the unsmooth overlay disappears immediately after terrain change.
  - Confirm valid unsmooth floor and wall orders remain untouched.
  - Start an unsmooth job, change the terrain mid-work, and confirm the pawn job stops without an exception from this mod.
- Regression checks:
  - Completing an unsmooth job still changes terrain/walls and removes the designation.
  - Manual cancel still works.
  - God mode instant unsmooth behavior still removes the designation.

## Assumptions

- Adding Harmony is acceptable because it avoids periodic scanning and gives immediate cleanup.
- The patch targets terrain changes only; wall despawn cleanup remains handled by `removeIfBuildingDespawned` and existing job logic.
- If vanilla itself throws an unrelated exception during its own smoothing job, this mod will not attempt to patch vanilla smoothing behavior globally.
