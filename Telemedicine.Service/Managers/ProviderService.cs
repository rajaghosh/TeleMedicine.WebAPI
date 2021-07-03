using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telemedicine.Service.DataConnection;
using Telemedicine.Service.Models;

namespace Telemedicine.Service.Managers
{
    public class ProviderService
    {
        DataConnection.DataProvider dataProvider = new DataConnection.DataProvider();
        readonly string PMSFolder = string.Empty;
        CommonFunctions commonFunctions;

        public async Task<ProviderInformation> GetProviderInformation(UserDetails userDetails)
        {
            ProviderInformation providerInformation = new ProviderInformation();
            string sqlQuery = "select * from provider_basic_info where user_id='{0}'";
            var dataTable = await dataProvider.GetDataTable(PMSFolder, string.Format(sqlQuery, userDetails.UserId)); 
            foreach (DataRow rw in dataTable.Rows)
            {  
                providerInformation.UserId = rw["user_id"] == DBNull.Value ? 0 : (int)rw["user_id"];
                providerInformation.ProfessionList= rw["profession_list"] == DBNull.Value ? null : (string)rw["profession_list"];

                //var test = JsonConvert.DeserializeObject<object>(providerInformation.ProfessionList);

                providerInformation.FirstName= rw["first_name"] == DBNull.Value ? null : (string)rw["first_name"];
                providerInformation.Initial = rw["initial"] == DBNull.Value ? null : (string)rw["initial"];
                providerInformation.LastName= rw["last_name"] == DBNull.Value ? null : (string)rw["last_name"];
                providerInformation.MailingAddress= rw["mailing_address"] == DBNull.Value ? null : (string)rw["mailing_address"];

                providerInformation.City= rw["city"] == DBNull.Value ? null : (string)rw["city"];
                providerInformation.State= rw["state"] == DBNull.Value ? null : (string)rw["state"];
                providerInformation.ZipCode= rw["zipcode"] == DBNull.Value ? null : (string)rw["zipcode"];
                providerInformation.Country= rw["country"] == DBNull.Value ? null : (string)rw["country"];
                providerInformation.PracticeAddress= rw["practice_address"] == DBNull.Value ? null : (string)rw["practice_address"];

                providerInformation.PracticeCity= rw["practice_city"] == DBNull.Value ? null : (string)rw["practice_city"];
                providerInformation.PracticeState= rw["practice_state"] == DBNull.Value ? null : (string)rw["practice_state"];
                providerInformation.PracticeZipCode= rw["practice_zipcode"] == DBNull.Value ? null : (string)rw["practice_zipcode"];
                providerInformation.PracticeCountry= rw["practice_country"] == DBNull.Value ? null : (string)rw["practice_country"];
                providerInformation.Email= rw["email"] == DBNull.Value ? null : (string)rw["email"];

                providerInformation.PhoneNumber= rw["phone_number"] == DBNull.Value ? null : (string)rw["phone_number"];
                providerInformation.MobileNumber= rw["mobile_number"] == DBNull.Value ? null : (string)rw["mobile_number"];
                providerInformation.FaxNumber= rw["fax_number"] == DBNull.Value ? null : (string)rw["fax_number"];
                providerInformation.SocialSecurity= rw["social_security"] == DBNull.Value ? null : (string)rw["social_security"];
                providerInformation.DOB= (DateTime)rw["dob"];

                providerInformation.Speciality= rw["speciality"] == DBNull.Value ? null : (string)rw["speciality"];
                providerInformation.NPI= rw["npi"] == DBNull.Value ? null : (string)rw["npi"];
                providerInformation.MedicareValidation= rw["medicare_validation"] == DBNull.Value ? "" : (string)rw["medicare_validation"];
                providerInformation.Education= rw["education"] == DBNull.Value ? null : (string)rw["education"];
                providerInformation.NursingSchool= rw["nursing_school"] == DBNull.Value ? null : (string)rw["nursing_school"];

                providerInformation.NursingYearOfGraduation= rw["nur_yearof_graduation"] == DBNull.Value ? null : (string)rw["nur_yearof_graduation"];
                providerInformation.PgEducation= rw["pg_education"] == DBNull.Value ? null : (string)rw["pg_education"];
                providerInformation.PgYearOfGraduation= rw["pg_yearof_graduation"] == DBNull.Value ? null : (string)rw["pg_yearof_graduation"];
                providerInformation.IsBoardEligible= rw["isboard_eligible"] == DBNull.Value ? "" : (string)rw["isboard_eligible"];
                providerInformation.IsBoardCertified= rw["isboard_certified"] == DBNull.Value ? "" : (string)rw["isboard_certified"];

                providerInformation.BoardName= rw["board_name"] == DBNull.Value ? null : (string)rw["board_name"];
                providerInformation.CertifiedNumber= rw["certified_number"] == DBNull.Value ? null : (string)rw["certified_number"];
                providerInformation.Gender= rw["gender"] == DBNull.Value ? "" : (string)rw["gender"];
                providerInformation.ContactPerson= rw["contact_person"] == DBNull.Value ? "" : (string)rw["contact_person"];
                providerInformation.IsOwnPractice= rw["isown_practice"] == DBNull.Value ? "" : (string)rw["isown_practice"];

                providerInformation.LanguageKnown= rw["language_known"] == DBNull.Value ? null : (string)rw["language_known"];
                providerInformation.CreatedDate= (DateTime)rw["created_date"];
                providerInformation.DEARegistraionStatus= rw["dea_registration_status"] == DBNull.Value ? null : (string)rw["dea_registration_status"];
                providerInformation.IsFullTimeJob= rw["isfull_time_job"] == DBNull.Value ? false : (bool)rw["isfull_time_job"];
                providerInformation.Hours= rw["hours"] == DBNull.Value ? null : (string)rw["hours"];

                providerInformation.ScheduledDays= rw["scheduled_days"] == DBNull.Value ? null : (string)rw["scheduled_days"];
                providerInformation.PreferedTypeOfConsults= rw["prefered_typeof_consults"] == DBNull.Value ? null : (string)rw["prefered_typeof_consults"];
                providerInformation.ScheduleComment= rw["schedule_comment"] == DBNull.Value ? null : (string)rw["schedule_comment"];
                providerInformation.CVFileName= rw["cv_file_name"] == DBNull.Value ? null : (string)rw["cv_file_name"];
                providerInformation.OtherFileName= rw["other_file_name"] == DBNull.Value ? null : (string)rw["other_file_name"];

                providerInformation.SubmittedDate= (DateTime)rw["submitted_date"];
                providerInformation.SignatureName= rw["signature_name"] == DBNull.Value ? null : (string)rw["signature_name"];
                providerInformation.IsDiciplinaryByOthers= rw["isdisciplinaryby_others"] == DBNull.Value ? null : (string)rw["isdisciplinaryby_others"];
                providerInformation.IsStateProfessionalLicense= rw["isstate_professional_license"] == DBNull.Value ? null : (string)rw["isstate_professional_license"];
                providerInformation.IsHospitalPrivilegesDenied= rw["ishospital_privileges_denied"] == DBNull.Value ? null : (string)rw["ishospital_privileges_denied"];

                providerInformation.IsPendingMalpractice= rw["ispending_malpractice"] == DBNull.Value ? null : (string)rw["ispending_malpractice"];
                providerInformation.IsConvictedVilationOfLaw= rw["isconvicted_vilationof_law"] == DBNull.Value ? null : (string)rw["isconvicted_vilationof_law"];
                providerInformation.IsProblemWithDrugAlcohol= rw["isproblemwith_drug_alcohol"] == DBNull.Value ? null : (string)rw["isproblemwith_drug_alcohol"];
                providerInformation.IsDeclinedanyInsuraceCompany= rw["isdeclinedany_insurace_company"] == DBNull.Value ? null : (string)rw["isdeclinedany_insurace_company"];
                providerInformation.IsPhysicalConditionFine= rw["isphysical_condition_fine"] == DBNull.Value ? null : (string)rw["isphysical_condition_fine"];
                providerInformation.ExplanationNotes = rw["explanation_notes"] == DBNull.Value ? null : (string)rw["explanation_notes"];
            }

            if(dataTable.Rows.Count > 0)
            {
                providerInformation.Mode = "Update";
                commonFunctions = new CommonFunctions();
                providerInformation.IsBlobExist = await commonFunctions.IsBlobExist("Telemedicine\\Esign\\" + userDetails.UserId + "_signature.jpg");
            }
            else
            {
                providerInformation.Mode = "Insert";
                providerInformation.UserId = userDetails.UserId;
                providerInformation.IsBlobExist = false;
            }
            
            providerInformation.DEARegistrationsList = await GetDEARegistration(userDetails);
            providerInformation.LicenseRegistrationList = await GetLicenseRegistration(userDetails);
            providerInformation.ProfessionalReferencesList = await GetProfessionalReferences(userDetails);
            return providerInformation;
        }
        public async Task<List<DEARegistration>> GetDEARegistration(UserDetails userDetails)
        {
            var listDEARegistration = new List<DEARegistration>();
            var dataTable = await dataProvider.GetDataTable(PMSFolder, "sp_dea_registration",
               new DataProvider.Parameter("@user_id", userDetails.UserId), 
               new DataProvider.Parameter("@mode", userDetails.Mode));

            listDEARegistration = (from rw in dataTable.AsEnumerable()
                                   select new DEARegistration()
                                   {
                                        Id = rw["id"] == DBNull.Value ? 0 : (int)rw["id"],
                                        UserId = rw["user_id"] == DBNull.Value ? 0 : (int)rw["user_id"],
                                        DeaRegistrationNumber = rw["dea_number"] == DBNull.Value ? null : (string)rw["dea_number"],
                                        ExpirationDate =  (DateTime)rw["expiration_date"],
                                        CreatedDate = (DateTime)rw["created_date"],
                                        Mode = "Update"
                                   }).ToList();
            
            return listDEARegistration;
        }
        public async Task<List<LicenseRegistration>> GetLicenseRegistration(UserDetails userDetails)
        {
            var listLicenseRegistration = new List<LicenseRegistration>(); 
            var dataTable = await dataProvider.GetDataTable(PMSFolder, "sp_license_registration",
               new DataProvider.Parameter("@user_id", userDetails.UserId),
               new DataProvider.Parameter("@mode", userDetails.Mode));

            listLicenseRegistration = (from rw in dataTable.AsEnumerable()
                                       select new LicenseRegistration()
                                       {
                                           Id = rw["id"] == DBNull.Value ? 0 : (int)rw["id"],
                                           UserId = rw["user_id"] == DBNull.Value ? 0 : (int)rw["user_id"],
                                           LicenseState = rw["license_state"] == DBNull.Value ? null : (string)rw["license_state"],
                                           LicenseType = rw["license_type"] == DBNull.Value ? null : (string)rw["license_type"],
                                           LicenseNumber = rw["license_number"] == DBNull.Value ? null : (string)rw["license_number"],
                                           LicenseExpirationDate = (DateTime)rw["license_expiration_date"],
                                           IsActiveLicense = rw["isactive_licenses"] == DBNull.Value ? false : (bool)rw["isactive_licenses"],
                                           CreatedDate = (DateTime)rw["created_date"],
                                           Mode = "Update"
                                       }).ToList();

            return listLicenseRegistration; 
        }
        public async Task<List<ProfessionalReferences>> GetProfessionalReferences(UserDetails userDetails)
        {
            var listProfessionalReferences = new List<ProfessionalReferences>(); 
            var dataTable = await dataProvider.GetDataTable(PMSFolder, "sp_professional_references",
               new DataProvider.Parameter("@user_id", userDetails.UserId),
               new DataProvider.Parameter("@mode", userDetails.Mode));

            listProfessionalReferences = (from rw in dataTable.AsEnumerable()
                                         select new ProfessionalReferences()
                                         {
                                            Id = rw["id"] == DBNull.Value ? 0 : (int)rw["id"],
                                            UserId = rw["user_id"] == DBNull.Value ? 0 : (int)rw["user_id"],
                                            FirstName = rw["name"] == DBNull.Value ? null : (string)rw["name"],
                                            PhoneNumber = rw["phone_number"] == DBNull.Value ? null : (string)rw["phone_number"],
                                            Email = rw["email"] == DBNull.Value ? null : (string)rw["email"],
                                            MailingAddress = rw["address"] == DBNull.Value ? null : (string)rw["address"],
                                            City = rw["city"] == DBNull.Value ? null : (string)rw["city"],
                                            State = rw["state"] == DBNull.Value ? null : (string)rw["state"],
                                            ZipCode = rw["zipcode"] == DBNull.Value ? null : (string)rw["zipcode"],
                                            Country = rw["country"] == DBNull.Value ? null : (string)rw["country"],
                                            CreatedDate = (DateTime)rw["created_date"],
                                             Mode = "Update"
                                         }).ToList();

            return listProfessionalReferences;
        }
        public async Task<bool> UpdateProviderInformation(ProviderInformation providerInformation)
        {
            providerInformation.SubmittedDate = providerInformation.SubmittedDate.AddDays(1);
            providerInformation.DOB = providerInformation.DOB.AddDays(1);

            return await dataProvider.ExecuteSP(PMSFolder, "sp_provider_basic_info",
                new DataProvider.Parameter("@user_id", providerInformation.UserId),
                new DataProvider.Parameter("@profession_list", providerInformation.ProfessionList ?? ""),
                new DataProvider.Parameter("@first_name", providerInformation.FirstName ?? ""),
                new DataProvider.Parameter("@last_name", providerInformation.LastName ?? ""),
                new DataProvider.Parameter("@initial", providerInformation.Initial ?? ""),
                new DataProvider.Parameter("@mailing_address", providerInformation.MailingAddress ?? ""),

                new DataProvider.Parameter("@city", providerInformation.City ?? ""),
                new DataProvider.Parameter("@state", providerInformation.State ?? ""),
                new DataProvider.Parameter("@zipcode", providerInformation.ZipCode ?? ""),
                new DataProvider.Parameter("@country", providerInformation.Country ?? ""),
                new DataProvider.Parameter("@practice_address", providerInformation.PracticeAddress ?? ""),

                new DataProvider.Parameter("@practice_city", providerInformation.PracticeCity ?? ""),
                new DataProvider.Parameter("@practice_state", providerInformation.PracticeState ?? ""),
                new DataProvider.Parameter("@practice_zipcode", providerInformation.PracticeZipCode ?? ""),
                new DataProvider.Parameter("@practice_country", providerInformation.PracticeCountry ?? ""),
                new DataProvider.Parameter("@email", providerInformation.Email ?? ""),

                new DataProvider.Parameter("@phone_number", providerInformation.PhoneNumber ?? ""),
                new DataProvider.Parameter("@mobile_number", providerInformation.MobileNumber ?? ""),
                new DataProvider.Parameter("@fax_number", providerInformation.FaxNumber ?? ""),
                new DataProvider.Parameter("@social_security", providerInformation.SocialSecurity ?? ""),
                new DataProvider.Parameter("@dob", providerInformation.DOB),

                new DataProvider.Parameter("@speciality", providerInformation.Speciality?? ""),
                new DataProvider.Parameter("@npi", providerInformation.NPI ?? ""),
                new DataProvider.Parameter("@medicare_validation", providerInformation.MedicareValidation ?? ""),
                new DataProvider.Parameter("@education", providerInformation.Education ?? ""),
                new DataProvider.Parameter("@nursing_school", providerInformation.NursingSchool ?? ""),

                new DataProvider.Parameter("@nur_yearof_graduation", providerInformation.NursingYearOfGraduation ?? ""),
                new DataProvider.Parameter("@pg_education", providerInformation.PgEducation ?? ""),
                new DataProvider.Parameter("@pg_yearof_graduation", providerInformation.PgYearOfGraduation ?? ""),
                new DataProvider.Parameter("@isboard_eligible", providerInformation.IsBoardEligible ?? ""),
                new DataProvider.Parameter("@isboard_certified", providerInformation.IsBoardCertified ?? ""),

                new DataProvider.Parameter("@board_name", providerInformation.BoardName ?? ""),
                new DataProvider.Parameter("@certified_number", providerInformation.CertifiedNumber ?? ""),
                new DataProvider.Parameter("@gender", providerInformation.Gender ?? "male"),
                new DataProvider.Parameter("@contact_person", providerInformation.ContactPerson ?? ""),
                new DataProvider.Parameter("@isown_practice", providerInformation.IsOwnPractice ?? ""),

                new DataProvider.Parameter("@language_known", providerInformation.LanguageKnown ?? ""),
                new DataProvider.Parameter("@created_date", providerInformation.CreatedDate),
                new DataProvider.Parameter("@dea_registration_status", providerInformation.DEARegistraionStatus ?? ""),
                new DataProvider.Parameter("@isfull_time_job", providerInformation.IsFullTimeJob),
                new DataProvider.Parameter("@hours", providerInformation.Hours ?? ""),

                new DataProvider.Parameter("@scheduled_days", providerInformation.ScheduledDays ?? ""),
                new DataProvider.Parameter("@prefered_typeof_consults", providerInformation.PreferedTypeOfConsults ?? ""),
                new DataProvider.Parameter("@schedule_comment", providerInformation.ScheduleComment ?? ""),
                new DataProvider.Parameter("@cv_file_name", providerInformation.CVFileName ?? ""),
                new DataProvider.Parameter("@other_file_name", providerInformation.OtherFileName ?? ""),

                new DataProvider.Parameter("@submitted_date", providerInformation.SubmittedDate),
                new DataProvider.Parameter("@signature_name", providerInformation.SignatureName ?? ""),
                new DataProvider.Parameter("@isdisciplinaryby_others", providerInformation.IsDiciplinaryByOthers ?? ""),
                new DataProvider.Parameter("@isstate_professional_license", providerInformation.IsStateProfessionalLicense ?? ""),
                new DataProvider.Parameter("@ishospital_privileges_denied", providerInformation.IsHospitalPrivilegesDenied ?? ""),

                new DataProvider.Parameter("@ispending_malpractice", providerInformation.IsPendingMalpractice ?? ""),
                new DataProvider.Parameter("@isconvicted_vilationof_law", providerInformation.IsConvictedVilationOfLaw ?? ""),
                new DataProvider.Parameter("@isproblemwith_drug_alcohol", providerInformation.IsProblemWithDrugAlcohol ?? ""),
                new DataProvider.Parameter("@isdeclinedany_insurace_company", providerInformation.IsDeclinedanyInsuraceCompany ?? ""),
                new DataProvider.Parameter("@isphysical_condition_fine", providerInformation.IsPhysicalConditionFine ?? ""),

                new DataProvider.Parameter("@mode", providerInformation.Mode ?? ""),
                new DataProvider.Parameter("@explanation_notes", providerInformation.ExplanationNotes ?? "")
                );
        }
        public async Task<bool> UpdateDEARegistration(DEARegistration dEARegistration)
        {
            dEARegistration.ExpirationDate = dEARegistration.ExpirationDate.AddDays(1);
            return await dataProvider.ExecuteSP(PMSFolder, "sp_dea_registration",
               new DataProvider.Parameter("@id", dEARegistration.Id),
               new DataProvider.Parameter("@user_id", dEARegistration.UserId),
               new DataProvider.Parameter("@dea_number", dEARegistration.DeaRegistrationNumber ?? ""),
               new DataProvider.Parameter("@expiration_date", dEARegistration.ExpirationDate),
               new DataProvider.Parameter("@created_date", dEARegistration.CreatedDate),
               new DataProvider.Parameter("@mode", dEARegistration.Mode));
        }
        public async Task<bool> UpdateLicenseRegistration(LicenseRegistration licenseRegistration)
        {
            licenseRegistration.LicenseExpirationDate = licenseRegistration.LicenseExpirationDate.AddDays(1);
            return await dataProvider.ExecuteSP(PMSFolder, "sp_license_registration",
               new DataProvider.Parameter("@id", licenseRegistration.Id),
               new DataProvider.Parameter("@user_id", licenseRegistration.UserId),
               new DataProvider.Parameter("@license_state", licenseRegistration.LicenseState ?? ""),
               new DataProvider.Parameter("@license_type", licenseRegistration.LicenseType ?? ""),
               new DataProvider.Parameter("@license_number", licenseRegistration.LicenseNumber ?? ""),
               new DataProvider.Parameter("@license_expiration_date", licenseRegistration.LicenseExpirationDate),
               new DataProvider.Parameter("@isactive_licenses", licenseRegistration.IsActiveLicense),
               new DataProvider.Parameter("@created_date", licenseRegistration.CreatedDate),
               new DataProvider.Parameter("@mode", licenseRegistration.Mode));
        }
        public async Task<bool> UpdateProfessionalReferences(ProfessionalReferences professionalReferences)
        {
            return await dataProvider.ExecuteSP(PMSFolder, "sp_professional_references",
               new DataProvider.Parameter("@id", professionalReferences.Id),
               new DataProvider.Parameter("@user_id", professionalReferences.UserId),
               new DataProvider.Parameter("@name", professionalReferences.FirstName ?? ""),
               new DataProvider.Parameter("@phone_number", professionalReferences.PhoneNumber ?? ""),
               new DataProvider.Parameter("@email", professionalReferences.Email ?? ""),
               new DataProvider.Parameter("@address", professionalReferences.MailingAddress ?? ""),
               new DataProvider.Parameter("@city", professionalReferences.City ?? ""),
               new DataProvider.Parameter("@state", professionalReferences.State ?? ""),
               new DataProvider.Parameter("@zipcode", professionalReferences.ZipCode ?? ""),
               new DataProvider.Parameter("@country", professionalReferences.Country ?? ""),
               new DataProvider.Parameter("@created_date", professionalReferences.CreatedDate),
               new DataProvider.Parameter("@mode", professionalReferences.Mode ?? ""));
        }
    }   
}
