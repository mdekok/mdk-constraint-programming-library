namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public abstract class MdkCpVariablesBuilder<TInput, TVariables>
    where TInput : class
    where TVariables : class
{
    public abstract TVariables Build(CpModel cpModel, TInput input);
}
