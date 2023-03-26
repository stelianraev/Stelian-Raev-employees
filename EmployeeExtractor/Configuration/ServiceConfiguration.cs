namespace EmployeeExtractor.Configuration
{
    public class ServiceConfiguration
    {
        public int FileSizeLimit { get; init; }
        public string[] AllowedFileTypes { get; init; }
    }
}
