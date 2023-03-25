namespace EmployeeExtractor.Controllers
{
    using EmployeeExtractor.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;

    public class FileUploadController : Controller
    {
        private readonly Engine _engine;
        public FileUploadController(Engine engine)
        {
            _engine = engine;
        }

        public Engine Engine
        {
            get { return _engine; }
        }

        public IActionResult FileUpload() => View(new EmployeeExtractorModel());

        [HttpPost]
        public async Task<IActionResult> FileUpload(EmployeeExtractorModel model, IFormFile fileSelect)
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var csvParse = await Engine.FileParser.ParseCsvCustomModelAsync(fileSelect);
            var workersCollection = Engine.CalculateWorkerPairs(csvParse);
            model.HtmlTable = Engine.FileParser.CsvModelToHtmlTable(workersCollection);

            if (workersCollection.CsvWorkerDublicatesCollection.Count > 0)
            {
                foreach (var dublicate in workersCollection.CsvWorkerDublicatesCollection)
                {
                    this.ModelState.AddModelError("file", $"Dublicates in the file: EmployeeID: {dublicate.EmployeeID1}, ProjectID {dublicate.ProjectID}" );
                }                
            }

            return View(model);
        }
    }
}
