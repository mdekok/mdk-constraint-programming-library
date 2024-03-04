namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public abstract class MdkCpConstraint<TConfig, TInput, TVariables>
    where TConfig: MdkCpConfiguration
    where TInput : class
    where TVariables : class
{
    public abstract void Register(CpModel cpModel, TConfig configuration, TInput input, TVariables cpVariables);
}
