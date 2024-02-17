namespace TestConsoleApp.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

/// <summary>The pupils do exactly one activity.</summary>
internal class PupilsDoExactlyOneActivityConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        foreach (CoPupil pupil in input.Pupils)
        {
            cpModel.AddExactlyOne(cpVariables
                .Where(slot => slot.Key.Pupil == pupil)
                .Select(slot => slot.Value));
        }
    }
}
