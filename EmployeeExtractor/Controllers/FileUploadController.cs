namespace EmployeeExtractor.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using Models;
    using EmployeeExtractor.Services;
    using EmployeeExtractor.Configuration;

    public class FileUploadController : Controller
    {
        private readonly Engine _engine;
        private readonly ServiceConfiguration _serviceConfiguration;
        public FileUploadController(Engine engine, IOptions<ServiceConfiguration> serviceConfiguration)
        {
            _engine = engine;
            _serviceConfiguration = serviceConfiguration.Value;
        }

        public Engine Engine
        {
            get { return _engine; }
        }

        public IActionResult FileUpload() => View(new CsvViewModel());

        [HttpPost]
        public async Task<IActionResult> FileUpload(CsvViewModel model, IFormFile fileSelect)
        {
            //check if file is from allowedFileTypes. Possible to extend with more files
            string[] allowedFileTypes = _serviceConfiguration.AllowedFileTypes;
            var extension = Path.GetExtension(fileSelect.FileName).ToLower();

            if (!allowedFileTypes.Contains(extension))
            {
                this.ModelState.AddModelError("file", "Only .CSV file is allowed.");
            }

            if (fileSelect != null)
            {
                //File size validation max size 5MB
                if (fileSelect.Length > _serviceConfiguration.FileSizeLimit * 1024 * 1024)
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

            try
            {
                var csvParse = await Engine.FileParser.ParseCsvCustomModelAsync(fileSelect);
                var workersCollection = Engine.CalculateWorkerPairs(csvParse);
                model.HtmlTable = Engine.FileParser.CsvModelToHtmlTable(workersCollection);

                if (workersCollection.CsvWorkerDublicatesCollection.Count > 0)
                {
                    foreach (var dublicate in workersCollection.CsvWorkerDublicatesCollection)
                    {
                        this.ModelState.AddModelError("file", $"Dublicates in the file: EmployeeID: {dublicate.EmployeeID1}, ProjectID {dublicate.ProjectID}");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("file", "Invalid csv file or data inside. Please notice EmployeeID`s and ProjID must be numbers");
            }            

            return View(model);
        }
    }
}
