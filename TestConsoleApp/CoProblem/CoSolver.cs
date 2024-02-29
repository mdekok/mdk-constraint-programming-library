using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal sealed class CoSolver : MdkCpSolver<CoInput, CoVariables, CoResults>
{
    public CoSolver(CoInput input)
        : base(input)
    {
        this.TimeLimitInSeconds = input.Configuration.TimeLimitInSeconds;
    }

    public bool IsSet(CoBuddyGroup buddyGroup, CoActivityGroup activityGroup)
        => this.Variables[(buddyGroup, activityGroup)].Any(boolVar => this.Value(boolVar) == 1L);
}