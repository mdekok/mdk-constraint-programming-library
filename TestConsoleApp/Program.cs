using Google.OrTools.Sat;
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

    .AddObjective<CoObjective>()

    .SetResultsBuilder<CoResultsBuilder>();

SolutionCallback solutionCallback = new ObjectiveSolutionPrinter();

CoResults results = solver.Solve(solutionCallback);

Console.WriteLine($"Solver status: {results.SolverStatus}");
Console.WriteLine($"Elapsed time: {results.WallTime} s");
Console.WriteLine($"Objective: {results.ObjectiveValue}, branches: {results.NumBranches}, conflicts: {results.NumConflicts}");

if (results.SolverStatus != CpSolverStatus.Optimal && results.SolverStatus != CpSolverStatus.Feasible)
{
    Console.WriteLine("No solution found");
    Environment.Exit(1);
}

Console.WriteLine();
Console.WriteLine($"Solution with general spread info:");

foreach (CoActivity activity in input.Activities)
{
    int n = 0;
    int nMale = 0;
    int nFemale = 0;
    int nNeedAttention = 0;

    List<CoResult> activityResults = results.Values.Where(result => result.Activity == activity).ToList();

    foreach (CoResult result in activityResults)
    {
        CoPupil pupil = result.Pupil;
        bool isMale = pupil.Gender == Gender.Male;

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

    string groupSpread = string.Empty;
    foreach (IGrouping<int, CoResult> grouping in activityResults
        .GroupBy(result => result.Pupil.GroupId)
        .OrderBy(grouping => grouping.Key))
    {
        groupSpread += grouping.Count();
    }

    Console.WriteLine($"a{activity.Id} n{n} (f{nFemale}/m{nMale}/a{nNeedAttention}) {groupSpread}");
}

Console.WriteLine();
Console.WriteLine("Dos or Don'ts");
foreach (CoDoOrDont doOrDont in input.DoOrDonts)
{
    Console.WriteLine($"p{doOrDont.Pupil.Id} a{doOrDont.Activity.Id} {doOrDont.MustDo} == {results.PupilDoesActivity(doOrDont.Pupil, doOrDont.Activity)} history gap {input.History[(doOrDont.Activity, doOrDont.Pupil.BuddyGroup)]} ({doOrDont.Pupil.BuddyGroup.Pupils.Count})");
}

Console.WriteLine();
Console.WriteLine($"History gaps distribution:");

int[] HistoryGapCount = new int[input.Configuration.MaxHistoryGap + 1];

foreach (CoResult result in results.Values)
{
    int historyGap = input.History[(result.Activity, result.Pupil.BuddyGroup)];
    HistoryGapCount[historyGap]++;
}

for (int i = 0; i < input.Configuration.MaxHistoryGap; i++)
{
    Console.WriteLine($"History gap {i} count {HistoryGapCount[i]}");
}
Console.WriteLine($"Max history gap count {HistoryGapCount[input.Configuration.MaxHistoryGap]}");