using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal sealed class CoResults : MdkCpResults
{
    public List<CoResult> Values { get; set; } = [];

    public bool PupilDoesActivity(CoPupil pupil, CoActivity activity)
        => this.Values.Any(result => result.Pupil == pupil && result.Activity == activity);
}

internal sealed record class CoResult(CoPupil Pupil, CoActivity Activity);