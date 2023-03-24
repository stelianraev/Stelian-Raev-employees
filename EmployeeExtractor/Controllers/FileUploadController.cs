namespace EmployeeExtractor.Controllers
{
    using EmployeeExtractor.Services;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class FileUploadController : Controller
    {
        private readonly FileParser _fileParser;
        public FileUploadController(FileParser fileParser)
        {
            _fileParser = fileParser;
        }

        public FileParser FileParser
        {
            get { return _fileParser; }
        }

        public IActionResult FileUpload() => View(new EmployeeExtractorModel());

        [HttpPost]
        public IActionResult FileUpload(EmployeeExtractorModel model, IFormFile fileSelect)
        {
            //check if file is from allowedFileTypes. Possible to extend with more files
            string[] allowedFileTypes = { ".csv" };
            var extension = Path.GetExtension(fileSelect.FileName).ToLower();

            if (!allowedFileTypes.Contains(extension))
            {
                this.ModelState.AddModelError("file", "Only .CSV file is allowed.");
            }

            if (fileSelect != null)
            {
                //File size validation max size 5MB
                if (fileSelect.Length > 5 * 1024 * 1024)
                {
                    this.ModelState.AddModelError("file", "File is too big. Max size is 5MB");
                }                                
            }
            else
            {
                this.ModelState.AddModelError("file", "File is missing. Please choose a file");
            }

            //model.HtmlTable = 
            var result = FileParser.ParseCsvToHtmlTable(fileSelect);
            model.HtmlTable = FileParser.ParseCsvToHtmlTable(result);

            if (result.CsvWorkerDublicatesCollection.Count > 0)
            {
                foreach (var dublicate in result.CsvWorkerDublicatesCollection)
                {
                    this.ModelState.AddModelError("file", $"Dublicates in the file: EmployeeID: {dublicate.EmployeeID1}, ProjectID {dublicate.ProjectID}" );
                }                
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return View(model);
        }
    }
}
