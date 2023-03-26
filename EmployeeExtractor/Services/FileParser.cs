namespace EmployeeExtractor.Services
{
    using System.Text;
    using System.Collections.Generic;

    using EmployeeExtractor.Models;

    public class FileParser : IFileParser
    {
        private readonly ILogger _logger;

        public FileParser(ILogger<IFileParser> logger)
        {
            _logger = logger;
        }

        public async Task<ICollection<CSVWorkerModel>> ParseCsvCustomModelAsync(IFormFile csvFile)
        {
            ICollection<CSVWorkerModel> workerModelCollection = new List<CSVWorkerModel>();

            using (StreamReader reader = new StreamReader(csvFile.OpenReadStream()))
            {
                string line = await reader.ReadLineAsync();
                string[] values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (CsvHeadersChecker(values))
                {
                    workerModelCollection.Add(CSVWorkerModelFactory(values));
                }

                while (!reader.EndOfStream)
                {
                    line = await reader.ReadLineAsync();
                    values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    workerModelCollection.Add(CSVWorkerModelFactory(values));
                }
            }

            _logger.LogInformation("Successfully pass CSV Parsing");
            return workerModelCollection;
        }

        /// <summary>
        /// Checking for headers. If there is no headers this will parse first raw
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool CsvHeadersChecker(string[] values)
        {
            if (DateTime.TryParse(values[2], out _))
            {
                return true;
            }

            return false;
        }

        public CSVWorkerModel CSVWorkerModelFactory(string[] values)
        {
            CSVWorkerModel csvWorkerModel = null;

            try
            {
                csvWorkerModel = new CSVWorkerModel()
                {
                    EmpID = int.Parse(values[0]),
                    ProjectID = int.Parse(values[1]),
                    DateFrom = DateTime.Parse(values[2]),
                    DateTo = DateTime.Parse(values[3].ToLower() == "null" || String.IsNullOrEmpty(values[3]) ? DateTime.Today.ToString() : values[3]),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Csv parsing model fail {Parser}", ex);
            }

            return csvWorkerModel;
        }

        public string CsvModelToHtmlTable(CsvViewModel csvViewModel)
        {
            StringBuilder htmlTable = new StringBuilder();

            htmlTable.Append("<table>");
            htmlTable.Append("<thead><tr>");
            htmlTable.Append("<th>" + "Employee ID #1" + "</th>");
            htmlTable.Append("<th>" + "Employee ID #2" + "</th>");
            htmlTable.Append("<th>" + "Project ID" + "</th>");
            htmlTable.Append("<th>" + "Days worked" + "</th>");
            htmlTable.Append("</tr></thead><tbody>");

            foreach (var pair in csvViewModel.CsvWorkedCollection)
            {
                htmlTable.Append("<tr>");
                htmlTable.Append("<td>" + pair.EmployeeID1 + "</td>");
                htmlTable.Append("<td>" + pair.EmployeeID2 + "</td>");
                htmlTable.Append("<td>" + pair.ProjectID + "</td>");
                htmlTable.Append("<td>" + pair.WorkedTimeTogether + "</td>");
                htmlTable.Append("</tr>");
            }

            htmlTable.Append("</tbody></table>");
            return htmlTable.ToString();
        }
    }
}
