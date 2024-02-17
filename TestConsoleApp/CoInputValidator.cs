
namespace TestConsoleApp;

using FluentValidation;
using FluentValidation.Results;
using MdkConstraintProgrammingLibrary;

internal class CoInputValidator : AbstractValidator<CoInput>, IMdkCpInputValidator<CoInput>
{
    public static string ValidationContextInputKey => "input";

    public CoInputValidator()
    {
        RuleFor(input => input.Activities.Count).GreaterThan(0);
        //RuleFor(d => d.Participants.Count).GreaterThan(0);
        //RuleForEach(d => d.Participants)
        //    .ChildRules(participant =>
        //    {
        //        participant.RuleFor(p => p.HistoryCount).GreaterThanOrEqualTo(0);
        //        participant.RuleFor(p => p.HistoryWeight).GreaterThanOrEqualTo(0);
        //        participant.RuleFor(p => p.HistoryGap).GreaterThanOrEqualTo(0);
        //        participant.RuleFor(p => p.Id).Must(this.ParticipantIsUnique);
        //    });

        //RuleFor(d => d.Activities.Count).GreaterThan(0);
        //RuleForEach(d => d.Activities)
        //    .ChildRules(activity =>
        //    {
        //        activity.RuleFor(a => a.CapacityNeeded).GreaterThanOrEqualTo(0);
        //        activity.RuleFor(a => a.Weight).GreaterThan(0);
        //        activity.RuleFor(p => p.Id).Must(this.ActivityIsUnique);
        //        activity.RuleFor(a => a.Assignments).Must(this.OneAssignmentPerParticipant);
        //        activity.RuleFor(a => a.Assignments).Must(this.EnoughParticipantsToAssign);
        //        activity.RuleForEach(a => a.Assignments)
        //            .ChildRules(assignment =>
        //            {
        //                assignment.RuleFor(a => a.IdParticipant).Must(ReferencesParticipant);
        //                assignment.RuleFor(a => a.AssignmentType).IsInEnum();
        //            });
        //    });
    }

    //private bool ParticipantIsUnique(JpParticipant participant, int idParticipant, ValidationContext<JpParticipant> context)
    //    => context.GetInput().Participants.Count(p => p.Id == idParticipant) == 1;

    //private bool ActivityIsUnique(JpActivity activity, int idActivity, ValidationContext<JpActivity> context)
    //    => context.GetInput().Activities.Count(a => a.Id == idActivity) == 1;

    //private bool ReferencesParticipant(JpAssignment assignment, int idParticipant, ValidationContext<JpAssignment> context)
    //    => context.GetInput().Participants.Any(p => p.Id == idParticipant);

    //private bool OneAssignmentPerParticipant(JpActivity activity, List<JpAssignment> assignmentList, ValidationContext<JpActivity> context)
    //{
    //    var groupByIdParticipant = assignmentList.GroupBy(assignment => assignment.IdParticipant);
    //    return groupByIdParticipant.Any()
    //    ? groupByIdParticipant.Max(group => group.Count()) == 1
    //    : true;
    //}

    //private bool EnoughParticipantsToAssign(JpActivity activity, List<JpAssignment> assignmentList, ValidationContext<JpActivity> context)
    //{
    //    int capacityAvailable = context.GetInput().Participants.Count
    //        - activity.Assignments.Count(a => a.AssignmentType == JobAssignmentType.WillNotDo || a.AssignmentType == JobAssignmentType.DontWantToDo);

    //    return capacityAvailable >= activity.CapacityNeeded;
    //}

    void IMdkCpInputValidator<CoInput>.Validate(CoInput input)
    {
        ValidationContext<CoInput> context = new ValidationContext<CoInput>(input);
        context.RootContextData[ValidationContextInputKey] = input;

        ValidationResult validationResult = this.Validate(context);

        if (!validationResult.IsValid)
            throw new CoException(validationResult.Errors.Select(failure => failure.ErrorMessage));
    }
}

public class CoException : Exception
{
    public CoException(IEnumerable<string> errors)
        : base(errors.FirstOrDefault())
        => this.Errors = errors.ToList();

    public IList<string> Errors { get; init; }
}