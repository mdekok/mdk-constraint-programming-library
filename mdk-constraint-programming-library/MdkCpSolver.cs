namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public class MdkCpSolver<TConfig, TInput, TVariables, TResults>
    where TConfig: MdkCpConfiguration
    where TInput : class
    where TVariables : class
    where TResults : MdkCpResults, new()
{
    private readonly CpModel googleCpModel = new();
    private readonly CpSolver googleCpSolver = new();

    public MdkCpSolver(TConfig configuration, TInput Input)
    { 
        this.Configuration = configuration;
        this.Input = Input;
    }

    public TConfig Configuration { get; }
    public TInput Input { get; }

    #region InputValidator

    public MdkCpSolver<TConfig, TInput, TVariables, TResults> SetInputValidator<TInputValidator>()
        where TInputValidator : class, IMdkCpInputValidator<TInput>, new()
    {
        this.inputValidator = new TInputValidator();
        return this;
    }

    private IMdkCpInputValidator<TInput>? inputValidator = null;

    #endregion

    #region Variables

    public MdkCpSolver<TConfig, TInput, TVariables, TResults> SetVariablesBuilder<TVariablesBuilder>()
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

    public MdkCpSolver<TConfig, TInput, TVariables, TResults> AddConstraint<TConstraint>()
        where TConstraint : MdkCpConstraint<TConfig, TInput, TVariables>, new()
    {
        this.constraints.Add(new TConstraint());
        return this;
    }

    private List<MdkCpConstraint<TConfig, TInput, TVariables>> constraints = [];

    #endregion

    #region Objectives

    public MdkCpSolver<TConfig, TInput, TVariables, TResults> AddObjective<TObjective>()
        where TObjective : MdkCpObjective<TConfig, TInput, TVariables>, new()
    {
        this.objectives.Add(new TObjective());
        return this;
    }

    private readonly List<MdkCpObjective<TConfig, TInput, TVariables>> objectives = [];

    #endregion

    #region Results

    public MdkCpSolver<TConfig, TInput, TVariables, TResults> SetResultsBuilder<TResultsBuilder>()
        where TResultsBuilder : MdkCpResultsBuilder<TInput, TVariables, TResults>, new()
    {
        this.resultsBuilder = new TResultsBuilder();
        return this;
    }

    private MdkCpResultsBuilder<TInput, TVariables, TResults>? resultsBuilder;

    #endregion

    public TResults Solve(SolutionCallback? solutionCallback = null)
    {
        this.googleCpSolver.StringParameters = $"max_time_in_seconds:{this.Configuration.TimeLimitInSeconds} ";
        this.googleCpSolver.StringParameters += $"enumerate_all_solutions:{this.Configuration.EnumerateAllSolutions.ToString().ToLower()} ";
        this.googleCpSolver.StringParameters += $"linearization_level:{this.Configuration.LinearizationLevel} ";

        this.inputValidator?.Validate(this.Input);

        if (this.variablesBuilder is null)
            throw new ArgumentException("Variables builder not set.");
        this.Variables = this.variablesBuilder.Build(this.googleCpModel, this.Input);

        this.constraints.ForEach(constraint => constraint.Register(this.googleCpModel, this.Configuration, this.Input, this.Variables));

        CpSolverStatus solverStatus = CpSolverStatus.Unknown;
        if (this.objectives.Count != 0)
        {
            foreach (MdkCpObjective<TConfig, TInput, TVariables> objective in this.objectives)
            {
                double previousObjectiveValue = 0;
                if (objective != this.objectives.First())
                {
                    if (solverStatus != CpSolverStatus.Feasible && solverStatus != CpSolverStatus.Optimal)
                        break;
                    previousObjectiveValue = this.googleCpSolver.ObjectiveValue;
                }

                objective.Build(this.googleCpModel, this.Configuration, this.Input, this.Variables, previousObjectiveValue);
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
}