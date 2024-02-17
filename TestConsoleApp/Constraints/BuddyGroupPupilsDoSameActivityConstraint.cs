namespace TestConsoleApp.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

/// <summary>Pupils from same buddy group do same activity.</summary>
internal class BuddyGroupPupilsDoSameActivityConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
        {
            if (buddyGroup.Pupils.Count == 1)
                continue;

            Dictionary<(CoPupil, CoActivity),LinearExpr> linearExprs = [];
            foreach (CoPupil pupil in buddyGroup.Pupils)
                foreach (CoActivity activity in input.Activities)
                {
                    LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();
                    for (int i = 0; i < activity.MaxCapacity; i++)
                    {
                        linearExprBuilder.Add(cpVariables[(pupil, activity, i)]);
                    }
                    linearExprs[(pupil, activity)] = linearExprBuilder;
                }

            foreach (CoActivity activity in input.Activities)
            {
                LinearExpr? prevLinearExpr = null;
                foreach (CoPupil pupil in buddyGroup.Pupils)
                {
                    LinearExpr linearExpr = linearExprs[(pupil, activity)];
                    if (prevLinearExpr is not null)
                    {
                        BoundedLinearExpression boundedLinearExpression = new(prevLinearExpr, linearExpr, true);
                        cpModel.Add(boundedLinearExpression);
                    }
                    prevLinearExpr = linearExpr;
                }
            }
        }
    }
}
