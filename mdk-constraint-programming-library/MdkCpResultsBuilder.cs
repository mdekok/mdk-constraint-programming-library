using Google.OrTools.Sat;

namespace MdkConstraintProgrammingLibrary;

public class MdkCpResultsBuilder<TInput, TVariables, TResults>
    where TInput : class
    where TVariables : class
    where TResults : MdkCpResults, new()
{
    public virtual TResults Build(TInput input, TVariables variables, CpSolver cpSolver, CpSolverStatus cpSolverStatus)
    {
        TResults results = new()
        {
            Solver = cpSolver,
            SolverStatus = cpSolverStatus
        };

        return results;
    }
}
