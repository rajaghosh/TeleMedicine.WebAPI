using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Telemedicine.Service.Models
{

    public class SurveyStep
    {
        public int StepID { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int SortOrder { get; set; }
        public List<SurveyQuestion> Questions { get; set; }
    }

    public class SurveyQuestion
    {
        public int StepID { get; set; }
        public int QuestionID { get; set; }
        public string Question { get; set; }
        public string Hint { get; set; }
        public string RegEx { get; set; }
        public int ItemsInRow { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; }
        public List<SurveyOption> Options { get; set; }
        public string Answer { get; set; }
    }

    public class SurveyOption
    {
        public int StepID { get; set; }
        public int QuestionID { get; set; }
        public int OptionID { get; set; }
        public int ItemsInRow { get; set; }
        public string OptionValue { get; set; }
    }

    public class SurveyAnswer
    {
        public int SubmissionID { get; set; }
        public int QuestionID { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
    }

    public class Origin
    {
        public string origin { get; set; }
    }

    public class IFrameKey
    {
        public string tokenExId { get; set; }
        public string key { get; set; }
        public string timestamp { get; set; }
    }

    public class PatiantInformation
    {
        public int PatientId { get; set; }
        public int SubmissionId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public int Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string DOBString { get; set; }
        public DateTime? PaymentTime { get; set; }
        public string AuthTranId { get; set; }
        public string Notes { get; set; }
        public bool IsCovid { get; set; }
        public string IsCovidPatient { get; set; }
        public double PaymentAmount { get; set; }
        public string ProfileDetails { get; set; }
        public LoginDetails LoginDetails { get; set; }
        public string Activities { get; set; }
        public string FollowupNotes { get; set; }
        public string ActivitiesDetails { get; set; }
        public int UserId { get; set; }
        public bool IsAssigneeChanged { get; set; }
        public string DoctorName { get; set; }
        public string PharmacyName { get; set; }
        public int PharmacySequenceId { get; set; }
        public  DateTime LastUpdatedDate { get; set; } 
        public string OTP { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Gender { get; set; }
        public bool IsReturningPatient { get; set; }
        public List<SurveyAnswer> Answers { get; set; }
        public bool IsPrescribed { get; set; }
        public bool IsCompleted { get; set; }
        public string PatientType { get; set; }
    } 


    public class Escribe
    {
        public PatiantInformation PatiantInformation { get; set; }
        public ProviderInformation ProviderInformation { get; set; } 
        public PharmacyDetails PharmacyDetails { get; set; }
        public string DEA { get; set; } 
        public string PrescribedMedicationDiscription { get; set; }
        public string PrescribedMedicationQTY { get; set; }
        public string PrescribedMedicationDirections { get; set; }
        public string PrescribedRefills { get; set; }
        public string DAWCode { get; set; }
        public string Notes { get; set; }
        public string DigitalSignature { get; set; }

        //PrescriberOrderNumber MessageID

        public string PrescriberOrderNumber { get; set; }
        public string MessageID { get; set; }
    }

    public class PaymentRequest
    {
        public int PatientID { get; set; }
        public int SubmissionID { get; set; }
        public string CardToken { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public float Amount { get; set; }
        public string Reference { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string AccountType { get; set; }
        public string TransactionId { get; set; }
        public string TransactionDescription { get; set; }
        public string AfterPaymentInstructions { get; set; }
    }

    public class LoginDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public bool IsActiveUser { get; set; }
        public bool IsAdmin { get; set; }
        public int AccessLevel { get; set; }
        public int AdminUserId { get; set; }
        public string AdminUserName { get; set; }
    }

    public class Config
    {
        public float AmountCovid { get; set; }
        public float AmountNonCovid { get; set; }
        public float AmountRefill { get; set; }
        public bool IsLive { get; set; }
        public string CovidQuestions { get; set; }
    }

    public class SecureConfig
    {
        public bool IsLive { get; set; }
        public string TokenExId { get; set; }
        public string TokenExApiKey { get; set; }
        public string TokenExCSKey { get; set; }
        public string AuthNetLogin { get; set; }
        public string AuthNetTransactionKey { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string InvoiceEmailContent { get; set; }
        public string OTPEmailContent { get; set; }
    }

    public class ReportRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAdmin { get; set; }
        public int UserId { get; set; }
        public bool IsSingleSubmission { get; set; }
        public int SubmissionId { get; set; }
        public string ReportType { get; set; }
    }

    public class ActivityLog
    {
        //HistoryDate, HistoryTime, UserId, UserName, Activity, Details, CreatedDate 
        public string HistoryDate { get; set; }
        public string HistoryTime { get; set; }
        public string Activity { get; set; }
        public string Details { get; set; }
        public DateTime CreatedDate { get; set; }
        public LoginDetails LoginDetails { get; set; }
    }

    public class EmailProperty
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
    }

    public class SearchFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAdmin { get; set; }
        public int UserId { get; set; }
    }

    public class AuthProperties
    {
        public bool IsVerifiedUser { get; set; }
        public LoginDetails LoginDetails { get; set; }
    }

    public class SurveyRule
    {
        public int StepID { get; set; }
        public int QuestionID { get; set; }
        public string Answer { get; set; }
        public int NextStepID { get; set; }
    }

    public class PaymentResult
    {
        public string TransactionId { get; set; }
        public string AuthTransId { get; set; }
        public bool Result { get; set; }
    }

    public class Refill : PatiantInformation
    {
        public int RefillId { get; set; }
        public DateTime? RefillDate { get; set; }
    }

    public class StatusDetails
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }

    public class Referrals
    {
        public int id { get; set; }
        public string ReferralName { get; set; }
        public string FullLink { get; set; }
        public string DateAccessed { get; set; }
        public int Ranking { get; set; }

        public int NoOfReferance { get; set; }
    }

    public class Rate
    {
        public string RateName { get; set; }
        public string TransactionType { get; set; }
        public string PaymentDescription { get; set; }
        public float Amount { get; set; }
        
    }


}
