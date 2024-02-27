using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TestConsoleApp.CoProblem.Import;

internal sealed class NullableIntConverter : ITypeConverter
{
    public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        => text == "NULL" ? 0 : int.Parse(text);

    public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        => throw new NotImplementedException();
}
