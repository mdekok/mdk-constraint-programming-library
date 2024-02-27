using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TestConsoleApp.CoProblem.Import;

internal class GenderConverter : ITypeConverter
{
    public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        => text == "1" ? Gender.Female : Gender.Male;

    public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        => throw new NotImplementedException();
}
