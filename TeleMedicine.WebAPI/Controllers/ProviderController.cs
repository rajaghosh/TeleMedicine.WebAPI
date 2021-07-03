using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Service.Models;
using Telemedicine.Service.Managers;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeleMedicine.WebAPI.Controllers
{ 
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProviderController : Controller
    {
        ProviderService providerService = new ProviderService();
        AzureUploads azureUploads;
        CommonFunctions commonFunctions;

        [HttpPost]
        public async Task<ProviderInformation> GetProviderInformation(UserDetails userDetails)
        {
            return await providerService.GetProviderInformation(userDetails);
        }
        [HttpPost]
        public async Task<List<DEARegistration>> GetDEARegistration(UserDetails userDetails)
        {
            return await providerService.GetDEARegistration(userDetails);
        }
        [HttpPost]
        public async Task<List<LicenseRegistration>> GetLicenseRegistration(UserDetails userDetails)
        {
            return await providerService.GetLicenseRegistration(userDetails);
        }
        [HttpPost]
        public async Task<List<ProfessionalReferences>> GetProfessionalReferences(UserDetails userDetails)
        {
            return await providerService.GetProfessionalReferences(userDetails);
        }
        [HttpPost]
        public async Task<bool> UpdateProviderInformation(ProviderInformation providerInformation)
        {
            bool ispovider, isDeaRegistion, isPrfessionalRef, isLicence, isAllSuccess;

            ispovider = await providerService.UpdateProviderInformation(providerInformation);

            isDeaRegistion = await UpdateDEARegistration(providerInformation.DEARegistrationsList);

            isLicence = await UpdateLicenseRegistration(providerInformation.LicenseRegistrationList);

            isPrfessionalRef = await UpdateProfessionalReferences(providerInformation.ProfessionalReferencesList);

            isAllSuccess = ispovider && isDeaRegistion && isPrfessionalRef && isLicence;


            return isAllSuccess;
        }
        [HttpPost]
        public async Task<bool> UpdateDEARegistration(List<DEARegistration> listDEARegistration)
        { 
            bool result = false;
            foreach (DEARegistration dEARegistration in listDEARegistration)
            {
                if (dEARegistration.ExpirationDate == null)
                {
                    //continue;
                    dEARegistration.ExpirationDate = new DateTime();
                }
                result = await providerService.UpdateDEARegistration(dEARegistration);
            }
            return result;
        }

        [HttpPost]
        public async Task<bool> UpdateLicenseRegistration(List<LicenseRegistration> listLicenseRegistration)
        { 
            bool result = false;
            foreach (LicenseRegistration licenseRegistration in listLicenseRegistration)
            {
                if (licenseRegistration.LicenseExpirationDate == null)
                {
                    //continue;
                    licenseRegistration.LicenseExpirationDate = new DateTime();
                }
                result = await providerService.UpdateLicenseRegistration(licenseRegistration);
            }
            return result;
        }

        [HttpPost]
        public async Task<bool> UpdateProfessionalReferences(List<ProfessionalReferences> listProfessionalReferences)
        {
            bool result = false;
            foreach (ProfessionalReferences professionalReferences in listProfessionalReferences)
            { 
                result = await providerService.UpdateProfessionalReferences(professionalReferences);
            }
            return result;
        }
          
        [HttpPost]
        async public Task<IActionResult> UploadImage(List<ImagData> objImageData, string userId)
        {
            string fileName;
            string[] result = new string[2];
            try
            {
                foreach (ImagData item in objImageData)
                {
                    if (item.Data != "")
                    {
                        //Code added by joshua to upload image to azure cloud storage on July-10-2019
                        fileName = userId.ToString() + "_signature.jpg";
                        commonFunctions = new CommonFunctions();
                        azureUploads = new AzureUploads
                        {
                            FolderPath = "Telemedicine\\Esign\\",
                            UserId = userId,
                            ESignatureData = Convert.FromBase64String(item.Data),
                            FileName = fileName,
                            IsImageFile = false
                        };
                        result = await commonFunctions.AzureUploads(azureUploads); 
                    }
                }
                var data = Json(new
                {
                    name = result[0],
                    url = result[1]
                });
                return Ok(data);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            } 
        }       

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromBody]IFormFile files)
        { 
            commonFunctions = new CommonFunctions();
            azureUploads = new AzureUploads
            {
                FolderPath = "Telemedicine\\Document\\",
                UserId = "", 
                IformFile = files,
                IsImageFile = false
            };
            string[] result = await commonFunctions.AzureUploads(azureUploads);
            var data = Json(new
            {
                name = result[0],
                url = result[1]
            });
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> upload()
        {
            var file = Request.Form.Files;

            commonFunctions = new CommonFunctions();
            azureUploads = new AzureUploads
            {
                FolderPath = "Telemedicine\\Document\\",
                UserId = "",
                IformFile = file,
                IsImageFile = false
            };
            string[] result = await commonFunctions.AzureUploads(azureUploads);
            var data = Json(new
            {
                name = result[0],
                url = result[1]
            });
            return Ok(data);
        }
    }
}
