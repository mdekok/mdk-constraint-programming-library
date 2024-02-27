namespace TestConsoleApp.CoProblem;

internal sealed class CoResults : List<CoResult>
{
    public bool PupilDoesActivity(CoPupil pupil, CoActivity activity)
        => this.Any(result => result.Pupil == pupil && result.Activity == activity);
}

internal sealed record class CoResult(CoPupil Pupil, CoActivity Activity);