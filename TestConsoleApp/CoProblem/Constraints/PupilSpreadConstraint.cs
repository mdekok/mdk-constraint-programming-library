namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Pupils are spread over locations and activities.</summary>
internal sealed class PupilSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int spread = 2;

        // Calculate the number of location with maximum capacity and the total capacity of these locations.
        int maximizedLocationCount = 0;
        int maximizedLocationCapacity = 0;
        foreach (CoLocation location in input.Locations.Where(location => location.MaxCapacity != 0))
        {
            maximizedLocationCount++;
            maximizedLocationCapacity += location.MaxCapacity;
        }

        // Locations with a maximum capacity are assigned as much as possible.
        // The remaining needed capacity is distributed over the other locations.
        int pupilCountPerNonMaximizedLocationAvg = (input.PupilCount() - maximizedLocationCapacity) / (input.LocationCount() - maximizedLocationCount);
        Console.WriteLine($"Remaining average Pupils per location: {pupilCountPerNonMaximizedLocationAvg}");

        foreach (CoLocation location in input.Locations)
        {
            int activityGroupCount = location.ActivityGroups.Count;

            int lowerBound = location.MaxCapacity == 0
                ? (pupilCountPerNonMaximizedLocationAvg - spread) / activityGroupCount
                : location.MaxCapacity / activityGroupCount - 1;
            int upperBound = location.MaxCapacity == 0
                ? (pupilCountPerNonMaximizedLocationAvg + spread) / activityGroupCount
                : location.MaxCapacity / activityGroupCount;

            foreach (CoActivityGroup activityGroup in location.ActivityGroups)
            {
                LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

                foreach (CoBuddyGroup buddyGroup in input.PlannableBuddyGroups())
                {
                    cpVariables[(buddyGroup, activityGroup)].ForEach(boolVar => linearExprBuilder.AddTerm(boolVar, buddyGroup.Pupils.Count));
                }

                // Take into account the capacity needed to pupils (with buddy group) that are preassigned to any of the activities of the activity group.
                int alreadyAssignedPupilCount = 0;
                foreach (CoActivity activity in activityGroup.Activities)
                {
                    alreadyAssignedPupilCount += input.MustDoBuddyGroups(activity).Sum(buddyGroup => buddyGroup.Pupils.Count);
                }

                cpModel.AddLinearConstraint(linearExprBuilder, lowerBound - alreadyAssignedPupilCount, upperBound - alreadyAssignedPupilCount);
            }
        }
    }
}
