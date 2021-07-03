using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Telemedicine.Service;
using Telemedicine.Service.Managers;
using Telemedicine.Service.Models;

namespace TelemedicineTest
{
    [TestClass]
    public class UnitTest1
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
        



            var data = await surveyObj.MakePayment(paymentRequest);
            //bool result = _primeService.IsPrime(1);
            bool result = true;
            Assert.IsFalse(result, "1 should not be prime");
        }
    }
}
