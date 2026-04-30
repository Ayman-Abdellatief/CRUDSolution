using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Threading.Tasks;
namespace CRUDExample.Controllers
{
    [Route("[Controller]")]
    public class PersonsController : Controller
    {

        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        //constractur injection to inject PersonsService into the controller
     
        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            ViewBag.SearchFilds = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName),"Person Name" },
                { nameof(PersonResponse.Email),"Email" },
                { nameof(PersonResponse.DateOfBirth),"Date Of Birth" },
                { nameof(PersonResponse.Gender),"Gender" },
                { nameof(PersonResponse.CountryID),"Country" },
                { nameof(PersonResponse.Address),"Address" }
            };
            List<PersonResponse> personResponses =await _personsService.GetFilteredPersons(searchBy, searchString);
            ViewBag.CurrentsearchBy = searchBy;
            ViewBag.CurrentsearchString = searchString;
            //Sort
            List<PersonResponse> sortedPersonsRespons =await _personsService.GetSortedPersons(personResponses, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentsortOrder = sortOrder.ToString();

            return View(sortedPersonsRespons);
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> Countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = Countries.Select(temp =>
           
                new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryID.ToString()
                }
            );


            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if(!ModelState.IsValid)
            {
                List<CountryResponse> Countries =await _countriesService.GetAllCountries();
                ViewBag.Countries = Countries.Select(temp =>

                 new SelectListItem()
                 {
                     Text = temp.CountryName,
                     Value = temp.CountryID.ToString()
                 }
             );
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

         PersonResponse personResponse = await   _personsService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]

        [Route("[action]/{PersonID}")]
        public async Task<IActionResult> Edit(Guid PersonID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdteRequest();
            List<CountryResponse> Countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = Countries.Select(temp =>

             new SelectListItem()
             {
                 Text = temp.CountryName,
                 Value = temp.CountryID.ToString()
             }
         );

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{PersonID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
           PersonResponse personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if(ModelState.IsValid)
            {
                PersonResponse Updateperson =await  _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> Countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = Countries.Select(temp =>

                 new SelectListItem()
                 {
                     Text = temp.CountryName,
                     Value = temp.CountryID.ToString()
                 }
             );
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdteRequest());
            }
           
        }

        [HttpGet]
        [Route("[action]/{PersonID}")]
        public async Task<IActionResult> Delete(Guid? PersonID)
        {
            if (PersonID == null)
            {
                return RedirectToAction("Index");
            }
            PersonResponse? personResponse =await _personsService.GetPersonByPersonID(PersonID.Value);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
            return View(personResponse);

        }

        [HttpPost]
        [Route("[action]/{PersonID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
           await _personsService.DeletePerson(personUpdateRequest.PersonID);
            return RedirectToAction("Index");
        }
        }
}
