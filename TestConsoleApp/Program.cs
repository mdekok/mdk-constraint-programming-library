// See https://aka.ms/new-console-template for more information

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;
using System.Diagnostics;
using TestConsoleApp;
using TestConsoleApp.Constraints;

CoInput input = new();

Random random = new();

for (int i = 0; i < 250; i++)
{
    CoPupil pupil = new()
    {
        Id = i,
        Gender = random.NextDouble() < 0.49 ? Gender.Female : Gender.Male,
        GroupId = random.Next(0, input.GroupCount)
    };

    input.Pupils.Add(pupil);
}

// Create buddy groups by pairing pupils from two groups.
// Group 0 is paired with group 6, 1 with 7, 2 with 8, etc.
int buddyGroupIndexDelta = input.GroupCount / 2;
int buddyGroupId = 0;
for (int i = 0; i < input.GroupCount / 2; i++)
{
    List<CoPupil> youngGroup = input.Pupils.Where(pupil => pupil.GroupId == i).ToList();
    List<CoPupil> oldGroup = input.Pupils.Where(pupil => pupil.GroupId == i + buddyGroupIndexDelta).ToList();

    int minGroupSize = Math.Min(youngGroup.Count, oldGroup.Count);

    List<CoBuddyGroup> buddyGroups = [];
    for (int j = 0; j < minGroupSize; j++)
    {
        CoBuddyGroup buddyGroup = new() { Id = buddyGroupId++ };
        buddyGroup.Pupils.AddRange(youngGroup.Where(pupil => youngGroup.IndexOf(pupil) % minGroupSize == j));
        buddyGroup.Pupils.AddRange(oldGroup.Where(pupil => oldGroup.IndexOf(pupil) % minGroupSize == j));
        input.BuddyGroups.Add(buddyGroup);
    }
}

Enumerable.Range(1, 12).ToList().ForEach(i => input.Activities.Add(
    new CoActivity
    {
        Id = i,
        MaxCapacity = i > 10 ? 12 : 0
    }));

MdkCpSolver<CoInput, CoVariables> solver = new()
{
    Input = input,
    TimeLimitInSeconds = 60
};

solver
    .SetInputValidator<CoInputValidator>()

    .SetVariablesBuilder<CoVariablesBuilder>()

    .AddConstraint<PupilsDoExactlyOneActivityConstraint>()
    .AddConstraint<SlotsAreAssignedMaxOnceConstraint>()
    .AddConstraint<PupilSpreadConstraint>()
    .AddConstraint<GenderSpreadConstraint>()
    .AddConstraint<GroupSpreadConstraint>()
    .AddConstraint<BuddyGroupPupilsDoSameActivityConstraint>()
    ;

//switch (dataModel.OptimizationType)
//{
//    case OptimizationType.OptimizeWeight:
//        solver.AddObjective<OptimizeWeightObjective>();
//        break;
//    case OptimizationType.OptimizeJobAssignmentSpread:
//        solver
//            .AddObjective<OptimizeJobAssignmentSpreadMaxGapObjective>()
//            .AddObjective<OptimizeJobAssignmentSpreadMinGapOccurencesObjective>();
//        break;
//}

// Tell the solver to enumerate all solutions.
// solver.StringParameters += "linearization_level:0 " + "enumerate_all_solutions:true ";

// const int solutionLimit = 50;
// SolutionPrinter? solutionPrinter = null; // new SolutionPrinter(solver, solutionLimit);

bool solutionFound;
IList<string>? errors;

Stopwatch stopwatch = Stopwatch.StartNew();

//try
//{
CpSolverStatus solverStatus = solver.Solve(null);

//if (solutionPrinter is not null && solutionLog is not null)
//{
//    solutionLog.Clear();
//    foreach (string s in solutionPrinter.Solution)
//        solutionLog.Add(s);
//    new SolutionLogger().Log(solutionLog, solver, solverStatus);
//}

stopwatch.Stop();


solutionFound = solverStatus == CpSolverStatus.Optimal || solverStatus == CpSolverStatus.Feasible;
errors = null;
//}
//catch (JobPlanningException e)
//{
//    solutionFound = false;
//    errors = e.Errors;
//}

//return new PlanningResultBuilder().Build(solver, solutionFound, errors);

if (!solutionFound)
{
    Console.WriteLine("No solution found");
    Environment.Exit(1);
}

foreach (CoPupil pupil in input.Pupils)
{
    string buddyAssignment = string.Empty;
    foreach (CoActivity activity in input.Activities)
    {
        int n = 0;
        for (int i = 0; i < activity.MaxCapacity; i++)
        {
            n += solver.Value(solver.Variables[(pupil, activity, i)]) == 1L ? 1 : 0;
        }
        buddyAssignment += n.ToString();
    }
    Console.WriteLine(buddyAssignment);
}

Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"{input.PupilCount()} pupils");
Console.WriteLine($"{input.MaleCount()} males");
Console.WriteLine($"{input.FemaleCount()} females");
Console.WriteLine($"{input.BuddyGroupCount()} buddy groups");

foreach (CoActivity activity in input.Activities)
{
    int n = 0;
    int nMale = 0;
    int nFemale = 0;

    foreach (CoPupil pupil in input.Pupils)
    {
        bool isMale = pupil.Gender == Gender.Male;
        for (int i = 0; i < activity.MaxCapacity; i++)
        {
            if (solver.Value(solver.Variables[(pupil, activity, i)]) == 1L)
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
            }
        }
    }

    string groupSpread = string.Empty;
    foreach (IGrouping<int, CoPupil> grouping in input.Pupils.GroupBy(pupil => pupil.GroupId).OrderBy(grouping => grouping.Key))
    {
        long nGroup = 0;
        for (int i = 0; i < activity.MaxCapacity; i++)
        {
            nGroup += grouping.Sum(pupil => solver.Value(solver.Variables[(pupil, activity, i)]));
        }
        groupSpread += nGroup.ToString();
    }

    Console.WriteLine($"a{activity.Id} n{n} (f{nFemale}/m{nMale}) {groupSpread}");
}

foreach (CoBuddyGroup buddyGroup in input.BuddyGroups)
{
    string activities = string.Empty;
    foreach (CoPupil pupil in buddyGroup.Pupils)
        foreach (CoActivity activity in input.Activities)
            for (int i = 0; i < activity.MaxCapacity; i++)
            {
                activities += solver.Value(solver.Variables[(pupil, activity, i)]) == 1L
                    ? $" {activity.Id}"
                    : string.Empty;
            }

    Console.WriteLine($"bg{buddyGroup.Id} activities:{activities}");
}