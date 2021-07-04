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

        //[TestMethod]
        //public void TestMakePaymentFromSurvey()
        //{
        //    string testData = ""; // 12.34M;
        //    const string apiLoginId = "5KP3u95bQpv";
        //    const string transactionKey = "346HZ32z3fP4hTG2";
        //    //TelemedicineChargeCreditCard telemed = new TelemedicineChargeCreditCard();
        //    TelemedicineChargeCreditCard.Run(apiLoginId, transactionKey, 12.34M, out testData);
        //}

        [TestMethod]
        public async Task TestMakePaymentAsync()
        {
            PaymentRequest payment = new PaymentRequest();

            Survey surveyObj = new Survey();
            var response = await surveyObj.MakePayment(payment);

            Assert.IsNull("False");
            Assert.IsNotNull("True");
        }

    }
}
