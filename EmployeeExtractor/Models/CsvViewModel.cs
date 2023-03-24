namespace EmployeeExtractor.Models
{
    public class CsvViewModel
    {
        public ICollection<CsvWorkedTogether> CsvWorkedCollection { get; set; }
        public ICollection<CsvWorkedTogether> CsvWorkerDublicatesCollection { get; set; }
    }
}
