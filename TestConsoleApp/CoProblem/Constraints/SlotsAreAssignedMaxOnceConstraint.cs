namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>Slots are assigned max once.</summary>
internal class SlotsAreAssignedMaxOnceConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        foreach (CoActivity activity in input.Activities)
        {
            int slotCount = cpVariables
                .Where(ba => ba.Key.Activity == activity)
                .First()
                .Value.Count;

            for (int i = 0; i < slotCount; i++)
            {
                cpModel.AddAtMostOne(cpVariables
                    .Where(slot => slot.Key.Activity == activity)
                    .Select(slot => slot.Value[i]));
            }
        }
    }
}
