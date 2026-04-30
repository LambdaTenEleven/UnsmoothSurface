using RimWorld;
using UnityEngine;
using Verse;

namespace LambdaTenEleven.UnsmoothSurface
{
    public class Designator_Unsmooth : Designator_Cells
    {
        public override DrawStyleCategoryDef DrawStyleCategory => DefDatabase<DrawStyleCategoryDef>.GetNamed(UnsmoothConstants.DefNames.DrawStyleDefault2D);

        private static DesignationDef UnsmoothDesignationDef => DefDatabase<DesignationDef>.GetNamed(UnsmoothConstants.DefNames.UnsmoothDesignation);

        protected override DesignationDef Designation => UnsmoothDesignationDef;

        public override bool DragDrawMeasurements => true;

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }

        public Designator_Unsmooth()
        {
            defaultLabel = UnsmoothConstants.TranslationKeys.DesignatorUnsmoothSurface.Translate();
            defaultDesc = UnsmoothConstants.TranslationKeys.DesignatorUnsmoothSurfaceDesc.Translate();
            icon = ContentFinder<Texture2D>.Get(UnsmoothConstants.TexturePaths.UnsmoothDesignator);
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            useMouseIcon = true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(Map))
            {
                return false;
            }

            if (!DebugSettings.godMode && Map.designationManager.DesignationAt(c, UnsmoothDesignationDef) != null)
            {
                return UnsmoothConstants.TranslationKeys.SurfaceBeingUnsmoothed.Translate();
            }

            if (!UnsmoothUtility.CanUnsmooth(c, Map))
            {
                return UnsmoothConstants.TranslationKeys.MessageMustDesignateUnsmoothableSurface.Translate();
            }

            return true;
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (!t.Spawned || t.Map != Map)
            {
                return false;
            }

            return CanDesignateCell(t.Position);
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            var map = Map;
            if (!DebugSettings.godMode)
            {
                if (map.designationManager.DesignationAt(c, UnsmoothDesignationDef) == null)
                {
                    map.designationManager.AddDesignation(new Designation(c, UnsmoothDesignationDef));
                }
                return;
            }

            UnsmoothUtility.TryUnsmooth(c, map);

            var designation = map.designationManager.DesignationAt(c, UnsmoothDesignationDef);
            if (designation != null)
            {
                map.designationManager.RemoveDesignation(designation);
            }
        }

        public override void DesignateThing(Thing t)
        {
            DesignateSingleCell(t.Position);
        }
    }
}
