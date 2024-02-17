namespace MdkConstraintProgrammingLibrary;

public interface IMdkCpInputValidator<TInput>
    where TInput : class
{
    void Validate(TInput input);
}