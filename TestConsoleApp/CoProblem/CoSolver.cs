using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal sealed class CoSolver : MdkCpSolver<CoInput, CoVariables>
{
    public CoSolver(CoInput input)
        : base(input)
    {
        this.TimeLimitInSeconds = input.Configuration.TimeLimitInSeconds;
    }

    public bool IsSet(CoBuddyGroup buddyGroup, CoActivity activity)
        => this.Variables[(buddyGroup, activity)].Any(boolVar => this.Value(boolVar) == 1L);
}