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
            if (!DebugSettings.godMode && designation != null)
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
                    var map = parent.Map;
                    var position = parent.Position;
                    if (DebugSettings.godMode)
                    {
                        UnsmoothUtility.TryUnsmooth(position, map);

                        var existingDesignation = map.designationManager.DesignationAt(position, UnsmoothDesignationDef);
                        if (existingDesignation != null)
                        {
                            map.designationManager.RemoveDesignation(existingDesignation);
                        }
                    }
                    else if (map.designationManager.DesignationAt(position, UnsmoothDesignationDef) == null)
                    {
                        map.designationManager.AddDesignation(new Designation(position, UnsmoothDesignationDef));
                    }
                }
            };
        }
    }
}
