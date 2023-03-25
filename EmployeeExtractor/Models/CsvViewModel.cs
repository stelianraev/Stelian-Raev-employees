namespace EmployeeExtractor.Models
{
    public class CsvViewModel
    {
        public ICollection<CsvWorkedTogether> CsvWorkedCollection { get; set; } = new List<CsvWorkedTogether>();
        public ICollection<CsvWorkedTogether> CsvWorkerDublicatesCollection { get; set; } = new List<CsvWorkedTogether>();
    }
}
