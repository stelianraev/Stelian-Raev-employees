using EmployeeExtractor.Models;

namespace EmployeeExtractor.Services
{
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
                var extractWorkersFromCollection = workerModelCollection.GroupBy(x => x.ProjectID).Where(y => y.Count() > 1).ToList();

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

                            if (days > 0)
                            {
                                CsvWorkedTogether workerModel = new CsvWorkedTogether()
                                {
                                    EmployeeID1 = worker1.EmpID,
                                    EmployeeID2 = worker2.EmpID,
                                    ProjectID = worker1.ProjectID,
                                    WorkedTimeTogether = days
                                };
                                //checking for dublicates
                                if (worker1.EmpID == worker2.EmpID)
                                {
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
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CsvFile calculation {Calculation}", ex);
            }

            csvViewModel.CsvWorkerDublicatesCollection = dublicates;

            if(workedTogether.Any())
            {
				var result = workedTogether.OrderByDescending(x => x.WorkedTimeTogether).First();
				ICollection<CsvWorkedTogether> allEmployees = workedTogether.Where(x => (x.EmployeeID1 == result.EmployeeID1 && x.EmployeeID2 == result.EmployeeID2) || (x.EmployeeID1 == result.EmployeeID2 && x.EmployeeID2 == result.EmployeeID1)).ToList();
				csvViewModel.CsvWorkedCollection = allEmployees;
				_logger.LogInformation("Succesfully pass CsvFile Calculation block");
			}        

            return csvViewModel;
        }
    }
}
