namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Pupils who need attention are spread over activities.</summary>
internal sealed class PupilNeedingAttentionSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int pupilNeedingAttentionPerActivityAvg = input.NeedingAttentionCount() / input.Activities.Count;

        // Assign maximum capacity if maximum capacity is less than average number of pupils per activity
        foreach (CoActivity activity in input.Activities)
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

            foreach (CoBuddyGroup buddyGroup in input
                .BuddyGroups
                .Where(buddyGroup => buddyGroup
                    .Pupils
                    .Any(pupil => pupil.NeedsAttention)))
            {
                cpVariables[(buddyGroup, activity)].ForEach(boolVar
                    => linearExprBuilder.AddTerm(boolVar, buddyGroup.Pupils.Count(pupil => pupil.NeedsAttention)));
            }

            cpModel.AddLinearConstraint(linearExprBuilder, pupilNeedingAttentionPerActivityAvg, pupilNeedingAttentionPerActivityAvg + 1);
        }
    }
}
