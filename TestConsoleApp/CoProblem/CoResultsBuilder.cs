using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal class CoResultsBuilder : MdkCpResultsBuilder<CoInput, CoVariables, CoResults>
{
    public override CoResults Build(CoInput input, CoVariables variables, CpSolver solver, CpSolverStatus solverStatus)
    {
        CoResults results = base.Build(input, variables, solver, solverStatus);

        if (solverStatus == CpSolverStatus.Feasible || solverStatus == CpSolverStatus.Optimal)
        {
            foreach (CoDoOrDont doOrDont in input.DoOrDonts.Where(doOrDont => doOrDont.MustDo))
            {
                // Assign all activities of the activity group which contains the activity to all pupils of the buddy group.
                CoActivityGroup activityGroup = input
                    .Locations
                    .SelectMany(location => location.ActivityGroups)
                    .First(activityGroup => activityGroup.Activities.Contains(doOrDont.Activity));
                foreach (CoActivity activity in activityGroup.Activities)
                {
                    doOrDont.Pupil.BuddyGroup.Pupils.ForEach(pupil => results.Values.Add(new(pupil, activity)));
                }
            };

            foreach (CoBuddyGroup buddyGroup in input.PlannableBuddyGroups())
                foreach (CoLocation location in input.Locations)
                    foreach (CoActivityGroup activityGroup in location.ActivityGroups)
                    {
                        if (variables[(buddyGroup, activityGroup)].Any(boolVar => solver.Value(boolVar) == 1L))
                        {
                            foreach (CoActivity activity in activityGroup.Activities)
                            {
                                buddyGroup.Pupils.ForEach(pupil => results.Values.Add(new(pupil, activity)));
                            }
                        }
                    }
        }

        return results;
    }
}
