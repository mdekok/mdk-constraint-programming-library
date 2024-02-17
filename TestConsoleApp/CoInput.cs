namespace TestConsoleApp;

internal sealed class CoInput
{
    public int GroupCount { get; set; } = 12; // For this tests needs to be even
    public int DefaultMaxCapacity { get; set; } = 30;
    public List<CoPupil> Pupils { get; set; } = [];
    public List<CoBuddyGroup> BuddyGroups { get; set; } = [];
    public List<CoActivity> Activities { get; set; } = [];
}

internal sealed class CoPupil
{
    public int Id { get; set; }
    public Gender Gender { get; set; }
    public int GroupId { get; set; }
}

internal enum Gender { Male, Female }

internal sealed class CoBuddyGroup
{
    public int Id { get; set; }
    public List<CoPupil> Pupils { get; set; } = [];
}

internal sealed class CoActivity
{
    public int Id { get; set; }
    public int MaxCapacity { get; set; }
}
