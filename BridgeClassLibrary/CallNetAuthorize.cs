using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.authorize.sample;

namespace BridgeClassLibrary
{
    public class CallNetAuthorize: SampleCode
    {
        public string ApiLoginId { get; set; }
        public string ApiTransactionKey { get; set; }
        public decimal Amount { get; set; }


        public void CheckPayment(string apiLoginId, string apiTransactionKey, decimal amount)
        {
            string status = "";
            try
            {
                //RunMethod("ChargeCreditCard");
                apiLoginId = "5KP3u95bQpv";
                apiTransactionKey = "346HZ32z3fP4hTG2";
                amount = 12.34M;

                net.authorize.sample.ChargeCreditCard.Run(apiLoginId, "346HZ32z3fP4hTG2", amount);
                
                status = "GOOD";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
        }
    }


    
}
