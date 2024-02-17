namespace TestConsoleApp;

using Google.OrTools.Sat;

internal sealed class CoVariables : Dictionary<(CoPupil Pupil, CoActivity Activity, int SlotIndex), BoolVar>
{
}
