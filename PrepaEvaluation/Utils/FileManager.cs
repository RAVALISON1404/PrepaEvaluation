using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace PrepaEvaluation.Utils;

public class FileManager
{
    public static List<T> ImportFromCsv<T>(string filepath, string delimiter = ",")
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter,
            HeaderValidated = null, 
            MissingFieldFound = null,
            BadDataFound = context => throw new Exception($"Erreur de données : {context.RawRecord}"),
        };
        using var reader = new StreamReader(filepath);
        using var csv = new CsvReader(reader, csvConfig);
        return csv.GetRecords<T>().ToList();
    }

    public static void ExportToCsv<T>(IEnumerable<T> list, string filepath)
    {
        using var writer = new StreamWriter(filepath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(list);
    }

    public static List<T> ImportFromExcel<T>(string filePath, string sheetName) where T : new()
    {
        var data = new List<T>();
        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets[sheetName];
        var start = worksheet.Dimension.Start;
        var end = worksheet.Dimension.End;
        for (var row = start.Row + 1; row <= end.Row; row++)
        {
            var item = new T();
            for (var col = start.Column; col <= end.Column; col++)
            {
                var cellValue = worksheet.Cells[row, col].Value?.ToString()?.Trim();
                var property = typeof(T).GetProperty(worksheet.Cells[start.Row, col].Text.Trim());
                if (property != null && !string.IsNullOrEmpty(cellValue)) property.SetValue(item, Convert.ChangeType(cellValue, property.PropertyType), null);
            }
            data.Add(item);
        }
        return data;
    }
    
    public static void ExportToExcel<T>(IEnumerable<T> data, string filePath)
    {
        using var package = new ExcelPackage();
        using var worksheet = package.Workbook.Worksheets.Add("Data");
        worksheet.Cells.LoadFromCollection(data, true);
        File.WriteAllBytes(filePath, package.GetAsByteArray());
    }
}