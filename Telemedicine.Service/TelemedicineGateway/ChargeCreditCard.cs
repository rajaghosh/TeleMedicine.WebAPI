using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using AuthorizeNet.Api.Controllers;
using net.authorize.sample;

namespace Telemedicine.Service.TelemedicineGateway
{
    public class TelemedicineChargeCreditCard//: ChargeCreditCard
    {
        public static ANetApiResponse Run(String ApiLoginID, String ApiTransactionKey, decimal amount, out string transId)
        {

            ANetApiResponse responseData = ChargeCreditCard.Run( ApiLoginID, ApiTransactionKey, amount);
            //createTransactionResponse transactResponse = new createTransactionResponse();
            //transactResponse = (createTransactionResponse)responseData;//.transactionResponse
            //transactionResponse trans = new transactionResponse();
            //trans = (transactionResponse)responseData;

            string? tempTransID = ((AuthorizeNet.Api.Contracts.V1.createTransactionResponse)responseData).transactionResponse.transId; // ""; // transactResponse.transId;

            if (string.IsNullOrEmpty(tempTransID))
            {
                transId = "";
            }
            else
            {
                transId = tempTransID;
            }
            return responseData;
        }
    }
}
