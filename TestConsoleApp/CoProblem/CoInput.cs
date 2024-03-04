using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

internal sealed class CoInput
{
    public List<CoPupil> Pupils { get; set; } = [];
    public List<CoBuddyGroup> BuddyGroups { get; set; } = [];
    public List<CoActivity> Activities { get; set; } = [];
    public List<CoLocation> Locations { get; set; } = [];
    public List<CoDoOrDont> DoOrDonts { get; set; } = [];
    public Dictionary<(CoActivity, CoBuddyGroup), int> History { get; set; } = [];
}

internal sealed record class CoPupil(int Id, Gender Gender, int GroupId, bool NeedsAttention, CoBuddyGroup BuddyGroup);

internal enum Gender { Male = 2, Female = 1 }

internal sealed record class CoBuddyGroup(int Id, List<CoPupil> Pupils);

internal sealed record class CoActivity(int Id, CoLocation Location);

internal sealed record class CoActivityGroup(List<CoActivity> Activities, CoLocation Location);

internal sealed record class CoLocation(int Id, int MaxCapacity, bool DoAll, List<CoActivityGroup> ActivityGroups);

internal sealed record class CoDoOrDont(CoPupil Pupil, CoActivity Activity, bool MustDo);