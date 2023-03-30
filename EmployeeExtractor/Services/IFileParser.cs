namespace EmployeeExtractor.Services
{
    using EmployeeExtractor.Models;
    public interface IFileParser
    {
        Task<ICollection<CSVWorkerModel>> ParseCsvCustomModelAsync(IFormFile csvFile);

        /// <summary>
        /// Checking for headers. If there is no headers this will parse first raw
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        bool CsvHeadersChecker(string[] values);

        CSVWorkerModel CSVWorkerModelFactory(string[] values);

        string CsvModelToHtmlTable(CsvViewModel csvViewModel);
    }
}
