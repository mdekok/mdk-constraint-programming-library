namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Pupils are spread over activities.</summary>
internal sealed class PupilSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int spread = 2;

        int pupilsPerActivityAvg = input.PupilCount() / input.ActivityCount();
        int capacityAssigned = 0;
        int activitiesRemaining = input.ActivityCount();

        // Assign maximum capacity if maximum capacity is set
        foreach (CoActivity activity in input.Activities.Where(activity => activity.MaxCapacity != 0))
        {
            int maxCapacity = activity.MaxCapacity - input.DoOrDontCapacityNeeded(activity);

            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

            foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
            {
                cpVariables[(buddyGroup, activity)].ForEach(boolVar => linearExprBuilder.AddTerm(boolVar, buddyGroup.Pupils.Count));
            }

            cpModel.AddLinearConstraint(linearExprBuilder, maxCapacity - 1, maxCapacity);

            capacityAssigned += activity.MaxCapacity - 1;
            activitiesRemaining--;
        }

        int pupilPerActivityRemainingAvg = (input.PupilCount() - capacityAssigned) / activitiesRemaining;
        Console.WriteLine($"Remaining average Pupils per activity: {pupilPerActivityRemainingAvg}");
        int lowerBound = pupilPerActivityRemainingAvg - spread;
        int upperBound = pupilPerActivityRemainingAvg + spread;

        // Assign bandwidth of +-spread capacity assignment around remaining average number of pupils per activity
        // if maximum capacity is not set
        foreach (CoActivity activity in input.Activities.Where(activity => activity.MaxCapacity == 0))
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

            foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
            {
                cpVariables[(buddyGroup, activity)].ForEach(boolVar => linearExprBuilder.AddTerm(boolVar, buddyGroup.Pupils.Count));
            }

            cpModel.AddLinearConstraint(linearExprBuilder, lowerBound, upperBound);
        }
    }
}
