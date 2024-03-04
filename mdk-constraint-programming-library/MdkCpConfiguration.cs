namespace MdkConstraintProgrammingLibrary;

public class MdkCpConfiguration
{
    /// <summary>Gets or sets the max time in seconds the solver is allowed to take to come to a solution.</summary>
    public int TimeLimitInSeconds { get; set; } = 120;

    /// <summary>Gets or sets a value indicating whether enumerate all solutions. Settings this to true takes more time but leads to better solutions.</summary>
    public bool EnumerateAllSolutions { get; set; } = true;

    /// <summary>
    /// Gets or sets the linearization level (0 = no_lp, 1 = default_lp, 2 = max_lp). A non-negative level indicating the type of constraints we consider in the
    /// LP relaxation. At level zero, no LP relaxation is used. At level 1, only the linear constraint and full encoding are added. At level 2, we also add all the Boolean constraints.
    /// See: https://en.wikipedia.org/wiki/Linear_programming_relaxation
    /// </summary>
    public int LinearizationLevel { get; set; } = LinearizationLevelConsts.MaxLP;
}
