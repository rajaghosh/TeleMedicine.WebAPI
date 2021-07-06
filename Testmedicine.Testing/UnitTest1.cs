using Microsoft.VisualStudio.TestTools.UnitTesting;
//using net.authorize.sample;
using Telemedicine.Service.TelemedicineGateway;
using Telemedicine.Service.Managers;
using Telemedicine.Service.Models;
using System.Threading.Tasks;

namespace Testmedicine.Testing
{
    [TestClass]
    public class UnitTest1 //: SampleCode
    {
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    RunMethod("ChargeCreditCard");
        //}

        [TestMethod]
        public void TestMakePaymentFromSurvey()
        {
            string testData = ""; // 12.34M;
            const string apiLoginId = "5KP3u95bQpv";
            const string transactionKey = "346HZ32z3fP4hTG2";
            //TelemedicineChargeCreditCard telemed = new TelemedicineChargeCreditCard();
            TelemedicineChargeCreditCard.Run(apiLoginId, transactionKey, 12.34M, out testData);
        }

        [TestMethod]
        public async Task TestMakePaymentAsync()
        {
            PaymentRequest payment = new PaymentRequest();
            payment.PatientID = 1001;
            payment.SubmissionID = 2001;
            payment.AccountName = "TestPatientAccName";
            payment.AccountType = "PatientAccount";
            payment.AccountCode = "PatCod001";
            payment.Amount = 1050.00F;
            payment.FirstName = "TestPatientFN";
            payment.LastName = "TestPatientLN";
            payment.Address = "TestAddress";
            payment.City = "City1";
            payment.Zip = "123456";
            payment.CardToken = "4111111111111111";
            payment.ExpMonth = 10;
            payment.ExpYear = 28;
            payment.Email = "transaction@testmail.com";
            payment.Phone = "12234567890";
            payment.DOB = "01/01/2000";
            payment.Gender = "Male";
            payment.Reference = "Test Reference";
            //payment.TransactionId;
            payment.TransactionDescription = "ChargeCreditCard";
            payment.AfterPaymentInstructions = "CreditCardCharged";


            Survey surveyObj = new Survey();
            var response = await surveyObj.MakePayment(payment);

            Assert.IsNull("False");
            Assert.IsNotNull("True");
        }

    }
}
