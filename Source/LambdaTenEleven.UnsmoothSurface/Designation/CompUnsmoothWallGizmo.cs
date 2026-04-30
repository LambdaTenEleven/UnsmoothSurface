using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace LambdaTenEleven.UnsmoothSurface
{
    public class CompUnsmoothWallGizmo : ThingComp
    {
        private static DesignationDef UnsmoothDesignationDef => DefDatabase<DesignationDef>.GetNamed(UnsmoothConstants.DefNames.UnsmoothDesignation);

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!parent.Spawned || !UnsmoothUtility.CanUnsmoothWall(parent))
            {
                yield break;
            }

            var designation = parent.Map.designationManager.DesignationAt(parent.Position, UnsmoothDesignationDef);
            if (designation != null)
            {
                var cancelDesignator = new Designator_Cancel();
                yield return new Command_Action
                {
                    defaultLabel = UnsmoothConstants.TranslationKeys.DesignatorCancel.Translate(),
                    defaultDesc = UnsmoothConstants.TranslationKeys.DesignatorCancelDesc.Translate(),
                    icon = ContentFinder<Texture2D>.Get(UnsmoothConstants.TexturePaths.CancelDesignator),
                    hotKey = KeyBindingDefOf.Designator_Cancel,
                    tutorTag = UnsmoothConstants.TutorTags.Cancel,
                    activateSound = SoundDefOf.Designate_Cancel,
                    action = delegate
                    {
                        cancelDesignator.DesignateSingleCell(parent.Position);
                    }
                };
                yield break;
            }

            yield return new Command_Action
            {
                defaultLabel = UnsmoothConstants.TranslationKeys.DesignatorUnsmoothSurface.Translate(),
                defaultDesc = UnsmoothConstants.TranslationKeys.DesignatorUnsmoothWallDesc.Translate(),
                icon = ContentFinder<Texture2D>.Get(UnsmoothConstants.TexturePaths.UnsmoothDesignator),
                action = delegate
                {
                    if (parent.Map.designationManager.DesignationAt(parent.Position, UnsmoothDesignationDef) == null)
                    {
                        parent.Map.designationManager.AddDesignation(new Designation(parent.Position, UnsmoothDesignationDef));
                    }
                }
            };
        }
    }
}
