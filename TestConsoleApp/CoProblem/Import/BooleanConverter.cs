using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TestConsoleApp.CoProblem.Import;

internal class BooleanConverter : ITypeConverter
{
    public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        => text == "1";

    public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        => throw new NotImplementedException();
}
