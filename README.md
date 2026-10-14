# Unsmooth Surface

[![RimWorld 1.6 and 1.7](https://img.shields.io/badge/RimWorld-1.6%20%7C%201.7-blue)](#compatibility)
[![License: MIT](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Steam Workshop](https://img.shields.io/badge/Steam_Workshop-Subscribe-1b2838?logo=steam&logoColor=white)](https://steamcommunity.com/sharedfiles/filedetails/?id=3717703166)

![Image](/About/Preview.png)

Get the mod on [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3717703166).

My first Rimworld mod that allows players to unsmooth smoothed surfaces.

It adds a new option "Unsmooth Surface" to the Orders menu and a new gizmo to smoothed walls.

Adds a new job `UnsmoothSurfaceJob` (check [Jobs.xml](Defs/Jobs.xml)) that requries Mining skill and gives the same amout of exp as mining.

## Compatibility

Supports 1.6 and 1.7 version.

In theory, the mod should support modded rocky walls and floors, tested it with Alpha Biomes.

When unsmoothing vanilla terrain, the `_RoughHewn` will be chosen.

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

The mod includes a patch for Quarry mod to turn the "Reclaimed soil" back into the "Quarried ground." Tbf I don't know if it does make sense, so if you want, you can disable it by editing [QuarryCompatibilityPatch.xml](Patches/QuarryCompatibilityPatch.xml) and using `disableUnsmooth` as in the example above.

## Contributing

Feel free to fork this repo and make pull requests. You can reupload this mod to Steam Workshop if I stop supporting it.

If you notice any bugs, please report them on the issues page with logs/screenshots attached.

## License

MIT License. See [LICENSE](LICENSE).
