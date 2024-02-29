using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

namespace TestConsoleApp.CoProblem;

/// <summary>Objective class for the CoProblem: maximize the minimal history gap of activities.</summary>
internal sealed class CoObjective : MdkCpObjective<CoInput, CoVariables>
{
    /// <inheritdoc />
    public override void Build(CpModel cpModel, CoInput input, CoVariables cpVariables, double previousObjectiveValue)
    {
        IntVar objective = cpModel.NewIntVar(0, input.Configuration.MaxHistoryGap, "objective: max minimal history gap");

        foreach (CoBuddyGroup buddyGroup in input.PlannableBuddyGroups())
        {
            LinearExprBuilder linearExprBuilder = LinearExpr.NewBuilder();

            foreach (CoLocation location in input.Locations)
                foreach (CoActivityGroup activityGroup in location.ActivityGroups)
                {
                    int gap = activityGroup.Activities.Min(activity => input.History[(activity, buddyGroup)]);

                    foreach (BoolVar boolVar in cpVariables[(buddyGroup, activityGroup)])
                    {
                        linearExprBuilder.AddTerm(boolVar, gap);
                    }
                }

            IntVar gapVar = cpModel.NewIntVar(0, input.Configuration.MaxHistoryGap, $"Gap for buddy group {buddyGroup.Id}");
            cpModel.Add(gapVar == linearExprBuilder);
            cpModel.Add(gapVar >= objective);
        }

        cpModel.Maximize(objective);
    }
}
