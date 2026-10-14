using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace LambdaTenEleven.UnsmoothSurface;

public class JobDriver_UnsmoothSurface : JobDriver
{
    private const float XpPerTick = 0.07f;

    private static DesignationDef UnsmoothDesignationDef => DefDatabase<DesignationDef>.GetNamed(UnsmoothConstants.DefNames.UnsmoothDesignation);

    private static SoundDef PickHitSoundDef => DefDatabase<SoundDef>.GetNamed(UnsmoothConstants.DefNames.PickHitSound);

    private float _workLeft;
    private float _workRequired;

    private IntVec3 TargetCell => job.GetTarget(TargetIndex.A).Cell;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(TargetCell, job, 1, -1, null, errorOnFailed);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _workLeft, UnsmoothConstants.ScribeKeys.WorkLeft);
        Scribe_Values.Look(ref _workRequired, UnsmoothConstants.ScribeKeys.WorkRequired, UnsmoothSurfaceSettings.DefaultFloorWorkRequired);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOn(() => !UnsmoothUtility.CanUnsmooth(TargetCell, Map));
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
        yield return MakeUnsmoothToil();
        yield return MakeFinishToil();
    }

    private Toil MakeUnsmoothToil()
    {
        var toil = ToilMaker.MakeToil(UnsmoothConstants.ToilNames.UnsmoothSurface);
        toil.initAction = delegate
        {
            _workRequired = UnsmoothSurfaceMod.Settings.WorkRequiredFor(TargetCell, Map);
            _workLeft = _workRequired;
        };

        toil.tickAction = delegate
        {
            _workLeft -= pawn.GetStatValue(StatDefOf.MiningSpeed);
            pawn.skills?.Learn(SkillDefOf.Mining, XpPerTick);

            if (_workLeft <= 0f)
            {
                toil.actor.jobs.curDriver.ReadyForNextToil();
            }
        };

        toil.defaultCompleteMode = ToilCompleteMode.Never;
        toil.activeSkill = () => SkillDefOf.Mining;
        toil.WithProgressBar(TargetIndex.A, () => 1f - _workLeft / _workRequired);
        toil.WithEffect(EffecterDefOf.Mine, TargetIndex.A);
        toil.PlaySustainerOrSound(() => PickHitSoundDef);
        return toil;
    }

    private Toil MakeFinishToil()
    {
        var toil = ToilMaker.MakeToil(UnsmoothConstants.ToilNames.FinishUnsmoothSurface);
        toil.initAction = delegate
        {
            UnsmoothUtility.TryUnsmooth(TargetCell, Map);

            var designation = Map.designationManager.DesignationAt(TargetCell, UnsmoothDesignationDef);
            if (designation != null)
            {
                Map.designationManager.RemoveDesignation(designation);
            }
        };

        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return toil;
    }
}
