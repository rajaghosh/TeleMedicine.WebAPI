using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Telemedicine.Service.Managers;
using Telemedicine.Service.Models;
#if NETCOREAPP2_0
using AuthorizeNet.Utilities;
#endif
using BridgeClassLibrary;
using net.authorize.sample;

namespace TelemedicineTest
{
    [TestClass]
    public class UnitTest1 : SampleCode
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public async Task CheckPayment()
        {
            Survey surveyObj = new Survey();
            PaymentRequest paymentRequest = new PaymentRequest();

            paymentRequest.PatientID = 1;
            paymentRequest.SubmissionID = 2;
            paymentRequest.Amount = 1000.00F;
            paymentRequest.FirstName = "Test Customer First Name";
            paymentRequest.LastName = "Test Customer Last Name";
            paymentRequest.Email = "TestCustomer@gmail.com";
            paymentRequest.Phone = "";
            paymentRequest.Address = "Addr1";
            paymentRequest.City = "Test City";
            paymentRequest.State = "Test State";
            paymentRequest.Zip = "";
            paymentRequest.ExpMonth = 6;
            paymentRequest.ExpYear = 2022;
            paymentRequest.CardToken = "";
            paymentRequest.TransactionId = "12345";
            paymentRequest.TransactionDescription = "This is a test transaction";

            var data = await surveyObj.MakePayment(paymentRequest);
            Assert.IsFalse(data.Result, "1 should not be prime");
        }


        [TestMethod]
        public void CallDirectCheckPayment()
        {
            Survey surveyObj = new Survey();
            PaymentRequest paymentRequest = new PaymentRequest();

            //string ApiLoginID = "5KP3u95bQpv";
            //string ApiTransactionKey = "346HZ32z3fP4hTG2";
            //decimal amount = 12.34M;
            string transId = "";
            string status = "";

            const string apiLoginId = "5KP3u95bQpv";
            const string transactionKey = "346HZ32z3fP4hTG2";

            //Update TransactionID for which you want to run the sample code
            const string transactionId = "2249735976";

            //Update PayerID for which you want to run the sample code
            const string payerId = "M8R9JRNJ3R28Y";

            const string customerProfileId = "1915435550"; //"213213";
            const string customerPaymentProfileId = "1828811149"; //"2132345";

            const string shippingAddressId = "1223213";
            const decimal amount = 12.34m;
            const string subscriptionId = "1223213";
            const short day = 45;
            const string emailId = "test@test.com";


            try
            {
                CallNetAuthorize callNet = new CallNetAuthorize();
                callNet.CheckPayment("123", "456", 12.34M);
                
                //RunMethod("ChargeCreditCard");
                //net.authorize.sample.ChargeCreditCard.Run("5KP3u95bQpv", "346HZ32z3fP4hTG2", amount);
                //ChargeCreditCard.Run("5KP3u95bQpv", "346HZ32z3fP4hTG2", amount, out transId);
                //ChargeCreditCard.Run(apiLoginId, transactionKey, amount, out transId);
                //var response = ChargeCreditCard.Run(ApiLoginID, ApiTransactionKey, amount, out transId);
                status = "GOOD";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }

            Console.WriteLine(status);
            var data = true; //await surveyObj.MakePayment(paymentRequest);
            Assert.IsFalse(data, "1 should not be prime");
        }

    }
}