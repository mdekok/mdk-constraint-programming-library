namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Pupils are spread by gender.</summary>
internal sealed class GenderSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int spread = 3;

        int femaleSurplusPerActivity = (input.FemaleCount() - input.MaleCount()) / input.ActivityCount();
        int lowerBound = femaleSurplusPerActivity - spread;
        int upperBound = femaleSurplusPerActivity + spread;

        foreach (CoActivity activity in input.Activities)
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();
            foreach (CoPupil pupil in input.Pupils)
            {
                int coefficient = pupil.Gender == Gender.Female ? 1 : -1;
                cpVariables[(pupil.BuddyGroup, activity)].ForEach(boolVar => linearExprBuilder.AddTerm(boolVar, coefficient));
            }

            cpModel.AddLinearConstraint(linearExprBuilder, lowerBound, upperBound);
        }
    }
}
