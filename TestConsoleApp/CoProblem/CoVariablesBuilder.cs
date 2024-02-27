namespace TestConsoleApp;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using TestConsoleApp.CoProblem;

internal class CoVariablesBuilder : MdkCpVariablesBuilder<CoInput, CoVariables>
{
    public override CoVariables Build(CpModel cpModel, CoInput input)
    {
        CoVariables variables = [];

        int pupilCountPerBuddyGroup = input.PupilCount() / input.BuddyGroupCount();
        int pupilCountLeft = input.PupilCount();
        int activityCountLeft = input.ActivityCount();
        foreach (CoActivity activity in input.Activities.Where(activity => activity.MaxCapacity != 0))
        {
            pupilCountLeft -= activity.MaxCapacity;
            activityCountLeft--;
        }
        int capacityNeededForNonMaximizedActivity = pupilCountLeft / activityCountLeft + 1;


        foreach (CoActivity activity in input.Activities)
        {
            int capacityNeeded = activity.MaxCapacity == 0
                ? capacityNeededForNonMaximizedActivity
                : activity.MaxCapacity;
            int slotCount = capacityNeeded / pupilCountPerBuddyGroup + 1;

            List<CoBuddyGroup> DoBuddyGroups = input
                .DoOrDonts
                .Where(doOrDont => doOrDont.Activity == activity && doOrDont.MustDo)
                .Select(doOrDont => doOrDont.Pupil.BuddyGroup)
                .Distinct()
                .ToList();

            List<CoBuddyGroup> ignoredBuddyGroups = input
                .DoOrDonts
                .Where(doOrDont => doOrDont.Activity == activity)
                .Select(doOrDont => doOrDont.Pupil.BuddyGroup)
                .Distinct()
                .ToList();

            slotCount -= DoBuddyGroups.Count;

            foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
            {
                List<BoolVar> slots = [];

                if (!ignoredBuddyGroups.Contains(buddyGroup))
                {
                    for (int i = 0; i < slotCount; i++)
                    {
                        slots.Add(cpModel.NewBoolVar($"slot_b{buddyGroup.Id}_a{activity.Id}_s{i}"));
                    }
                }

                variables.Add((buddyGroup, activity), slots);
            }
        }

        return variables;
    }
}