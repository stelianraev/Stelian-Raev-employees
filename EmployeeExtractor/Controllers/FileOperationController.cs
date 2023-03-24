using Microsoft.AspNetCore.Mvc;

namespace EmployeeExtractor.Controllers
{
    public class FileOperationController : Controller
    {
        public IActionResult CSVFileExtractWorkers()
        {

            return View();
        }
    }
}
