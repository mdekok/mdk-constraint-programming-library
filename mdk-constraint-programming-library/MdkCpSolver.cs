namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public class MdkCpSolver<TInput, TVariables, TResults>
    where TInput : class
    where TVariables : class
    where TResults : class, new()
{
    private readonly CpModel googleCpModel = new();
    private readonly CpSolver googleCpSolver = new();

    public MdkCpSolver(TInput Input)
        => this.Input = Input;

    public TInput Input { get; }

    /// <summary>Gets or sets the max time in seconds the solver is allowed to take to come to a solution.</summary>
    protected int TimeLimitInSeconds { get; set; } = 10;

    #region InputValidator

    public MdkCpSolver<TInput, TVariables, TResults> SetInputValidator<TInputValidator>()
        where TInputValidator : class, IMdkCpInputValidator<TInput>, new()
    {
        this.inputValidator = new TInputValidator();
        return this;
    }

    private IMdkCpInputValidator<TInput>? inputValidator = null;

    #endregion

    #region Variables

    public MdkCpSolver<TInput, TVariables, TResults> SetVariablesBuilder<TVariablesBuilder>()
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

    public MdkCpSolver<TInput, TVariables, TResults> AddConstraint<TConstraint>()
        where TConstraint : MdkCpConstraint<TInput, TVariables>, new()
    {
        this.constraints.Add(new TConstraint());
        return this;
    }

    private List<MdkCpConstraint<TInput, TVariables>> constraints = new();

    #endregion

    #region Objectives

    public MdkCpSolver<TInput, TVariables, TResults> AddObjective<TObjective>()
        where TObjective : MdkCpObjective<TInput, TVariables>, new()
    {
        this.objectives.Add(new TObjective());
        return this;
    }

    private List<MdkCpObjective<TInput, TVariables>> objectives = new();

    #endregion

    #region Results

    public MdkCpSolver<TInput, TVariables, TResults> SetResultsBuilder<TResultsBuilder>()
        where TResultsBuilder : MdkCpResultsBuilder<TInput, TVariables, TResults>, new()
    {
        this.resultsBuilder = new TResultsBuilder();
        return this;
    }

    private MdkCpResultsBuilder<TInput, TVariables, TResults>? resultsBuilder;

    #endregion

    public TResults Solve(SolutionCallback? solutionCallback = null)
    {
        this.googleCpSolver.StringParameters = $"max_time_in_seconds:{this.TimeLimitInSeconds} ";
        this.googleCpSolver.StringParameters += "enumerate_all_solutions:true ";
        this.googleCpSolver.StringParameters += "linearization_level:2 ";

        this.inputValidator?.Validate(this.Input);

        if (this.variablesBuilder is null)
            throw new ArgumentException("Variables builder not set.");
        this.Variables = this.variablesBuilder.Build(this.googleCpModel, this.Input);

        this.constraints.ForEach(constraint => constraint.Register(this.googleCpModel, this.Input, this.Variables));

        CpSolverStatus solverStatus = CpSolverStatus.Unknown;
        if (this.objectives.Count != 0)
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

        if (this.resultsBuilder is null)
            throw new ArgumentException("Results builder not set.");

        return this.resultsBuilder.Build(this.Input, this.Variables, this.googleCpSolver, solverStatus);
    }

    public long Value(IntVar intVar) => this.googleCpSolver.Value(intVar);
    public double ObjectiveValue => this.googleCpSolver.ObjectiveValue;
    public long NumConflicts => this.googleCpSolver.NumConflicts();
    public long NumBranches => this.googleCpSolver.NumBranches();
    public double WallTime => this.googleCpSolver.WallTime();
}