namespace LambdaTenEleven.UnsmoothSurface
{
    internal static class UnsmoothConstants
    {
        internal static class DefNames
        {
            internal const string DrawStyleDefault2D = "Default2D";
            internal const string PickHitSound = "PickHit";
            internal const string UnsmoothDesignation = "UnsmoothSurfaceDes";
            internal const string UnsmoothJob = "UnsmoothSurfaceJob";
        }

        internal static class TexturePaths
        {
            internal const string CancelDesignator = "UI/Designators/Cancel";
            internal const string UnsmoothDesignator = "UI/Designators/UnsmoothSurface";
        }

        internal static class TranslationKeys
        {
            internal const string DesignatorCancel = "DesignatorCancel";
            internal const string DesignatorCancelDesc = "DesignatorCancelDesc";
        }

        internal static class Labels
        {
            internal const string UnsmoothSurface = "Unsmooth surface";
        }

        internal static class Descriptions
        {
            internal const string UnsmoothDesignator = "Break up smoothed rock floors and walls, reverting them to rough stone.";
            internal const string UnsmoothWall = "Break up this smoothed rock wall, reverting it to rough stone.";
        }

        internal static class Messages
        {
            internal const string AlreadyDesignated = "Already designated.";
            internal const string MustDesignateSmoothedSurface = "Must designate smoothed rock floor or wall.";
        }

        internal static class TutorTags
        {
            internal const string Cancel = "Cancel";
        }

        internal static class ToilNames
        {
            internal const string UnsmoothSurface = "UnsmoothSurface";
            internal const string FinishUnsmoothSurface = "FinishUnsmoothSurface";
        }

        internal static class ScribeKeys
        {
            internal const string WorkLeft = "workLeft";
        }

        internal static class DefNamePatterns
        {
            internal const string SmoothSuffix = "_Smooth";
            internal const string RoughHewnSuffix = "_RoughHewn";
            internal const string SmoothedPrefix = "Smoothed";
        }
    }
}
