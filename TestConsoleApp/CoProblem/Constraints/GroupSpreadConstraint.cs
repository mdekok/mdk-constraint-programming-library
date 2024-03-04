namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Pupils are spread by group.</summary>
internal sealed class GroupSpreadConstraint : MdkCpConstraint<CoConfiguration, CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoConfiguration configuration, CoInput input, CoVariables cpVariables)
    {
        int maxSpread = configuration.GroupSpread;

        foreach (IGrouping<int, CoPupil> grouping in input.PlannablePupils().GroupBy(pupil => pupil.GroupId))
        {
            int pupilCountInGroupAvg = grouping.Count() / input.LocationCount();
            int lowerBound = Math.Max(pupilCountInGroupAvg - maxSpread, 0);
            int upperBound = pupilCountInGroupAvg + maxSpread;

            foreach (CoLocation location in input.Locations)
            {
                LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

                foreach (CoActivityGroup activityGroup in location.ActivityGroups)
                    foreach (CoPupil pupil in grouping)
                    {
                        cpVariables[(pupil.BuddyGroup, activityGroup)].ForEach(boolVar => linearExprBuilder.Add(boolVar));
                    }

                // ToDo : Take into account the pupils that are preassigned like in GenderSpreadConstraint.
                // Does not seem to be very necessary seeing test results.

                cpModel.AddLinearConstraint(linearExprBuilder, lowerBound, upperBound);
            }
        }
    }
}
