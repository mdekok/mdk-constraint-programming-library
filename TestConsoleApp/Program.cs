using Google.OrTools.Sat;
using System.Diagnostics;
using TestConsoleApp;
using TestConsoleApp.CoProblem;
using TestConsoleApp.CoProblem.Constraints;
using TestConsoleApp.CoProblem.Import;

CoInput input = new();

CoImporter importer = new(input);
importer.Import();

Console.WriteLine($"{input.PupilCount()} pupils ({input.NeedingAttentionCount()} need attention)");
Console.WriteLine($"{input.MaleCount()} males");
Console.WriteLine($"{input.FemaleCount()} females");
Console.WriteLine($"{input.BuddyGroupCount()} buddy groups");

CoSolver solver = new(input);

solver
    .SetInputValidator<CoInputValidator>()

    .SetVariablesBuilder<CoVariablesBuilder>()

    .AddConstraint<BuddyGroupsDoExactlyOneActivityConstraint>()
    .AddConstraint<SlotsAreAssignedMaxOnceConstraint>()
    .AddConstraint<PupilSpreadConstraint>()
    .AddConstraint<GenderSpreadConstraint>()
    .AddConstraint<GroupSpreadConstraint>()
    .AddConstraint<PupilNeedingAttentionSpreadConstraint>()

    .AddObjective<CoObjective>();

Stopwatch stopwatch = Stopwatch.StartNew();

SolutionCallback solutionCallback = new ObjectiveSolutionPrinter();

CpSolverStatus solverStatus = solver.Solve(solutionCallback);

Console.WriteLine($"Solver status: {solverStatus}");

stopwatch.Stop();

bool solutionFound = solverStatus == CpSolverStatus.Optimal || solverStatus == CpSolverStatus.Feasible;

if (!solutionFound)
{
    Console.WriteLine("No solution found");
    Environment.Exit(1);
}

Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"Objective: {solver.ObjectiveValue}, branches: {solver.NumBranches}, conflicts: {solver.NumConflicts}, walltime: {solver.WallTime}");

CoResults results = new();

input.DoOrDonts.ForEach(doOrDont =>
{
    if (doOrDont.MustDo)
    {
        doOrDont.Pupil.BuddyGroup.Pupils.ForEach(pupil => results.Add(new(pupil, doOrDont.Activity)));
    }
});

foreach (CoActivity activity in input.Activities)
    foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
    {
        if (solver.IsSet(buddyGroup, activity))
        {
            buddyGroup.Pupils.ForEach(pupil => results.Add(new(pupil, activity)));
        }
    }

Console.WriteLine();
Console.WriteLine($"Solution with general spread info:");

foreach (CoActivity activity in input.Activities)
{
    int n = 0;
    int nMale = 0;
    int nFemale = 0;
    int nNeedAttention = 0;

    foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
        foreach (CoPupil pupil in buddyGroup.Pupils)
        {
            bool isMale = pupil.Gender == Gender.Male;
            if (results.PupilDoesActivity(pupil, activity))
            {
                n++;
                if (isMale)
                {
                    nMale++;
                }
                else
                {
                    nFemale++;
                }
                if (pupil.NeedsAttention)
                {
                    nNeedAttention++;
                }
            }
        }

    string groupSpread = string.Empty;
    foreach (IGrouping<int, CoPupil> grouping in input
        .BuddyGroups
        .SelectMany(buddyGroup => buddyGroup.Pupils)
        .GroupBy(pupil => pupil.GroupId)
        .OrderBy(grouping => grouping.Key))
    {
        long nGroup = 0;

        foreach (CoPupil pupil in grouping)
        {
            if (results.PupilDoesActivity(pupil, activity))
            {
                nGroup++;
            }
        }
        groupSpread += nGroup.ToString();
    }

    Console.WriteLine($"a{activity.Id} n{n} (f{nFemale}/m{nMale}/a{nNeedAttention}) {groupSpread}");
}

Console.WriteLine();
Console.WriteLine("Dos or Don'ts");
foreach (CoDoOrDont doOrDont in input.DoOrDonts)
{
    Console.WriteLine($"p{doOrDont.Pupil.Id} a{doOrDont.Activity.Id} {doOrDont.MustDo} == {results.PupilDoesActivity(doOrDont.Pupil, doOrDont.Activity)} history gap {input.History[(doOrDont.Activity, doOrDont.Pupil.BuddyGroup)]}");
}

int[] HistoryGapCount = new int[input.Configuration.MaxHistoryGap + 1]; 

foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
    foreach (CoActivity activity in input.Activities.Where(activity => solver.IsSet(buddyGroup, activity)))
    {
        int historyGap = input.History[(activity, buddyGroup)];
        HistoryGapCount[historyGap] += buddyGroup.Pupils.Count;
    }

Console.WriteLine();
Console.WriteLine($"History gaps distribution:");

for (int i = 0; i < input.Configuration.MaxHistoryGap; i++)
{
    Console.WriteLine($"History gap {i} count {HistoryGapCount[i]}");
}
Console.WriteLine($"Max history gap count {HistoryGapCount[input.Configuration.MaxHistoryGap]}");