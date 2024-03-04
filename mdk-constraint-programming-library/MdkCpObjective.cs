namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public abstract class MdkCpObjective<TConfig, TInput, TVariables>
    where TConfig : MdkCpConfiguration
    where TInput : class
    where TVariables : class
{
    /// <summary>Builds the objective method.</summary>
    public abstract void Build(CpModel cpModel, TConfig configuration, TInput input, TVariables cpVariables, double previousObjectiveValue);
}