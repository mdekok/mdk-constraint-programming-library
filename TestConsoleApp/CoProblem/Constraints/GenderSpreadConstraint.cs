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

        int femaleSurplusPerLocation = (input.FemaleCount() - input.MaleCount()) / input.LocationCount();
        int lowerBound = femaleSurplusPerLocation - spread;
        int upperBound = femaleSurplusPerLocation + spread;

        foreach (CoLocation location in input.Locations)
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

            foreach (CoActivityGroup activityGroup in location.ActivityGroups)
                foreach (CoPupil pupil in input.PlannableBuddyGroups().SelectMany(buddyGroup => buddyGroup.Pupils))
                {
                    int coefficient = pupil.Gender == Gender.Female ? 1 : -1;
                    cpVariables[(pupil.BuddyGroup, activityGroup)].ForEach(boolVar => linearExprBuilder.AddTerm(boolVar, coefficient));
                }

            // Take into account the pupils that are preassigned to this location.
            int mustDoConstant = input
                .MustDoBuddyGroups(location)
                .Sum(buddyGroup => buddyGroup.Pupils.Sum(pupil => pupil.Gender == Gender.Female ? 1 : -1));
            if (mustDoConstant != 0)
            {
                linearExprBuilder.Add(mustDoConstant);
            }

            cpModel.AddLinearConstraint(linearExprBuilder, lowerBound, upperBound);
        }
    }
}
