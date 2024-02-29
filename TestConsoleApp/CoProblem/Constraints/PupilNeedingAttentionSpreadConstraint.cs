namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Pupils who need attention are spread over activities.</summary>
internal sealed class PupilNeedingAttentionSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int pupilNeedingAttentionPerLocationAvg = input.NeedingAttentionCount() / input.Locations.Count;

        foreach (CoLocation location in input.Locations)
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

            foreach (CoActivityGroup activityGroup in location.ActivityGroups)
                foreach (CoBuddyGroup buddyGroup in input
                    .PlannableBuddyGroups()
                    .Where(buddyGroup => buddyGroup
                        .Pupils
                        .Any(pupil => pupil.NeedsAttention)))
                {
                    cpVariables[(buddyGroup, activityGroup)].ForEach(boolVar
                        => linearExprBuilder.AddTerm(boolVar, buddyGroup.Pupils.Count(pupil => pupil.NeedsAttention)));
                }

            // Take into account the pupils that are preassigned to this location.
            int mustDoConstant = input
                .MustDoBuddyGroups(location)
                .Sum(buddyGroup => buddyGroup.Pupils.Count(pupil => pupil.NeedsAttention));
            if (mustDoConstant != 0)
            {
                linearExprBuilder.Add(mustDoConstant);
            }

            cpModel.AddLinearConstraint(linearExprBuilder, pupilNeedingAttentionPerLocationAvg, pupilNeedingAttentionPerLocationAvg + 1);
        }
    }
}
