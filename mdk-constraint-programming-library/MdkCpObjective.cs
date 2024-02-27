namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public abstract class MdkCpObjective<TInput, TVariables>
    where TInput : class
    where TVariables : class
{
    /// <summary>Builds the objective method.</summary>
    public abstract void Build(CpModel cpModel, TInput input, TVariables cpVariables, double previousObjectiveValue);
}