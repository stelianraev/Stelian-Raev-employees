﻿namespace EmployeeExtractor.Services
{
    using EmployeeExtractor.Models;
    using System.Collections.Generic;

    public class Engine
    {
        private readonly FileParser _fileParser;
        private readonly ILogger _logger;
        public Engine(ILogger<Engine> logger, FileParser fileParser)
        {
            _fileParser = fileParser;
            _logger = logger;
        }

        public FileParser FileParser => _fileParser;

        public CsvViewModel CalculateWorkerPairs(ICollection<CSVWorkerModel> workerModelCollection)
        {
            CsvViewModel csvViewModel = new CsvViewModel();
            ICollection<CsvWorkedTogether> workedTogether = new List<CsvWorkedTogether>();
            ICollection<CsvWorkedTogether> dublicates = new List<CsvWorkedTogether>();
            try
            {
                var extractWorkersFromCollection = workerModelCollection?.GroupBy(x => x.ProjectID).Where(y => y.Count() > 1).ToList();

                //TODO (Possible Recursion)
                foreach (IGrouping<int, CSVWorkerModel> proj in extractWorkersFromCollection)
                {
                    var test = proj.ToList();

                    for (int i = 0; i < test.Count; i++)
                    {
                        var worker1 = test[i];

                        for (int j = i + 1; j < test.Count; j++)
                        {
                            var worker2 = test[j];

                            DateTime start = worker1.DateFrom > worker2.DateFrom ? worker1.DateFrom : worker2.DateFrom;
                            DateTime? end = worker1.DateTo < worker2.DateTo ? worker1.DateTo : worker2.DateTo;
                            TimeSpan duration = (TimeSpan)(end - start);
                            var time = duration > TimeSpan.Zero ? duration : TimeSpan.Zero;
                            var days = time.Days;

                            int smallerID = worker1.EmpID < worker2.EmpID ? worker1.EmpID : worker2.EmpID;
                            int largerID = worker1.EmpID > worker2.EmpID ? worker1.EmpID : worker2.EmpID;

                            CsvWorkedTogether workerModel = new CsvWorkedTogether()
                            {
                                EmployeeID1 = smallerID,
                                EmployeeID2 = largerID,
                                ProjectID = worker1.ProjectID,
                                WorkedTimeTogether = days
                            };

                            if (worker1.EmpID == worker2.EmpID)
                            {
                                dublicates.Add(workerModel);
                            }
                            else
                            {
                                if (days > 0)
                                {
                                    workedTogether.Add(workerModel);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CsvFile calculation {Calculation}", ex);
                throw;
            }

            csvViewModel.CsvWorkerDublicatesCollection = dublicates;

            if (workedTogether.Any())
            {
                var groups = workedTogether.GroupBy(x => new { x.EmployeeID1, x.EmployeeID2 })
                           .OrderByDescending(x => x.Sum(x => x.WorkedTimeTogether))
                           .ToList();

                var highestSum = groups.First().Sum(x => x.WorkedTimeTogether);

                var highestGroups = groups.Where(x => x.Sum(y => y.WorkedTimeTogether) == highestSum)
                                          .SelectMany(x => x)
                                          .ToList();

                csvViewModel.CsvWorkedCollection = highestGroups;
                _logger.LogInformation("Succesfully pass CsvFile Calculation block");
            }

            return csvViewModel;
        }
    }
}
