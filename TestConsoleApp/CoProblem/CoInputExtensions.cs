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

    /// <summary>Total number of locations.</summary>
    public static int LocationCount(this CoInput input)
        => input.Locations.Count;

    /// <summary>Total number of buddy groups.</summary>
    public static int BuddyGroupCount(this CoInput input)
        => input.BuddyGroups.Count;

    /// <summary>Buddy groups that must do an activity on a location due to pupils mandatory doing activities.</summary>
    public static IEnumerable<CoBuddyGroup> MustDoBuddyGroups(this CoInput input, CoLocation location)
        => input
            .DoOrDonts
            .Where(doOrDont => location.ActivityGroups.SelectMany(activityGroup => activityGroup.Activities).Contains(doOrDont.Activity) && doOrDont.MustDo)
            .Select(CoDoOrDont => CoDoOrDont.Pupil.BuddyGroup)
            .Distinct();

    public static IEnumerable<CoBuddyGroup> MustDoBuddyGroups(this CoInput input, CoActivity activity)
        => input
            .DoOrDonts
            .Where(doOrDont => doOrDont.Activity == activity && doOrDont.MustDo)
            .Select(CoDoOrDont => CoDoOrDont.Pupil.BuddyGroup)
            .Distinct();

    public static List<CoBuddyGroup> PreAssignedBuddyGroups(this CoInput input)
        => input.DoOrDonts
            .Where(doOrDont => doOrDont.MustDo)
            .Select(doOrDont => doOrDont.Pupil.BuddyGroup)
            .ToList();

    public static List<CoBuddyGroup> PlannableBuddyGroups(this CoInput input)
        => input
           .BuddyGroups
           .Where(buddyGroup => !input.PreAssignedBuddyGroups().Contains(buddyGroup))
           .ToList();

    public static List<CoPupil> PlannablePupils(this CoInput input)
    {
        List<CoPupil> pupilsPreAssigned = input
            .PreAssignedBuddyGroups()
            .SelectMany(buddyGroup => buddyGroup.Pupils)
            .ToList();

        return input
           .Pupils
           .Where(pupil => !pupilsPreAssigned.Contains(pupil))
           .ToList();
    }
}
