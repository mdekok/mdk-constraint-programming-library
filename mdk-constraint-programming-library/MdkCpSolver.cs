namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public class MdkCpSolver<TInput, TVariables>
    where TInput : class
    where TVariables : class
{
    private CpModel googleCpModel = new();
    private CpSolver googleCpSolver = new();

    public required TInput Input { get; set; }

    /// <summary>Gets or sets the max time in seconds the solver is allowed to take to come to a solution.</summary>
    public int TimeLimitInSeconds { get; set; } = 10;

    #region InputValidator

    public MdkCpSolver<TInput, TVariables> SetInputValidator<TInputValidator>()
        where TInputValidator : class, IMdkCpInputValidator<TInput>, new()
    {
        this.inputValidator = new TInputValidator();
        return this;
    }

    private IMdkCpInputValidator<TInput>? inputValidator = null;

    #endregion

    #region Variables

    public MdkCpSolver<TInput, TVariables> SetVariablesBuilder<TVariablesBuilder>()
        where TVariablesBuilder : MdkCpVariablesBuilder<TInput, TVariables>, new()
    {
        this.variablesBuilder = new TVariablesBuilder();
        return this;
    }

    private MdkCpVariablesBuilder<TInput, TVariables>? variablesBuilder;

    public TVariables Variables
    {
        get
        {
            if (this._variables is null)
                throw new InvalidOperationException("Variables not set.");
            return this._variables;
        }
        set => this._variables = value;
    }
    private TVariables? _variables;
    // ToDo: this.cpModel.Model.Variables.Capacity = variables.Count; needed after setting variables?

    #endregion

    #region Constraints

    public MdkCpSolver<TInput, TVariables> AddConstraint<TConstraint>()
        where TConstraint : MdkCpConstraint<TInput, TVariables>, new()
    {
        this.constraints.Add(new TConstraint());
        return this;
    }

    private List<MdkCpConstraint<TInput, TVariables>> constraints = new();

    #endregion

    #region Objectives

    public MdkCpSolver<TInput, TVariables> AddObjective<TObjective>()
        where TObjective : MdkCpObjective<TInput, TVariables>, new()
    {
        this.objectives.Add(new TObjective());
        return this;
    }

    private List<MdkCpObjective<TInput, TVariables>> objectives = new();

    #endregion

    public CpSolverStatus Solve(SolutionCallback? solutionCallback = null)
    {
        this.googleCpSolver.StringParameters = $"max_time_in_seconds:{this.TimeLimitInSeconds}";

        this.inputValidator?.Validate(this.Input);

        if (this.variablesBuilder is null)
            throw new ArgumentException("Variables builder not set.");
        this.Variables = this.variablesBuilder.Build(this.googleCpModel, this.Input);

        this.constraints.ForEach(constraint => constraint.Register(this.googleCpModel, this.Input, this.Variables));

        CpSolverStatus solverStatus = CpSolverStatus.Unknown;
        if (this.objectives.Any())
        {
            foreach (MdkCpObjective<TInput, TVariables> objective in this.objectives)
            {
                double previousObjectiveValue = 0;
                if (objective != this.objectives.First())
                {
                    if (solverStatus != CpSolverStatus.Feasible && solverStatus != CpSolverStatus.Optimal)
                        break;
                    previousObjectiveValue = this.googleCpSolver.ObjectiveValue;
                }

                objective.Build(this.googleCpModel, this.Input, this.Variables, previousObjectiveValue);
                solverStatus = this.googleCpSolver.Solve(this.googleCpModel, solutionCallback);
            }
        }
        else
        {
            solverStatus = this.googleCpSolver.Solve(this.googleCpModel, solutionCallback);
        }

        return solverStatus;
    }

    public long Value(IntVar intVar) => this.googleCpSolver.Value(intVar);
    public double ObjectiveValue => this.googleCpSolver.ObjectiveValue;
    public long NumConflicts => this.googleCpSolver.NumConflicts();
    public long NumBranches => this.googleCpSolver.NumBranches();
    public double WallTime => this.googleCpSolver.WallTime();
}