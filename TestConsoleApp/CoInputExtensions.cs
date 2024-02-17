namespace TestConsoleApp;

internal static class CoInputExtensions
{
    public static int PupilCount(this CoInput input)
        => input.Pupils.Count();

    public static int MaleCount(this CoInput input)
        => input.Pupils.Count(pupil => pupil.Gender == Gender.Male);

    public static int FemaleCount(this CoInput input)
        => input.Pupils.Count(pupil => pupil.Gender == Gender.Female);

    public static int ActivityCount(this CoInput input)
        => input.Activities.Count;

    public static int BuddyGroupCount(this CoInput input)
        => input.BuddyGroups.Count;
}
