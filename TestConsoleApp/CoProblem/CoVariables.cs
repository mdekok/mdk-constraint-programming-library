namespace TestConsoleApp.CoProblem;

using Google.OrTools.Sat;

internal sealed class CoVariables : Dictionary<(CoBuddyGroup BuddyGroup, CoActivity Activity), List<BoolVar>>
{ }
