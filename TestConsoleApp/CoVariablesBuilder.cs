namespace TestConsoleApp;

using Google.OrTools.Sat;
using MdkConstraintProgrammingLibrary;

internal class CoVariablesBuilder : MdkCpVariablesBuilder<CoInput, CoVariables>
{
    public override CoVariables Build(CpModel cpModel, CoInput input)
    {
        foreach (CoActivity activity in input.Activities.Where(activity => activity.MaxCapacity == 0))
        {
            activity.MaxCapacity = input.DefaultMaxCapacity;
        }

        CoVariables variables = [];

        foreach (CoPupil pupil in input.Pupils)
            foreach (CoActivity activity in input.Activities)
            {
                for (int i = 0; i < activity.MaxCapacity; i++)
                {
                    variables.Add((pupil, activity, i),
                        cpModel.NewBoolVar($"slot_b{pupil.Id}_a{activity.Id}_s{i}"));
                }
            }

        return variables;
    }
}