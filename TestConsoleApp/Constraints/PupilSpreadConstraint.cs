namespace TestConsoleApp.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

/// <summary>Pupils are spread over activities.</summary>
internal class PupilSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int pupilPerActivityAvg = input.PupilCount() / input.Activities.Count;
        int capacityAssigned = 0;
        int activitiesRemaining = input.Activities.Count;

        // Assign maximum capacity if maximum capacity is less than average number of pupils per activity
        foreach (CoActivity activity in input.Activities.Where(activity => activity.MaxCapacity < pupilPerActivityAvg))
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

            for (int i = 0; i < activity.MaxCapacity; i++)
            {
                input.Pupils.ForEach(pupil => linearExprBuilder.Add(cpVariables[(pupil, activity, i)]));
            }
            BoundedLinearExpression boundedLinearExpression = new(linearExprBuilder, activity.MaxCapacity, true);
            cpModel.Add(boundedLinearExpression);
            //// ToDo : Upper bound cannot be set to activity.MaxCapacity, why not? Adds lb ≤ expr ≤ ub.
            //cpModel.AddLinearConstraint(linearExprBuilder, activity.MaxCapacity, activity.MaxCapacity + 1);

            capacityAssigned += activity.MaxCapacity;
            activitiesRemaining--;
        }

        int pupilPerActivityRemainingAvg = (input.PupilCount() - capacityAssigned) / activitiesRemaining;
        int lowerBound = pupilPerActivityRemainingAvg - 2;

        // Assign bandwidth of +-2 capacity assignment around remaining average number of pupils per activity
        // if maximum capacity is more than average number of pupils per activity
        foreach (CoActivity activity in input.Activities.Where(activity => activity.MaxCapacity >= pupilPerActivityAvg))
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();
            for (int i = 0; i < activity.MaxCapacity; i++)
            {
                input.Pupils.ForEach(pupil => linearExprBuilder.Add(cpVariables[(pupil, activity, i)]));
            }
            int upperBound = Math.Min(pupilPerActivityRemainingAvg + 2, activity.MaxCapacity);

            cpModel.AddLinearConstraint(linearExprBuilder, lowerBound, upperBound);
        }
    }
}
