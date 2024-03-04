namespace MdkConstraintProgrammingLibrary;

/// <summary>Values for LP relaxation. See: https://en.wikipedia.org/wiki/Linear_programming_relaxation</summary>
public static class LinearizationLevelConsts
{
    public static readonly int NoLP = 0;
    public static readonly int DefaultLP = 1;
    public static readonly int MaxLP = 2;
}