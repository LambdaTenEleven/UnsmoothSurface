using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace LambdaTenEleven.UnsmoothSurface
{
    public class WorkGiver_UnsmoothSurface : WorkGiver_Scanner
    {
        private static DesignationDef UnsmoothDesignationDef => DefDatabase<DesignationDef>.GetNamed(UnsmoothConstants.DefNames.UnsmoothDesignation);

        private static JobDef UnsmoothJobDef => DefDatabase<JobDef>.GetNamed(UnsmoothConstants.DefNames.UnsmoothJob);

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(UnsmoothDesignationDef);
        }

        public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            foreach (var designation in pawn.Map.designationManager.SpawnedDesignationsOfDef(UnsmoothDesignationDef))
            {
                yield return designation.target.Cell;
            }
        }

        public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            if (pawn.Map.designationManager.DesignationAt(c, UnsmoothDesignationDef) == null)
            {
                return false;
            }

            if (!UnsmoothUtility.CanUnsmooth(c, pawn.Map))
            {
                return false;
            }

            return pawn.CanReach(c, PathEndMode.Touch, Danger.Deadly) &&
                   pawn.CanReserve(c, 1, -1, null, forced);
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            return JobMaker.MakeJob(UnsmoothJobDef, c);
        }
    }
}
