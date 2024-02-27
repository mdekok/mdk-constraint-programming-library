
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace TestConsoleApp.CoProblem.Import;

internal sealed class CoImporter(CoInput input)
{
    readonly CsvConfiguration config = new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";"
    };
    readonly NullableIntConverter nullableIntConverter = new();
    readonly GenderConverter genderConverter = new();

    internal void Import()
    {
        this.ImportPupils();
        this.ImportActivities();
        this.ImportHistory();
        this.ImportMustDo();
    }

    private void ImportPupils()
    {
        using var reader = new StreamReader("C:\\Source\\Repos\\mdk-constraint-programming-library\\Data\\Pupils.csv");
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            int pupilId = csv.GetField<int>("Id");
            int buddyGroupId = csv.GetField<int>("Team", nullableIntConverter);

            CoBuddyGroup? buddyGroup = input.BuddyGroups.FirstOrDefault(buddyGroup => buddyGroup.Id == buddyGroupId);

            if (buddyGroupId == 0) // Pupil is not in a buddy group, he/she is in a group of his/her own.
            {
                buddyGroup = new CoBuddyGroup(-pupilId, []);
                input.BuddyGroups.Add(buddyGroup);
            }
            else
            {
                buddyGroup = input.BuddyGroups.FirstOrDefault(buddyGroup => buddyGroup.Id == buddyGroupId);
                if (buddyGroup is null)
                {
                    buddyGroup = new CoBuddyGroup(buddyGroupId, []);
                    input.BuddyGroups.Add(buddyGroup);
                }
            }

            CoPupil pupil = new(pupilId,
                csv.GetField<Gender>("Gender", genderConverter),
                csv.GetField<int>("IdGroup"),
                csv.GetField<bool>("NeedsAttention"),
                buddyGroup);

            buddyGroup.Pupils.Add(pupil);
            input.Pupils.Add(pupil);
        }
    }

    private void ImportActivities()
    {
        using var reader = new StreamReader("C:\\Source\\Repos\\mdk-constraint-programming-library\\Data\\Activities.csv");
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            int activityId = csv.GetField<int>("IdActivity");
            int maxCapacity = csv.GetField<int>("MaxCapacity", nullableIntConverter);
            if (activityId == 10 || activityId == 22)
            {
                maxCapacity = 12;
            }

            input.Activities.Add(new(activityId, maxCapacity));
        }
    }

    private void ImportHistory()
    {
        foreach (CoActivity activity in input.Activities)
            foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
            {
                input.History[(activity, buddyGroup)] = input.Configuration.MaxHistoryGap; // Set to max value
            }

        int refEventId = 1102; // The last used event id

        using (var reader = new StreamReader("C:\\Source\\Repos\\mdk-constraint-programming-library\\Data\\History.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                int idEvent = csv.GetField<int>("IdEvent");
                int idActivity = csv.GetField<int>("IdActivity");
                int idPupil = csv.GetField<int>("IdStudent");

                CoPupil pupil = input.Pupils.First(pupil => pupil.Id == idPupil);
                if (input.Activities.FirstOrDefault(activity => activity.Id == idActivity) is not CoActivity activity)
                    continue; // Ignore non relevant activities
                int historyGap = refEventId - idEvent;

                if (input.History[(activity, pupil.BuddyGroup)] > historyGap)
                {
                    input.History[(activity, pupil.BuddyGroup)] = historyGap;
                }
            }
        }
    }

    private void ImportMustDo()
    {
        CoPupil mustDoPupil1 = input.Pupils.First(pupil => pupil.Id == 1459);
        CoPupil mustDoPupil2 = input.Pupils.First(pupil => pupil.Id == 1505);
        CoActivity mustDoActivity = input.Activities.First(activity => activity.Id == 22);

        input.DoOrDonts.Add(new(mustDoPupil1, mustDoActivity, true));
        input.DoOrDonts.Add(new(mustDoPupil2, mustDoActivity, true));
    }
}
