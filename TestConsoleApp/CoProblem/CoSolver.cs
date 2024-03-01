using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal sealed class CoSolver : MdkCpSolver<CoInput, CoVariables, CoResults>
{
    public CoSolver(CoInput input)
        : base(input)
    {
        this.TimeLimitInSeconds = input.Configuration.TimeLimitInSeconds;
    }
}