using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal sealed class CoSolver : MdkCpSolver<CoConfiguration, CoInput, CoVariables, CoResults>
{
    public CoSolver(CoConfiguration configuration, CoInput input)
        : base(configuration, input)
    { }
}