using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
    [Route("[Controller]")]
    public class CountriesController : Controller
    {

        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }
        [Route("UploadFromExcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("UploadFromExcel")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select a valid Excel file.";
                return View();
            }

            if(!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Only .xlsx files are allowed.";
                return View();
            }

            // Add your logic to process the Excel file here

       int CountriesCountInserted =   await  _countriesService.UploadCountriesFromExcelFile(excelFile);

            ViewBag.Message = $"{CountriesCountInserted} countries have been successfully uploaded from the Excel file.";

            return View();
        }
    }
}
