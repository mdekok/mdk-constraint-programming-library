using Google.OrTools.Sat;

namespace MdkConstraintProgrammingLibrary;

public class MdkCpResults
{
    public CpSolver Solver { get; init; }
    public CpSolverStatus SolverStatus { get; init; }

    public double ObjectiveValue => this.Solver.ObjectiveValue;
    public long NumConflicts => this.Solver.NumConflicts();
    public long NumBranches => this.Solver.NumBranches();
    public double WallTime => this.Solver.WallTime();
}
