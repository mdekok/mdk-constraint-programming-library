
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace TestConsoleApp.CoProblem.Import;

internal sealed class CoImporter(CoConfiguration configuration, CoInput input)
{
    readonly CsvConfiguration config = new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";"
    };
    readonly BooleanConverter booleanConverter = new();
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

            CoBuddyGroup? buddyGroup;

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
            int locationId = csv.GetField<int>("IdLocation");
            int maxCapacity = csv.GetField<int>("MaxCapacity", nullableIntConverter);
            bool doAll = csv.GetField<bool>("HasCombiActivities", booleanConverter);

            CoLocation? location = input.Locations.FirstOrDefault(location => location.Id == locationId);
            if (location is null)
            {
                location = new CoLocation(locationId, maxCapacity, doAll, []);
                input.Locations.Add(location);
            }

            CoActivity activity = new(activityId, location);
            input.Activities.Add(activity);

            if (doAll && location.ActivityGroups.Count == 1)
            {
                location.ActivityGroups.First().Activities.Add(activity);
            }
            else
            {
                location.ActivityGroups.Add(new CoActivityGroup([activity], location));
            }
        }
    }

    private void ImportHistory()
    {
        foreach (CoActivity activity in input.Activities)
            foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
            {
                input.History[(activity, buddyGroup)] = configuration.MaxHistoryGap; // Set to max value
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
