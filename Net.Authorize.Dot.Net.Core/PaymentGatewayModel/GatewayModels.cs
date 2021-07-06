using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Authorize.Dot.Net.Core.PaymentGatewayModel
{
    class GatewayModels
    {
    }

    public class GatewayRequest
    {
        public GatewayRequest() { }

        public string Currency { get; set; }
        public string CustomerRefId { get; set; }
        public string CustomerRefCode { get; set; }
        public string PaymentRefDescription { get; set; }
        public string PaymentRefId { get; set; }
        public string AppCode { get; set; }
        public float Amount { get; set; }
        public string RefAuthTransId { get; set; }
        public string RefCCNumber { get; set; }
        public string CVV { get; set; }
        public float RefTranAmount { get; set; }
        public string RecurringIntervalUnit { get; set; }
        public int RecurringInterval { get; set; }
        public int RecurringMaxOccurrences { get; set; }
        public float RecurringAmount { get; set; }
        public string BaseURL { get; set; }
        public string BusinessName { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string SubscriptionId { get; set; }
        public string CardType { get; set; }
        public string ExpiryDate { get; set; }
        public string CreditCardNumber { get; set; }
        public Gateway Gateway { get; set; }
        public TransactionType TransactionType { get; set; }
        public string GatewayUserName { get; set; }
        public string GatewayPassword { get; set; }
        public string GatewayAPIKey { get; set; }
        public string GatewayCountry { get; set; }
        public bool IsTest { get; set; }
        public string TokenExId { get; set; }
        public string TokenExAPIKey { get; set; }
        public float GatewayServiceCharge { get; set; }
        public float GatewayServicePercentage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
    }

    public enum Gateway
    {
        AuthorizeNet = 0,
        Moneris = 1,
        PaypalCheckout = 2,
        PaypalPaymentsPro = 3,
        PaypalWebsitePaymentsPro = 4,
        Stripe = 5,
        Manual = 6
    }

    public enum TransactionType
    {
        AuthCapture = 0,
        AuthOnly = 1,
        Refund = 2,
        Void = 3,
        CaptureAuthorization = 4,
        AuthVoid = 5,
        SaveCard = 6,
        Recurring = 7
    }
}
