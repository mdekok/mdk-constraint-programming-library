namespace MdkConstraintProgrammingLibrary;

using Google.OrTools.Sat;

public abstract class MdkCpConstraint<TInput, TVariables>
    where TInput : class
    where TVariables : class
{
    public abstract void Register(CpModel cpModel, TInput input, TVariables cpVariables);
}
