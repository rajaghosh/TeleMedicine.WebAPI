using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
//using System.Threading.Tasks;
//using Telemedicine.Service;
//using Telemedicine.Service.Managers;
//using Telemedicine.Service.Models;

using net.authorize.sample;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1: SampleCode
    {
        [TestMethod]
        public void TestMethod1()
        {
        }


        [TestMethod]
        public void CallDirectCheckPayment()
        {
            //Survey surveyObj = new Survey();
            //PaymentRequest paymentRequest = new PaymentRequest();

            ////string ApiLoginID = "5KP3u95bQpv";
            ////string ApiTransactionKey = "346HZ32z3fP4hTG2";
            ////decimal amount = 12.34M;
            //string transId = "";
            string status = "";

            //const string apiLoginId = "5KP3u95bQpv";
            //const string transactionKey = "346HZ32z3fP4hTG2";

            ////Update TransactionID for which you want to run the sample code
            //const string transactionId = "2249735976";

            ////Update PayerID for which you want to run the sample code
            //const string payerId = "M8R9JRNJ3R28Y";

            //const string customerProfileId = "1915435550"; //"213213";
            //const string customerPaymentProfileId = "1828811149"; //"2132345";

            //const string shippingAddressId = "1223213";
            //const decimal amount = 12.34m;
            //const string subscriptionId = "1223213";
            //const short day = 45;
            //const string emailId = "test@test.com";


            try
            {
                RunMethod("ChargeCreditCard");
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
