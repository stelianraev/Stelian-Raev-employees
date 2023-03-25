namespace EmployeeExtractor.Models
{
    public class CSVWorkerModel
    {
        public int EmpID { get; init; }
        public int ProjectID { get; init; }
        public DateTime DateFrom { get; init; }
        public DateTime? DateTo { get; init; }
    }
}
