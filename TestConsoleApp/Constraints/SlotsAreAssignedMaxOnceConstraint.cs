namespace TestConsoleApp.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

/// <summary>Slots are assigned max once.</summary>
internal class SlotsAreAssignedMaxOnceConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables)
    {
        foreach (CoActivity activity in input.Activities)
            for (int i = 0; i < activity.MaxCapacity; i++)
        {
            cpModel.AddAtMostOne(cpVariables
                .Where(slot => slot.Key.Activity == activity && slot.Key.SlotIndex == i)
                .Select(slot => slot.Value));
        }
    }
}
