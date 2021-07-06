using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Authorize.Dot.Net.Core.PaymentGatewayModel
{
    class PaymentModels
    {

    }

    public class GatewayPaymentRequest
    {
        public int PatientID { get; set; }
        public int SubmissionID { get; set; }
        public string CardNumber { get; set; }
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
}
