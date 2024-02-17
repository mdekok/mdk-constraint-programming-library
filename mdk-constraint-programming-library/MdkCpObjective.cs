namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public abstract class MdkCpObjective<TInput, TVariables>
    where TInput : class
    where TVariables : class
{
    public abstract void Build(CpModel cpModel, TInput input, TVariables cpVariables, double previousObjectiveValue);
}
