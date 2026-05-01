# Unsmooth Surface

My first Rimworld mod that allows players to unsmooth smoothed surfaces.

It adds a new option "Unsmooth Surface" to the Orders menu and a new gizmo to smoothed walls.

Adds a new job `UnsmoothSurfaceJob` (check [Jobs.xml](Defs/Jobs.xml)) that requries Mining skill and gives the same amout of exp as mining.
    
## Compatibility

In theory, it should support modded rocky walls and floors, tested it with Alpha Biomes.

[UnsmoothUtility.cs](Source/LambdaTenEleven.UnsmoothSurface/UnsmoothUtility.cs) builds two lookups for walls and floors after the game loads.

If several rough floors or walls point to the same smoothed def, the mod skips that ambiguous mapping and logs a warning. Modders can resolve it with a `DefModExtension` on the smoothed def:

```xml
<li Class="LambdaTenEleven.UnsmoothSurface.UnsmoothSurfaceExtension">
  <unsmoothedTerrain>Some_RoughTerrainDef</unsmoothedTerrain>
</li>
```

For walls, use `<unsmoothedThing>Some_RoughWallThingDef</unsmoothedThing>` instead.

To intentionally prevent a smoothed floor or wall from being unsmoothed, add:

```xml
<li Class="LambdaTenEleven.UnsmoothSurface.UnsmoothSurfaceExtension">
  <disableUnsmooth>true</disableUnsmooth>
</li>
```

The mod includes a patch for Quarry mod to turn the "Reclaimed soil" back into the "Quarried ground. Tbf I don't know if it does make sense, so if you want, you can disable it by editing [QuarryCompatibilityPatch.xml](Patches/QuarryCompatibilityPatch.xml) and using `disableUnsmooth` as in the example above.

## Contributing

Feel free to fork this repo and make pull requests. You can reupload this mod to Steam Workshop if I stop supporting it.

## License

MIT License. See [LICENSE](LICENSE).
