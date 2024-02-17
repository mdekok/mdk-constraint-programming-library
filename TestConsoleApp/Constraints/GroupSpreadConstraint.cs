namespace TestConsoleApp.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

/// <summary>Pupils are spread by group.</summary>
internal class GroupSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        foreach (IGrouping<int, CoPupil> grouping in input.Pupils.GroupBy(group => group.GroupId))
        {
            int upperbound = (grouping.Count() / input.ActivityCount()) + 2;

            foreach (CoActivity activity in input.Activities)
            {
                LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();
                foreach (CoPupil pupil in grouping)
                {
                    for (int i = 0; i < activity.MaxCapacity; i++)
                    {
                        linearExprBuilder.Add(cpVariables[(pupil, activity, i)]);
                    }
                }

                cpModel.AddLinearConstraint(linearExprBuilder, 0, upperbound);
            }
        }
    }
}
