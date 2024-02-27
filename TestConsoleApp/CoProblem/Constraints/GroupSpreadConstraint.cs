namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Pupils are spread by group.</summary>
internal sealed class GroupSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int spread = 3;

        foreach (IGrouping<int, CoPupil> grouping in input.Pupils.GroupBy(pupil => pupil.GroupId))
        {
            int pupilCountInGroup = grouping.Count();
            int upperbound = pupilCountInGroup / input.ActivityCount() + spread;

            foreach (CoActivity activity in input.Activities)
            {
                LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();
                foreach (CoPupil pupil in grouping)
                {
                    cpVariables[(pupil.BuddyGroup, activity)].ForEach(boolVar => linearExprBuilder.Add(boolVar));
                }

                cpModel.AddLinearConstraint(linearExprBuilder, 0, upperbound);
            }
        }
    }
}
