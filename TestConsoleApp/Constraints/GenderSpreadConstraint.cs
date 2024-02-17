namespace TestConsoleApp.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

/// <summary>Pupils are spread by gender.</summary>
internal class GenderSpreadConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        int femaleSurplusPerActivity = (input.FemaleCount() - input.MaleCount()) / input.ActivityCount();
        int lowerBound = femaleSurplusPerActivity - 2;
        int upperBound = femaleSurplusPerActivity + 2;

        foreach (CoActivity activity in input.Activities)
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();
            foreach (CoPupil pupil in input.Pupils)
            {
                int coefficient = pupil.Gender == Gender.Female ? 1 : -1;
                for (int i = 0; i < activity.MaxCapacity; i++)
                {
                    linearExprBuilder.AddTerm(cpVariables[(pupil, activity, i)], coefficient);
                }
            }

            cpModel.AddLinearConstraint(linearExprBuilder, lowerBound, upperBound);
        }
    }
}
