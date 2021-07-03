using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Service.Managers;
using Telemedicine.Service.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeleMedicine.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        readonly Admin admin = new Admin();
        [HttpPost]
        public async Task<List<PatiantInformation>> GetPatientsList(ReportRequest reportRequest)
        {
            return await admin.GetPatientsList(reportRequest);
        }

        [HttpPost]
        public async Task<bool> IsValidUser(LoginDetails loginDetails)
        {
            return await admin.IsValidUser(loginDetails);
        }

        [HttpPost]
        public async Task<bool> AddUserLoginDetails(LoginDetails loginDetails)
        {
            return await admin.AddUserLoginDetails(loginDetails);
        }

        [HttpPost]
        public async Task<int> UpdateLoginPassword(LoginDetails loginDetails)
        {
            return await admin.UpdateLoginPassword(loginDetails);
        }

        [HttpPost]
        public async Task<int> UpdatePatiantDetails(PatiantInformation PatiantInformation)
        {
            return await admin.UpdatePatiantDetails(PatiantInformation);
        }

        [HttpPost]
        public async Task<List<SurveyStep>> GetAnswers(PatiantInformation PatiantInformation)
        {
            return await admin.getAnswers(PatiantInformation.SubmissionId);
        }

        [HttpPost]
        public async Task<Config> GetConfig()
        {
            Survey survey = new Survey();
            return await survey.getConfig();
        }

        [HttpGet]
        public async Task<List<LoginDetails>> GetUserDetails(string sortingColumn = "userId")
        {
            return await admin.GetUserDetails(sortingColumn);
        }

        [HttpPost]
        public async Task<int> UpdateUserDetails(LoginDetails loginDetails)
        {
            return await admin.UpdateUserDetails(loginDetails);
        }


        [HttpPost]
        public async Task<List<ActivityLog>> GetActivityLogs(ReportRequest reportRequest)
        {
            return await admin.GetActivityLogs(reportRequest);
        }

        [HttpPost]
        public async Task<bool> LogOut(LoginDetails loginDetails)
        {
            ActivityLog activityLog = new ActivityLog()
            {
                LoginDetails = loginDetails,
                Activity = "User Logged out successfully",
                Details = ""
            };
            return await admin.InsertActivityLog(activityLog);
        }

        [HttpPost]
        public async Task<bool> IsUserNameExist(LoginDetails loginDetails)
        {
            return await admin.IsUserNameExist(loginDetails);
        }

        [HttpPost]
        public async Task<List<PharmacyDetails>> GetPharmacyDetailsList()
        {
            return await admin.GetPharmacyDetailsList();
        }

        [HttpPost]
        public async Task<bool> UpdatePharmacyInSubmissionDetails(PharmacyDetails pharmacyDetails, int submissionId)
        {
            return await admin.UpdatePharmacyInSubmissionDetails(pharmacyDetails, submissionId);
        }

        [HttpPost]
        public async Task<Escribe> GetEscibeDetails(int submissionId)
        {
            return await admin.GetEscibeDetails(submissionId);
        }
        
        [HttpPost]
        public async Task<List<MedicineDetails>> GetMedicines(int sequenceId)
        {
            return await admin.GetMedicineDetails(sequenceId);
        }

        [HttpPost]
        public async Task<bool> InsertMedicineDetails(List<MedicineDetails> medicineDetails, int userId)
        {
            return await admin.InsertMedicineDetails(medicineDetails, userId);
        }

        [HttpPost]
        public async Task<List<MedicineDetails>> GetPriscriptionDetails(int submissionId)
        {
            return await admin.GetPriscription(submissionId);
        }

        [HttpPost]
        public async Task<List<Refill>> GetRefills(SearchFilter search)
        {
            return await admin.GetRefills(search);
        }

        [HttpPost]
        public async Task<List<StatusDetails>> GetStatusDetails()
        {
            return await admin.GetStatus();
        }

        [HttpPost]
        public async Task<List<Referrals>> getReferrals(SearchFilter search)
        {
            return await admin.GetReferrals(search);
        }
    }
}
