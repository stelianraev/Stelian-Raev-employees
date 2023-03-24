namespace EmployeeExtractor.Services
{
    using EmployeeExtractor.Models;
    using System.Collections.Generic;
    using System.Text;

    public class FileParser
    {
        public CsvViewModel ParseCsvToHtmlTable(IFormFile csvFile)
        {
            bool isHeaderRow = default;
            CsvViewModel csvViewModel = new CsvViewModel();
            ICollection<CSVWorkerModel> workerModelCollection = new List<CSVWorkerModel>();

            using (StreamReader reader = new StreamReader(csvFile.OpenReadStream()))
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (DateTime.TryParse(values[2], out _) == false)
                {
                    isHeaderRow = true;
                }
                else
                {
                    isHeaderRow = false;
                }

                while (!reader.EndOfStream)
                {
                    if (isHeaderRow)
                    {
                        line = reader.ReadLine();
                    }

                    values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    workerModelCollection.Add(new CSVWorkerModel()
                    {
                        EmpID = int.Parse(values[0]),
                        ProjectID = int.Parse(values[1]),
                        DateFrom = DateTime.Parse(values[2]),
                        DateTo = DateTime.Parse(values[3].ToLower() == "null" || String.IsNullOrEmpty(values[3]) ? DateTime.Today.ToString() : values[3]),
                    });
                }
            }

            var extractWorkersFromCollection = workerModelCollection.GroupBy(x => x.ProjectID).Where(y => y.Count() > 1).ToList();

            ICollection<CsvWorkedTogether> workedTogether = new List<CsvWorkedTogether>();
            ICollection<CsvWorkedTogether> dublicates = new List<CsvWorkedTogether>();
            //ICollection<Dictionary<int, List<CsvWorkedTogether>>> employeeCollection = new List<Dictionary<int, List<CsvWorkedTogether>>>();

            //TODO (Possible Recursion)
            foreach (IGrouping<int, CSVWorkerModel> proj in extractWorkersFromCollection)
            {
                var test = proj.ToList();

                for (int i = 0; i < test.Count; i++)
                {
                    var worker1 = test[i];

                    for (int j = i + 1; j < test.Count; j++)
                    {
                        if (worker1.ProjectID == 10)
                        {

                        }

                        var worker2 = test[j];

                        DateTime start = worker1.DateFrom > worker2.DateFrom ? worker1.DateFrom : worker2.DateFrom;
                        DateTime? end = worker1.DateTo < worker2.DateTo ? worker1.DateTo : worker2.DateTo;
                        TimeSpan duration = (TimeSpan)(end - start);
                        var time = duration > TimeSpan.Zero ? duration : TimeSpan.Zero;
                        var days = time.Days;

                        if (days > 0)
                        {
                            CsvWorkedTogether workerModel = new CsvWorkedTogether()
                            {
                                EmployeeID1 = worker1.EmpID,
                                EmployeeID2 = worker2.EmpID,
                                ProjectID = worker1.ProjectID,
                                WorkedTimeTogether = days
                            };

                            //TODO configuration
                            //var dublicate = workedTogether.FirstOrDefault(x => x.EmployeeID1 == workerModel.EmployeeID1 
                            //                                                && x.EmployeeID2 == workerModel.EmployeeID2 
                            //                                                && x.ProjectID == workerModel.ProjectID
                            //                                                && x.WorkedTimeTogether == workerModel.WorkedTimeTogether);
                            //
                            //if (dublicate != null)
                            //{
                            //    continue;
                            //    //dublicates.Add(dublicate);
                            //}
                            //if(worker1.EmpID == worker2.EmpID && worker1.DateFrom == worker2.DateFrom && worker1.DateTo == worker2.DateTo)
                            if (worker1.EmpID == worker2.EmpID)
                            {
                                //TODO Return Error for dublicates
                                // Dublicates - dublicate message - check your csv file
                                dublicates.Add(workerModel);
                            }
                            else
                            {
                                workedTogether.Add(workerModel);
                            }
                        }
                    }
                }
            }

            csvViewModel.CsvWorkerDublicatesCollection = dublicates;
            var result = workedTogether.OrderByDescending(x => x.WorkedTimeTogether).First();

            ICollection<CsvWorkedTogether> allEmployees = workedTogether.Where(x => x.EmployeeID1 == result.EmployeeID1 && x.EmployeeID2 == result.EmployeeID2).ToList();
            csvViewModel.CsvWorkedCollection = allEmployees;
            return csvViewModel;
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

        public string ParseCsvToHtmlTable(CsvViewModel csvViewModel)
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
