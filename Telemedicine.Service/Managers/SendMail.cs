using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Telemedicine.Service.Models;
using Telemedicine.Service.Managers;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

namespace Telemedicine.Service.Managers
{
    public class SendMail
    {
        DataConnection.DataProvider dataProvider = new DataConnection.DataProvider();
        string PMSFolder = "";        
            
        public async void SendEmail(EmailProperty emailProperty,string mailcontent)
        {
            bool noBcc = false;
            SecureConfig secureConfig = await GetConfigTable();
            emailProperty.EmailBody = !string.IsNullOrEmpty(mailcontent) ? mailcontent : emailProperty.EmailBody;
            noBcc = string.IsNullOrEmpty(mailcontent) ? true : false;
            try
            {               
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage(secureConfig.MailFrom, emailProperty.ToAddress, emailProperty.Subject, emailProperty.EmailBody);
                if((!noBcc) || (emailProperty.Subject == "TeleMedicine Registration")) 
                {
                    mailMessage.Bcc.Add(secureConfig.MailTo);
                }                
                mailMessage.IsBodyHtml = true;                             
                System.Net.Mail.SmtpClient mailclient = new System.Net.Mail.SmtpClient();
                mailclient.Host = secureConfig.SMTPServer;
                mailclient.Port = secureConfig.SMTPPort;
                mailclient.Credentials = new System.Net.NetworkCredential(secureConfig.SMTPUserName,secureConfig.SMTPPassword);                                           
                mailclient.Send(mailMessage);
            }
            catch (Exception ex)
            {}           
        }

        public async Task<SecureConfig> GetConfigTable()
        {
            DataTable dataTable = await dataProvider.GetDataTable(PMSFolder, "Select * from Config");
            var configList = (from rw in dataTable.AsEnumerable()
                              select new SecureConfig()
                              {
                                  IsLive = Convert.ToBoolean(rw["IsLive"]),
                                  TokenExId = rw["TokenExId"].ToString(),
                                  TokenExApiKey = rw["TokenExApiKey"].ToString(),
                                  TokenExCSKey = rw["TokenExCSKey"].ToString(),
                                  SMTPServer = rw["SMTPServer"].ToString(),
                                  SMTPPort = Convert.ToInt32(rw["SMTPPort"]),
                                  SMTPUserName = rw["SMTPUserName"].ToString(),
                                  SMTPPassword = rw["SMTPPassword"].ToString(),
                                  MailFrom = rw["MailFrom"].ToString(),
                                  MailTo = rw["MailTo"].ToString(),
                                  AuthNetLogin = rw["AuthNetLogin"].ToString(),
                                  AuthNetTransactionKey = rw["AuthNetTransactionKey"].ToString(),
                                  InvoiceEmailContent = rw["InvoiceEmailContent"].ToString(),
                                  OTPEmailContent = rw["OTPEmailContent"].ToString()
                              }).ToList();
            return configList[0];
        }

    }
}
