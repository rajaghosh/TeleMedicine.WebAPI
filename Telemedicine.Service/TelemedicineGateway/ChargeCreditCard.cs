using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using AuthorizeNet.Api.Controllers;
using net.authorize.sample;
using Net.Authorize.Dot.Net.Core.PaymentGatewayModel;
using System.Text.Json;

namespace Telemedicine.Service.TelemedicineGateway
{
    public class TelemedicineChargeCreditCard//: ChargeCreditCard
    {
        public static ANetApiResponse Run(String ApiLoginID, String ApiTransactionKey, decimal amount, out string transId, Models.PaymentRequest paymentRequest = null)
        {
            ANetApiResponse responseData = null;
            try
            {
                GatewayPaymentRequest gatewayPayment = new GatewayPaymentRequest();

                gatewayPayment.PatientID = paymentRequest.PatientID;
                gatewayPayment.SubmissionID = paymentRequest.SubmissionID;
                gatewayPayment.CardNumber = paymentRequest.CardToken;
                gatewayPayment.ExpMonth = paymentRequest.ExpMonth;
                gatewayPayment.ExpYear = paymentRequest.ExpYear;
                gatewayPayment.FirstName = paymentRequest.FirstName;
                gatewayPayment.LastName = paymentRequest.LastName;
                gatewayPayment.Address = paymentRequest.Address;
                gatewayPayment.City = paymentRequest.City;
                gatewayPayment.State = paymentRequest.State;
                gatewayPayment.Zip = paymentRequest.Zip;
                gatewayPayment.Email = paymentRequest.Email;
                gatewayPayment.Phone = paymentRequest.Phone;
                gatewayPayment.DOB = paymentRequest.DOB;
                gatewayPayment.Gender = paymentRequest.Gender;
                gatewayPayment.Amount = paymentRequest.Amount;
                gatewayPayment.Reference = paymentRequest.Reference;
                gatewayPayment.AccountName = paymentRequest.AccountName;
                gatewayPayment.AccountCode = paymentRequest.AccountCode;
                gatewayPayment.AccountType = paymentRequest.AccountType;
                gatewayPayment.TransactionId = paymentRequest.TransactionId;
                gatewayPayment.TransactionDescription = paymentRequest.TransactionDescription;
                gatewayPayment.AfterPaymentInstructions = paymentRequest.AfterPaymentInstructions;


                responseData = ChargeCreditCard.Run(ApiLoginID, ApiTransactionKey, amount, gatewayPayment);
                //createTransactionResponse transactResponse = new createTransactionResponse();
                //transactResponse = (createTransactionResponse)responseData;//.transactionResponse
                //transactionResponse trans = new transactionResponse();
                //trans = (transactionResponse)responseData;

                string? tempTransID = ((AuthorizeNet.Api.Contracts.V1.createTransactionResponse)responseData).transactionResponse.transId; // ""; // transactResponse.transId;

                if (string.IsNullOrEmpty(tempTransID))
                {
                    transId = ""; 
                    ChargeCreditCard.LogTransaction("No Trans ID generateed XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX --- "); 
                }
                else
                {
                    transId = tempTransID;
                }
            }
            catch (Exception ex)
            {
                ChargeCreditCard.LogTransaction("Log Start: " + System.DateTime.Now.ToString() + "*************");
                ChargeCreditCard.LogTransaction("Transction Failed XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX --- " + ex.Message.ToString());
                ChargeCreditCard.LogTransaction(JsonSerializer.Serialize(paymentRequest));
                
                transId = "";
            }
            finally
            { 
            }
            return responseData;
        }
    }
}
