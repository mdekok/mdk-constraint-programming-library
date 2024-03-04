using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal sealed class CoConfiguration : MdkCpConfiguration
{
    /// <summary>
    /// Gets or sets the max history gap. Pupils that did not do an activity before get this value as history gap for that activity.
    /// History gaps are also maximized to this value.
    /// </summary>
    public int MaxHistoryGap { get; set; } = 10;

    /// <summary>Gets or sets the pupil spread: max deviation from average pupil count per location.</summary>
    public int PupilSpread { get; set; } = 2;

    /// <summary>Gets or sets the gender spread: max deviation from average female count per location.</summary>
    public int GenderSpread { get; set; } = 3;

    /// <summary>Gets or sets the group spread: max deviation from average group count per location.</summary>
    public int GroupSpread { get; set; } = 3;
}
