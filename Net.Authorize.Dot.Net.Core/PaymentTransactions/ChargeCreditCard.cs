using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using Net.Authorize.Dot.Net.Core.PaymentGatewayModel;
using System;
using System.IO;
using System.Text.Json;
//using System.Web.Script.Serialization;

namespace net.authorize.sample
{
    public class ChargeCreditCard
    {
        public static ANetApiResponse Run(String ApiLoginID, String ApiTransactionKey, decimal amount, GatewayPaymentRequest paymentRequest = null)
        {
            Console.WriteLine("Charge Credit Card");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey,
            };

            var creditCard = new creditCardType
            {
                cardNumber = paymentRequest.CardNumber,
                expirationDate = string.Concat(Convert.ToString(paymentRequest.ExpMonth), Convert.ToString(paymentRequest.ExpYear))
                //cardCode = "" //FOR CVV
            };

            var billingAddress = new customerAddressType
            {
                firstName = paymentRequest.FirstName,
                lastName = paymentRequest.LastName,
                address = paymentRequest.Address,
                city = paymentRequest.City,
                zip = paymentRequest.Zip
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            // Add line Items - Product Details here
            //var lineItems = new lineItemType[2];
            //lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(15.00) };
            //lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(450.00) };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card

                amount = amount,
                payment = paymentType,
                billTo = billingAddress
                //lineItems = lineItems
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();
            LogTransaction("  ");
            LogTransaction("  ");
            LogTransaction("Log Start: " + System.DateTime.Now.ToString() + "*************");
            LogTransaction(JsonSerializer.Serialize(paymentRequest));

            // validate response
            if ((response.transactionResponse.errors == null) && (response.transactionResponse != null))
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        LogTransaction("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        LogTransaction("Response Code: " + response.transactionResponse.responseCode);
                        LogTransaction("Message Code: " + response.transactionResponse.messages[0].code);
                        LogTransaction("Description: " + response.transactionResponse.messages[0].description);
                        LogTransaction("Success, Auth Code : " + response.transactionResponse.authCode);

                        //Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        //Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        //Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        //Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        //Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        LogTransaction("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            LogTransaction("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            LogTransaction("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                        throw new NullReferenceException("Payment Not made");
                    }
                }
                else
                {
                    LogTransaction("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        LogTransaction("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        LogTransaction("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        LogTransaction("Error Code: " + response.messages.message[0].code);
                        LogTransaction("Error message: " + response.messages.message[0].text);
                    }
                    throw new NullReferenceException("Payment Not made");
                }
            }
            else
            {

                if(response.transactionResponse.errors != null)
                {
                   // print error code
                    foreach (var x in response.transactionResponse.errors) {
                        LogTransaction(x.errorText);
                    }
                }
                throw new NullReferenceException("Payment Not made");

            }

            return response;
        }
    
    
        public static void LogTransaction(string logData)
        {
            string path = @"C:\Log\Payment.txt";

            // transction Log
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine("The very first line!");
                tw.Close();
            }

            if (File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(logData);
                    //tw.WriteLine("date time: " + System.DateTime.Now.ToShortDateString() + " TransId:  " + tempTransID + "ApiLoginID: " + ApiLoginID + " ApiTransactionKey:" + ApiTransactionKey);
                }
            }

        }



    }
}
