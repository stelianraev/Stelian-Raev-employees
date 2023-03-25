namespace EmployeeExtractor.Services
{
    using System.Text;
    using System.Collections.Generic;

    using EmployeeExtractor.Models;

    public class FileParser
    {
        private readonly ILogger _logger;
        public FileParser(ILogger<FileParser> logger)
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

                //Checking for headers. If there is no headers this will parse first raw
                if (DateTime.TryParse(values[2], out _))
                {
                    workerModelCollection.Add(CSVWorkerModelFactory(values));
				}

                while (!reader.EndOfStream)
                {                    
					line = await reader.ReadLineAsync();
					values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

					workerModelCollection.Add(CSVWorkerModelFactory(values));
					//try
					//{
					//    workerModelCollection.Add(new CSVWorkerModel()
					//    {
					//        EmpID = int.Parse(values[0]),
					//        ProjectID = int.Parse(values[1]),
					//        DateFrom = DateTime.Parse(values[2]),
					//        DateTo = DateTime.Parse(values[3].ToLower() == "null" || String.IsNullOrEmpty(values[3]) ? DateTime.Today.ToString() : values[3]),
					//    });
					//}
					//catch(Exception ex)
					//{
					//    _logger.LogError("Csv parsing model fail {Parser}", ex);
					//}
				}
            }

            _logger.LogInformation("Successfully pass CSV Parsing");
            return workerModelCollection;
        }

        private CSVWorkerModel CSVWorkerModelFactory(string[] values)
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


        //public static void TestRecursion(List<CSVWorkerModel> projGroup, int index)
        //{
        //    if(index == projGroup.Count - 1)
        //    {
        //        return;
        //    }
        //
        //    var worker1 = projGroup[index];
        //
        //    TestRecursion(projGroup, index + 1);
        //
        //}

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
