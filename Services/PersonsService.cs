using CsvHelper;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helper;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //private readonly List<Person> _persons = new List<Person>();
        private readonly IPersonsRepository _personRepository;
      

        public PersonsService(IPersonsRepository personsRepository)
        {
            _personRepository = personsRepository;

        }
  
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest), "Person details cannot be null.");

            //Model validation for PersonAddRequest

            ValidationHelper.ModelValidation(personAddRequest);


            //convert PersonAddRequest into Person type
            Person person = personAddRequest.ToPerson();
            //generta personID
            person.PersonID = Guid.NewGuid();
            //Add person to the list
          await  _personRepository.AddPerson(person);
         

           
            //Create a PersonResponse object to return
            return person.ToPersonResponse();


        }
        public async Task<List<PersonResponse>> GetAllPersons()
        {

            var persons = await _personRepository.GetAllPersons(); 
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
            //return _db.sp_GetAllPersons().Select(temp => temp.ToPersonResponse()).ToList(); 
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid personID)
        {
            //Check whether personID is empty

            if (personID == null)
                return null;
 
          Person? person =await  _personRepository.GetPersonsByPersonID(personID);
           
         if (person == null)
                return null;
            return person.ToPersonResponse();

        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString)
        {
            //Check if"Search By" is not Null

            List<Person> filteredPersons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                    await _personRepository.GetFilteredPersons(
                        x => x.PersonName != null &&
                             x.PersonName.Contains(searchString)
                    ),

                nameof(PersonResponse.Email) =>
                    await _personRepository.GetFilteredPersons(
                        x => x.Email != null &&
                             x.Email.Contains(searchString )
                    ),

                nameof(PersonResponse.DateOfBirth) =>
                    await _personRepository.GetFilteredPersons(
                        x => x.DateOfBirth != null &&
                             x.DateOfBirth.Value
                                .ToString("dd MMMM yyyy")
                                .Contains(searchString )
                    ),

                nameof(PersonResponse.Gender) =>
                    await _personRepository.GetFilteredPersons(
                        x => x.Gender != null &&
                             x.Gender.Contains(searchString )
                    ),

                nameof(PersonResponse.CountryID) =>
                    await _personRepository.GetFilteredPersons(
                        x => x.Country != null &&
                             x.Country.CountryName != null &&
                             x.Country.CountryName.Contains(searchString )
                    ),

                nameof(PersonResponse.Address) =>
                    await _personRepository.GetFilteredPersons(
                        x => x.Address != null &&
                             x.Address.Contains(searchString )
                    ),

                _ => await _personRepository.GetAllPersons()
            };


            return filteredPersons
                .Select(temp => temp.ToPersonResponse())
                .ToList();

        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;
            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
          switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                  allPersons.OrderBy(x => x.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
               allPersons.OrderByDescending(x => x.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                allPersons.OrderBy(x => x.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                   allPersons.OrderByDescending(x => x.DateOfBirth).ToList(),


                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
              allPersons.OrderBy(x => x.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                      allPersons.OrderByDescending(x => x.Age).ToList(),


                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                       allPersons.OrderBy(x => x.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(x => x.Gender, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                  allPersons.OrderBy(x => x.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                  allPersons.OrderByDescending(x => x.Country, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                 allPersons.OrderBy(x => x.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                   allPersons.OrderByDescending(x => x.Address, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                  allPersons.OrderBy(x => x.ReceiveNewsLetters ).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                   allPersons.OrderByDescending(x => x.ReceiveNewsLetters ).ToList(),

                 _  => allPersons


            };

            return sortedPersons;
        }

            public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
            {
                if (personUpdateRequest == null)
                    throw new ArgumentNullException(nameof(personUpdateRequest), "Person details cannot be null.");

                //Model validation for PersonUpdateRequest
                ValidationHelper.ModelValidation(personUpdateRequest);

                //Find the person to be updated
                Person? Matchingperson = await _personRepository.GetPersonsByPersonID(personUpdateRequest.PersonID);
                if (Matchingperson == null)
                    throw new KeyNotFoundException($"Person with ID {personUpdateRequest.PersonID} not found.");

            //Update the person details
            Matchingperson.PersonName = personUpdateRequest.PersonName;
            Matchingperson.Email = personUpdateRequest.Email;
            Matchingperson.DateOfBirth = personUpdateRequest.DateOfBirth;
            Matchingperson.Gender = personUpdateRequest.Gender.ToString();
            Matchingperson.CountryID = personUpdateRequest.CountryID;
            Matchingperson.Address = personUpdateRequest.Address;
            Matchingperson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            await _personRepository.UpdatePerson(Matchingperson);  //Update the person details in the list

            return Matchingperson.ToPersonResponse() ;
            }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if(personID == null)
                throw new ArgumentNullException(nameof(personID), "Person ID cannot be null.");

          Person? person = await _personRepository.GetPersonsByPersonID(personID.Value);
            if (person == null)
                    return false;

            await _personRepository.DeletePersonByPersonID(personID.Value);
             
            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
           MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
       
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfig);


            //PersonName,Email,DateOfBirth,Age,Gender,CountryID,Address,ReceiveNewsLetters
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.CountryID));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
          

            csvWriter.NextRecord();
            
            List<PersonResponse> persones =await GetAllPersons();
          await  csvWriter.WriteRecordsAsync(persones);


            foreach (PersonResponse person in persones)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth != null)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                else
                    csvWriter.WriteField(string.Empty);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();

            }
                memoryStream.Position = 0;
            return memoryStream;

        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
           MemoryStream memoryStream = new MemoryStream();
            //Excel generation logic using a library like EPPlus or ClosedXML
            using (ExcelPackage excelpackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelpackage.Workbook.Worksheets.Add("PersonsSheet");
                // Add headers
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date Of Birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Country";
                worksheet.Cells["G1"].Value = "Address";
                worksheet.Cells["H1"].Value = "Recive News Letters";

                using (ExcelRange headerRange = worksheet.Cells["A1:H1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }
                int row = 2;
                List<PersonResponse> persones =await GetAllPersons();

                foreach (PersonResponse person in persones)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth != null)
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    else
                        worksheet.Cells[row, 3].Value = string.Empty;
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Country;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }
                worksheet.Cells[$"A1:H{row}"].AutoFitColumns(); 

                await excelpackage.SaveAsAsync(memoryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
