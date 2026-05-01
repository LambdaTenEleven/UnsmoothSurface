# Unsmooth Surface

My first Rimworld mod that allows players to unsmooth smoothed surfaces.

It adds a new option "Unsmooth Surface" to the Orders menu and a new gizmo to smoothed walls.

## Compatibility

In theory, it should support modded rocky walls and floors, tested it with Alpha Biomes.

[SmoothedWallGizmoPatch.xml](Patches/SmoothedWallGizmoPatch.xml) shows the gizmo on `smoothedThing`, so it should work with any smoothed wall.

And for the Order action, [UnsmoothUtility.cs](Source/LambdaTenEleven.UnsmoothSurface/UnsmoothUtility.cs) builds two lookups for walls and floors after the game loads.

## Contributing

Feel free to fork this repo and make pull requests.

## License

MIT License. See [LICENSE](LICENSE).
