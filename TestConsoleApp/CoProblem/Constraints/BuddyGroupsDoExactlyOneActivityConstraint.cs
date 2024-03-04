namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>The buddy groups do exactly one activity.</summary>
internal sealed class BuddyGroupsDoExactlyOneActivityConstraint : MdkCpConstraint<CoConfiguration, CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoConfiguration configuration, CoInput input, CoVariables cpVariables)
    {
        foreach (CoBuddyGroup buddyGroup in input.PlannableBuddyGroups())
        {
            cpModel.AddExactlyOne(cpVariables
                .Where(ba => ba.Key.BuddyGroup == buddyGroup)
                .SelectMany(ba => ba.Value));
        }
    }
}
