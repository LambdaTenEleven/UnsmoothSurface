using UnityEngine;
using Verse;

namespace LambdaTenEleven.UnsmoothSurface;

public class UnsmoothSurfaceMod : Mod
{
    public static UnsmoothSurfaceSettings Settings { get; private set; } = new UnsmoothSurfaceSettings();

    public UnsmoothSurfaceMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<UnsmoothSurfaceSettings>();
    }

    public override string SettingsCategory()
    {
        return UnsmoothConstants.TranslationKeys.SettingsCategory.Translate();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing = new Listing_Standard();
        listing.Begin(inRect);
        listing.Label(UnsmoothConstants.TranslationKeys.SettingsWorkAmountNote.Translate());

        Settings.floorWorkRequired = DrawWorkSlider(
            listing,
            UnsmoothConstants.TranslationKeys.SettingsFloorWorkAmount.Translate(),
            Settings.floorWorkRequired);

        Settings.wallWorkRequired = DrawWorkSlider(
            listing,
            UnsmoothConstants.TranslationKeys.SettingsWallWorkAmount.Translate(),
            Settings.wallWorkRequired);

        listing.Gap();
        if (listing.ButtonText(UnsmoothConstants.TranslationKeys.SettingsResetDefaults.Translate(), widthPct: 0.35f))
        {
            Settings.ResetToDefaults();
        }

        listing.End();
        Settings.ClampValues();
    }

    public override void WriteSettings()
    {
        Settings.ClampValues();
        base.WriteSettings();
    }

    private static float DrawWorkSlider(Listing_Standard listing, string label, float value)
    {
        var roundedValue = Mathf.Round(value);
        var sliderLabel = $"{label}: {roundedValue:0}";
        return Mathf.Round(listing.SliderLabeled(
            sliderLabel,
            roundedValue,
            UnsmoothSurfaceSettings.MinWorkRequired,
            UnsmoothSurfaceSettings.MaxWorkRequired));
    }
}
