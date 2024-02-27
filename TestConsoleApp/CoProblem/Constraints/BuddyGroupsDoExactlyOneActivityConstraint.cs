namespace TestConsoleApp.CoProblem.Constraints;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

/// <summary>The buddy groups do exactly one activity.</summary>
internal sealed class BuddyGroupsDoExactlyOneActivityConstraint : MdkCpConstraint<CoInput, CoVariables>
{
    public override void Register(CpModel cpModel, CoInput input, CoVariables cpVariables) =>
        input.BuddyGroups.ForEach(buddyGroup =>
        {
            cpModel.AddExactlyOne(cpVariables
                .Where(ba => ba.Key.BuddyGroup == buddyGroup)
                .SelectMany(ba => ba.Value));
        });
}
