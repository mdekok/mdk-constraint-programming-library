namespace TestConsoleApp.CoProblem;

/// <summary>CoInput extension methods.</summary>
internal static class CoInputExtensions
{
    /// <summary>Total number of pupils.</summary>
    public static int PupilCount(this CoInput input)
        => input.Pupils.Count;

    /// <summary>Total number of male pupils.</summary>
    public static int MaleCount(this CoInput input)
        => input.Pupils.Count(pupil => pupil.Gender == Gender.Male);

    /// <summary>Total number of female pupils.</summary>
    public static int FemaleCount(this CoInput input)
        => input.Pupils.Count(pupil => pupil.Gender == Gender.Female);

    /// <summary>Total number of pupils needing extra attention.</summary>
    public static int NeedingAttentionCount(this CoInput input)
        => input.Pupils.Count(pupil => pupil.NeedsAttention);

    /// <summary>Total number of activities.</summary>
    public static int ActivityCount(this CoInput input)
        => input.Activities.Count;

    /// <summary>Total number of buddy groups.</summary>
    public static int BuddyGroupCount(this CoInput input)
        => input.BuddyGroups.Count;

    /// <summary>Total capacity needed for activity due to pupils mandatory doing this activity.</summary>
    public static int DoOrDontCapacityNeeded(this CoInput input, CoActivity activity)
        => input
            .DoOrDonts
            .Where(doOrDont => doOrDont.Activity == activity && doOrDont.MustDo)
            .Select(CoDoOrDont => CoDoOrDont.Pupil.BuddyGroup)
            .Distinct()
            .Sum(buddyGroup => buddyGroup.Pupils.Count);
}
