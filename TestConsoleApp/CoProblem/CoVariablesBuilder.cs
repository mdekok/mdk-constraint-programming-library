namespace TestConsoleApp;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using System.Collections.Generic;
using TestConsoleApp.CoProblem;

internal class CoVariablesBuilder : MdkCpVariablesBuilder<CoInput, CoVariables>
{
    public override CoVariables Build(CpModel cpModel, CoInput input)
    {
        CoVariables variables = [];

        int pupilCountPerBuddyGroup = input.PupilCount() / input.BuddyGroupCount();
        int pupilCountLeft = input.PupilCount();
        int locationCountLeft = input.LocationCount();
        foreach (CoLocation location in input.Locations.Where(location => location.MaxCapacity != 0))
        {
            pupilCountLeft -= location.MaxCapacity;
            locationCountLeft--;
        }
        int capacityNeededForNonMaximizedLocations = pupilCountLeft / locationCountLeft + 1;

        foreach (CoLocation location in input.Locations)
        {
            int capacityNeeded = location.MaxCapacity == 0
                ? capacityNeededForNonMaximizedLocations
                : location.MaxCapacity;
            int slotCount = capacityNeeded / pupilCountPerBuddyGroup + 1;

            // Remove slots for buddy groups of which a pupil must do the activity or one of the activities
            slotCount -= input.MustDoBuddyGroups(location).Count();

            int activityGroupIndex = 0;
            foreach (CoActivityGroup activityGroup in location.ActivityGroups)
            {
                List<CoBuddyGroup> ignoredBuddyGroups = input
                    .DoOrDonts
                    .Where(doOrDont => activityGroup.Activities.Contains(doOrDont.Activity))
                    .Select(doOrDont => doOrDont.Pupil.BuddyGroup)
                    .Distinct()
                    .ToList();

                foreach (CoBuddyGroup buddyGroup in input.PlannableBuddyGroups())
                {
                    List<BoolVar> slots = [];

                    if (!ignoredBuddyGroups.Contains(buddyGroup))
                    {
                        for (int i = 0; i < slotCount; i++)
                        {
                            slots.Add(cpModel.NewBoolVar($"slot_b{buddyGroup.Id}_a{activityGroupIndex}_s{i}"));
                        }
                    }

                    variables.Add((buddyGroup, activityGroup), slots);
                }
                activityGroupIndex++;
            }
        }

        return variables;
    }
}