using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Telemedicine.Service.Models
{
    public class UserDetails
    {
        public int UserId { get; set; }
        public string Mode { get; set; }
    }

    public class Address : UserDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initial { get; set; }
        public string MailingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }

    public class ProviderInformation : Address
    {
        public string ProfessionList { get; set; }
        public string PracticeAddress { get; set; }
        public string PracticeCity { get; set; }
        public string PracticeState { get; set; }
        public string PracticeZipCode { get; set; }
        public string PracticeCountry { get; set; }
        public string FaxNumber { get; set; }
        public string SocialSecurity { get; set; }
        public string Speciality { get; set; }
        public string NPI { get; set; }
        public string MedicareValidation { get; set; }
        public string Education { get; set; }
        public string NursingSchool { get; set; }
        public string NursingYearOfGraduation { get; set; }
        public string PgEducation { get; set; }
        public string PgYearOfGraduation { get; set; }
        public string IsBoardEligible { get; set; }
        public string IsBoardCertified { get; set; }
        public string BoardName { get; set; }
        public string CertifiedNumber { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string ContactPerson { get; set; }
        public string IsOwnPractice { get; set; }
        public string LanguageKnown { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DEARegistraionStatus { get; set; }
        public bool IsFullTimeJob { get; set; }
        public string Hours { get; set; }
        public string ScheduledDays { get; set; }
        public string PreferedTypeOfConsults { get; set; }
        public string ScheduleComment { get; set; }
        public string CVFileName { get; set; }
        public string OtherFileName { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string SignatureName { get; set; }
        public string IsDiciplinaryByOthers { get; set; }
        public string IsStateProfessionalLicense { get; set; }
        public string IsHospitalPrivilegesDenied { get; set; }
        public string IsPendingMalpractice { get; set; }
        public string IsConvictedVilationOfLaw { get; set; }
        public string IsProblemWithDrugAlcohol { get; set; }
        public string IsDeclinedanyInsuraceCompany { get; set; }
        public string IsPhysicalConditionFine { get; set; }
        public string ExplanationNotes { get; set; }

        public bool IsBlobExist { get; set; }
        public List<DEARegistration> DEARegistrationsList { get; set; }
        public List<LicenseRegistration> LicenseRegistrationList { get; set; }
        public List<ProfessionalReferences> ProfessionalReferencesList { get; set; }
    }

    public class ProfessionalReferences : Address
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class LicenseRegistration : UserDetails
    {
        public int Id { get; set; }
        public string LicenseState { get; set; }
        public string LicenseType { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpirationDate { get; set; }
        public bool IsActiveLicense { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class DEARegistration : UserDetails
    {
        public int Id { get; set; }
        public string DeaRegistrationNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
   
