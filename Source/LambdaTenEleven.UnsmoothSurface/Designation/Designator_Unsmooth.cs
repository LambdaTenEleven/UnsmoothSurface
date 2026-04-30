using RimWorld;
using UnityEngine;
using Verse;

namespace LambdaTenEleven.UnsmoothSurface
{
    public class Designator_Unsmooth : Designator_Cells
    {
        public override DrawStyleCategoryDef DrawStyleCategory => DefDatabase<DrawStyleCategoryDef>.GetNamed(UnsmoothConstants.DefNames.DrawStyleDefault2D);

        private static DesignationDef UnsmoothDesignationDef => DefDatabase<DesignationDef>.GetNamed(UnsmoothConstants.DefNames.UnsmoothDesignation);

        public override bool DragDrawMeasurements => true;

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }

        public Designator_Unsmooth()
        {
            defaultLabel = UnsmoothConstants.Labels.UnsmoothSurface;
            defaultDesc = UnsmoothConstants.Descriptions.UnsmoothDesignator;
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

            if (Map.designationManager.DesignationAt(c, UnsmoothDesignationDef) != null)
            {
                return UnsmoothConstants.Messages.AlreadyDesignated;
            }

            if (!UnsmoothUtility.CanUnsmooth(c, Map))
            {
                return UnsmoothConstants.Messages.MustDesignateSmoothedSurface;
            }

            return true;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            Map.designationManager.AddDesignation(new Designation(c, UnsmoothDesignationDef));
        }
    }
}
