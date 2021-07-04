using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telemedicine.Service.Models;
using System.Linq;
using System.Data.SqlClient;
using GracePG.Gateway.Models;
using System.Drawing.Drawing2D;
using Telemedicine.Service.Managers;
using System.IO;
using Telemedicine.Service.TelemedicineGateway;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using AuthorizeNet.Api.Controllers;
using net.authorize.sample;

namespace Telemedicine.Service.Managers
{
    public class Survey
    {
        DataConnection.DataProvider dataProvider = new DataConnection.DataProvider();
        string PMSFolder = "";
        public async Task<List<Models.SurveyStep>> getQuestions()
        {
            string retval = await dataProvider.GetScalarStringval(PMSFolder,
                           @"select a.StepID, a.Title, a.SubTitle, a.SortOrder, 
                                (Select b.QuestionID, b.Question, b.Hint, b.RegEx, b.ItemsInRow, b.Required, b.Type ,
 	                                (Select c.OptionID, c.OptionValue, c.ItemsInRow from SurveyOption c where c.QuestionID = b.questionid and  IsActive = 1 order by c.SortOrder for json path) as Options
		                                from SurveyQuestion b where a.StepID = b.StepID and IsActive = 1  order by b.SortOrder for json path ) as Questions
	                                from SurveyStep a where IsActive = 1  order by a.SortOrder
		                                for json path");
            List<Models.SurveyStep> steps = JsonConvert.DeserializeObject< List<Models.SurveyStep>>(retval);
            return steps;
        }

        public async Task<int> updateAnswer( List<Models.SurveyAnswer> surveyAnswers)
        {


            int submissionid;
            if (surveyAnswers.Count > 0 && surveyAnswers[0].SubmissionID == 0)
                submissionid = Int32.Parse((await dataProvider.GetScalarval(PMSFolder, @"Insert into SurveySubmission(SubmissionTime, Status) values (getdate(),1); select @@IDENTITY;")).ToString());
            else
                submissionid = surveyAnswers[0].SubmissionID;

            foreach (Models.SurveyAnswer surveyAnswer in surveyAnswers)
            {
                //await dataProvider.ExecuteQuery(PMSFolder, string.Format(@"Insert into SurveyAnswer(SubmissionId, QuestionId, Answer) values ({0},{1},'{2}')", 
                //    submissionid, surveyAnswer.QuestionID, formatAnswer(surveyAnswer.Answer) ));

                dataProvider.Cmd = new SqlCommand
                {
                    CommandText = "Insert into SurveyAnswer(SubmissionId, QuestionId, Answer) values (@SubmissionID,@QuestionID,@Answer)"
                };

                dataProvider.Cmd.Parameters.Add("@SubmissionID", SqlDbType.Int).Value = submissionid;
                dataProvider.Cmd.Parameters.Add("@QuestionID", SqlDbType.Int).Value = surveyAnswer.QuestionID;
                dataProvider.Cmd.Parameters.Add("@Answer", SqlDbType.NVarChar).Value = surveyAnswer.Answer;

                int result = await dataProvider.CmdExeNonQuery(PMSFolder, dataProvider.Cmd);

            }


            //dataProvider.Cmd = new SqlCommand
            //{
            //    CommandText = "update SurveySubmission set IsCovid = dbo.isCovid(@SubmissionId) where SubmissionId = @SubmissionId"
            //};

            //dataProvider.Cmd.Parameters.Add("@SubmissionID", SqlDbType.Int).Value = submissionid;
            
            //await dataProvider.CmdExeNonQuery(PMSFolder, dataProvider.Cmd);

            //await CreatePDF(surveyAnswers, submissionid);

            return submissionid;
        }

        private string formatAnswer(string answer)
        {
            return answer.Replace("'", "''");
        }

        public async Task<Models.IFrameKey> GetIframeKey(string origin)
        {

            
            SecureConfig secureConfig = await getSecureConfig();
            string tokenExID = secureConfig.TokenExId;
            string tokenExCSKey = secureConfig.TokenExCSKey;

            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string concatedinfo = string.Format("{0}|{1}|{2}|{3}", tokenExID, origin, timestamp, 2);
            string hmac = GenerateHMAC(concatedinfo, tokenExCSKey);
            return new Models.IFrameKey() { tokenExId = tokenExID, key = hmac , timestamp = timestamp }; // + "--split--" + timestamp;
        }

        private string GenerateHMAC(string concatenatedInfo, string clientSecretKey)
        {
            var result = string.Empty;
            var hmac = new System.Security.Cryptography.HMACSHA256();
            hmac.Key = Encoding.UTF8.GetBytes(clientSecretKey);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedInfo));
            result = Convert.ToBase64String(hash); // Ensure the string returned is Base64 Encoded
            return result;
        }



        public async Task<bool> CreatePDF(List<Models.SurveyAnswer> answers, int submissionid)
        {

            List<Models.SurveyStep> steps = await getQuestions();
            SecureConfig config = await getSecureConfig();

           return await Task.Run(() =>
           {

               StringBuilder html = new StringBuilder();
               html.AppendLine("<body>");
               html.AppendLine("<h3>Telemedicine Intake Request</h3>");
               steps.ForEach((step) =>
               {
                   html.AppendFormat("<h5>{0}</h5>", step.Title);
                   html.AppendLine("<hr>");
                   html.AppendLine("<table border='0' width='100%' style='font-size:13px;padding:5px'>");
                   step.Questions.ForEach((question) =>
                   {
                       html.AppendFormat("<tr><td width='75%'>{0} :</td><td><strong>{1}</strong></td></tr>", question.Question, answers.Find(x => x.QuestionID == question.QuestionID).Answer);
                   });
                   html.AppendLine("</table>");
               });
               html.AppendLine("</body>");

 /*
               PdfSharpCore.Pdf.PdfDocument pdfDocument = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html.ToString(), PdfSharpCore.PageSize.A4);
               //.PdfSharp.PdfGenerator.GeneratePdf(html.ToString(), PdfSharp.PageSize.A4);
               
               string filename = "D:\\Telemedicine\\website\\pdfs\\intake-" + submissionid.ToString() + ".pdf";
               pdfDocument.Save(filename);

               html = new StringBuilder();



              

               PdfDocument document = new PdfDocument();
               document.Info.Title = "Telemedicine Intake Request";
               PdfPage page = document.AddPage();
               XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont fontTitle = new XFont("Verdana", 12, XFontStyle.BoldItalic);
                XFont fontQuestion = new XFont("Verdana", 11, XFontStyle.Regular);
                XFont fontAnswer = new XFont("Verdana", 11, XFontStyle.Bold);

                int x = 20; // Convert.ToInt32( 0 - (page.Height / 2));
                int y = 20; // Convert.ToInt32(0 - (page.Height / 2)); ;



                steps.ForEach((step) =>
                {
                    gfx.DrawString(step.Title, fontTitle, XBrushes.Black,
                    new XRect(x, y, page.Width, page.Height),
                    XStringFormats.TopLeft);
                    y += 15;
                    step.Questions.ForEach((question) =>
                    {
                        string answer = question.Question + " : " + answers.Find(x => x.QuestionID == question.QuestionID).Answer;
                        int i = 0;

                        while(i < answer.Length)
                        {
                            int strlen = i + 90 < answer.Length ? 90 : answer.Length - i;
                            int newstrlen = strlen >= 90 ? answer.Substring(i, strlen).LastIndexOf(" ") : strlen;

                            gfx.DrawString(answer.Substring(i,newstrlen), fontQuestion, XBrushes.Black,
                                new XRect(x, y, page.Width, page.Height),
                                XStringFormats.TopLeft);
                            //gfx.
                            y += 15;
                            i = i + newstrlen;
                        }
                        y += 15;

                        if (y > 700)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            x = 20; // Convert.ToInt32( 0 - (page.Height / 2));
                            y = 20; // Convert.ToInt32(0 - (page.Height / 2)); ;
                        }

                    });

                });

               /*

             StringBuilder sb = new StringBuilder();
             sb.AppendLine("<html><h3>Telemedicine Intake Request</h3><br/>");
             steps.ForEach((step) =>
             {
                 sb.AppendFormat("<html><h4>{0}</h4><br/>", step.Title);
                 step.Questions.ForEach((question) =>
                 {
                     sb.AppendFormat("<html><p>{0} : <strong>{1}</strong></p><hr/>", question.Question, answers.Find(x => x.QuestionID == question.QuestionID).Answer);
                 });
             });



             TheArtOfDev.HtmlRenderer.PdfSharp.HtmlContainer c = new TheArtOfDev.HtmlRenderer.PdfSharp.HtmlContainer();


          */

               //try
               //{

               //    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
               //    client.Host = config.SMTPServer;
               //    client.Port = config.SMTPPort;
               //    client.Credentials = new System.Net.NetworkCredential(config.SMTPUserName, config.SMTPPassword);


               //    string name = answers.Find(x => x.QuestionID == 1).Answer + " " + answers.Find(x => x.QuestionID == 3).Answer;
               //    string phone = answers.Find(x => x.QuestionID == 10).Answer;

               //    string emailMessage = string.Format("New Intake Form Sumbission <br> Time : {0} <br> Name : {1} <br> Phone : {2} <br> :", DateTime.Now.ToLongDateString(), name, phone);

               //    System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage(config.MailFrom, config.MailTo, "TeleMedicine Intake", emailMessage);
               //    mailMessage.IsBodyHtml = true;
               //    //mailMessage.Attachments.Add(new System.Net.Mail.Attachment(filename));
               //    client.Send(mailMessage);
               //}
               //catch(Exception ex)
               //{
                   
               //}
               
                return true;
           });
        }
        
        public async Task<PaymentResult> MakePayment(Models.PaymentRequest paymentRequest)
        {

            string res = string.Empty;
            try
            {

                SecureConfig config = await getSecureConfig();

                
                GracePG.Gateway.Models.GatewayRequest gatewayRequest = new GracePG.Gateway.Models.GatewayRequest()
                {
                    IsTest = !config.IsLive,
                    Gateway = Gateway.AuthorizeNet,
                    TransactionType = TransactionType.AuthCapture,
                    GatewayUserName = config.AuthNetLogin,
                    GatewayPassword = config.AuthNetTransactionKey,
                    TokenExId = config.TokenExId,
                    TokenExAPIKey = config.TokenExApiKey,
                    Amount = paymentRequest.Amount,
                    FirstName = paymentRequest.FirstName,
                    LastName = paymentRequest.LastName,
                    Email = paymentRequest.Email,
                    Phone = paymentRequest.Phone,
                    Address = paymentRequest.Address,
                    City = paymentRequest.City,
                    State = paymentRequest.State,
                    Zip = paymentRequest.Zip,
                    ExpiryDate = paymentRequest.ExpMonth + "/" + paymentRequest.ExpYear,
                    CreditCardNumber = paymentRequest.CardToken,
                    PaymentRefId = paymentRequest.TransactionId,
                    PaymentRefDescription = paymentRequest.TransactionDescription

                };

                //ChargeCreditCard cardData = new ChargeCreditCard();
                string transId = "";
                var response = TelemedicineChargeCreditCard.Run(gatewayRequest.TokenExId, gatewayRequest.TokenExAPIKey, Convert.ToDecimal(gatewayRequest.Amount), out transId);

                //GracePG.Gateway.Managers.AuthorizeNetManager authorizeNetManager = new GracePG.Gateway.Managers.AuthorizeNetManager();
                //GatewayResponse gatewayResponse = await authorizeNetManager.MakePayment(gatewayRequest);


                //if (gatewayResponse.PaymentResult)
                if (response != null)
                {

                    dataProvider.Cmd = new SqlCommand
                    {
                        CommandText = @"Insert into Account(PatientId, SubmissionId, AccountDate, AccountName, AccountType, AccountCode, IsCredit, Amount, AuthTransId) 
                                        values(@PatientID, @SubmissionID, getdate(), @AccountName, @AccountType, @AccountCode, 1, @Amount, @AuthTransId); Select @@Identity;"
                    };

                    dataProvider.Cmd.Parameters.Add("@PatientID", SqlDbType.Int).Value = paymentRequest.PatientID;
                    dataProvider.Cmd.Parameters.Add("@SubmissionID", SqlDbType.Int).Value = paymentRequest.SubmissionID;
                    dataProvider.Cmd.Parameters.Add("@AccountName", SqlDbType.NVarChar).Value = paymentRequest.AccountName;
                    dataProvider.Cmd.Parameters.Add("@AccountType", SqlDbType.NVarChar).Value = paymentRequest.AccountType;
                    dataProvider.Cmd.Parameters.Add("@AccountCode", SqlDbType.NVarChar).Value = paymentRequest.AccountCode;
                    dataProvider.Cmd.Parameters.Add("@Amount", SqlDbType.Float).Value = paymentRequest.Amount;
                    dataProvider.Cmd.Parameters.Add("@AuthTransId", SqlDbType.NVarChar).Value = transId; // gatewayResponse.TransId;
                    await dataProvider.CmdGetScalarval(PMSFolder, dataProvider.Cmd);

                    if (paymentRequest.Reference != null && paymentRequest.Reference != string.Empty)
                    {
                        dataProvider.Cmd = new SqlCommand
                        {
                            CommandText = @"update SurveySubmission set Reference = @Reference where SubmissionId = @SubmissionId;"
                        };

                        dataProvider.Cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = paymentRequest.Reference;
                        dataProvider.Cmd.Parameters.Add("@SubmissionID", SqlDbType.Int).Value = paymentRequest.SubmissionID;
                        await dataProvider.CmdGetScalarval(PMSFolder, dataProvider.Cmd);
                    }

                    //SendEmail Start
                    try
                    {                        
                        string mailcontent = config.InvoiceEmailContent;
                        mailcontent = mailcontent.Replace("[[FirstName]]", paymentRequest.FirstName);
                        mailcontent = mailcontent.Replace("[[LastName]]", paymentRequest.LastName);
                        mailcontent = mailcontent.Replace("[[Address]]", paymentRequest.Address);
                        mailcontent = mailcontent.Replace("[[City]]", paymentRequest.City);
                        mailcontent = mailcontent.Replace("[[State]]", paymentRequest.State);
                        mailcontent = mailcontent.Replace("[[Zip]]", paymentRequest.Zip);
                        mailcontent = mailcontent.Replace("[[InvNo]]", paymentRequest.TransactionId.ToString());
                        mailcontent = mailcontent.Replace("[[AuthTransId]]", transId); // gatewayResponse.TransId);
                        mailcontent = mailcontent.Replace("[[Date]]", DateTime.Now.ToString("MM/dd/yyyy"));
                        mailcontent = mailcontent.Replace("[[ItemDescription]]", paymentRequest.TransactionDescription);
                        mailcontent = mailcontent.Replace("[[AfterPaymentInstructions]]", paymentRequest.AfterPaymentInstructions);
                        mailcontent = mailcontent.Replace("[[Price]]", paymentRequest.Amount.ToString());

                        SendMail sendMail = new SendMail();
                        EmailProperty emailProperty = new EmailProperty();
                        emailProperty.FromAddress = config.MailFrom;
                        emailProperty.ToAddress = paymentRequest.Email;
                        emailProperty.Subject = "TeleMedicine Invoice";
                        sendMail.SendEmail(emailProperty,mailcontent);                        
                    }
                    catch (Exception ex)
                    {

                    }

                    //SendEmail End
                    //return new PaymentResult() { TransactionId = paymentRequest.TransactionId, AuthTransId = gatewayResponse.TransId, Result = true };
                    return new PaymentResult() { TransactionId = paymentRequest.TransactionId, AuthTransId = transId, Result = true };
                    
                    //}
                    //else
                    //{
                    //    //Failed Transaction
                    //    //responseString
                    //    return new PaymentResult() { SubmissionID = 0, AuthTransId = "", Result = false }; ;
                    //}
                }
                else
                {
                    return new PaymentResult() { TransactionId = "0", AuthTransId = "", Result = false };
                }



            }
            catch (Exception ex)
            {
                return new PaymentResult() { TransactionId = paymentRequest.TransactionId, AuthTransId = res + " " + ex.Message, Result = false }; 
                //throw new Exception(res, ex);
            }

        }

        public async Task<PatiantInformation> UpdateSubmissionAfterPayment(PatiantInformation patiant)
        {
            if (patiant.PatientId == 0)
            {
                dataProvider.Cmd = new SqlCommand
                {
                    CommandText = @"Insert into Patient(FirstName, LastName, Phone, Email, Address, City, State, Zip, Gender, DOB, LastSubmissionID, LastTransactionTime) 
                                        values(@FirstName, @LastName, @Phone, @Email, @Address, @City, @State, @Zip, @Gender, @DOB, @SubmissionID, getdate() ); Select @@Identity;"
                };

                dataProvider.Cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = patiant.FirstName;
                dataProvider.Cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = patiant.LastName;
                dataProvider.Cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = patiant.Email;
                dataProvider.Cmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = patiant.Phone;
                dataProvider.Cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = patiant.Address;
                dataProvider.Cmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = patiant.City;
                dataProvider.Cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = patiant.State;
                dataProvider.Cmd.Parameters.Add("@Zip", SqlDbType.NVarChar).Value = patiant.Zip;
                dataProvider.Cmd.Parameters.Add("@DOB", SqlDbType.NVarChar).Value = patiant.DOBString;
                dataProvider.Cmd.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = patiant.Gender;
                dataProvider.Cmd.Parameters.Add("@SubmissionID", SqlDbType.Int).Value = patiant.SubmissionId;
                patiant.PatientId = Convert.ToInt32(await dataProvider.CmdGetScalarval(PMSFolder, dataProvider.Cmd));
            }
            
            dataProvider.Cmd = new SqlCommand
            {
                CommandText = @"update SurveySubmission set Status = 1, PaymentTime = getdate(), PatientId = @PatientID, IsCovid=@IsCovid where SubmissionID=@SubmissionID"
            };

            dataProvider.Cmd.Parameters.Add("@PatientID", SqlDbType.Int).Value = patiant.PatientId;
            dataProvider.Cmd.Parameters.Add("@SubmissionID", SqlDbType.Int).Value = patiant.SubmissionId;
            dataProvider.Cmd.Parameters.Add("@IsCovid", SqlDbType.Bit).Value = patiant.IsCovid;

            await dataProvider.CmdExeNonQuery(PMSFolder, dataProvider.Cmd);



            return patiant;

        }

        public async Task<Config> getConfig()
        {
            DataTable dataTable = await dataProvider.GetDataTable( PMSFolder, "Select * from Config");
            var configList = (from rw in dataTable.AsEnumerable()
                                          select new Config()
                                          {
                                              AmountCovid = float.Parse( rw["AmountCovid"].ToString()),
                                              AmountNonCovid = float.Parse(rw["AmountNonCovid"].ToString()),
                                              AmountRefill = float.Parse(rw["AmountRefill"].ToString()),
                                              IsLive = Convert.ToBoolean(rw["IsLive"]),
                                              CovidQuestions = rw["CovidQuestions"].ToString()
                                          }).ToList();
            return configList[0];
        }

        public async Task<SecureConfig> getSecureConfig()
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


        public async Task<List<SurveyRule>> getRules()
        {
            DataTable dataTable = await dataProvider.GetDataTable(PMSFolder, "Select * from SurveyRule");
            var configList = (from rw in dataTable.AsEnumerable()
                              select new SurveyRule()
                              {
                                StepID = Convert.ToInt32(rw["StepID"]),
                                QuestionID = Convert.ToInt32(rw["QuestionId"]),
                                Answer = rw["Answer"].ToString(),
                                NextStepID = Convert.ToInt32(rw["NextStepId"])
                              }).ToList();
            return configList;
        }

        public async Task<bool> SendOTP(PatiantInformation patient)
        {
            try
            {

                string otp = new Random().Next(100000, 999999).ToString();

                SqlCommand command = new SqlCommand();
                switch (patient.PatientType)
                {
                    case "RETURNINGCOVID":
                        command.CommandText = @"update Patient set OTP = @OTP where PatientID in ( Select top 1 PatientID from Patient a where FirstName = @FirstName and Email = @Email 
                            and convert(date,DOB) = convert(date,@DOB)
                            and exists (Select submissionid from SurveySubmission where IsCovid=1 and PatientId = a.PatientId and Status > 0)
                            order by PatientId desc)";
                        break;
                    case "RETURNINGPREVENTION":
                        command.CommandText = @"update Patient set OTP = @OTP where PatientID in ( Select top 1 PatientID from Patient a where FirstName = @FirstName and Email = @Email 
                            and exists (Select submissionid from SurveySubmission where IsCovid=0 and PatientId = a.PatientId  and Status > 0)
                            and convert(date,DOB) = convert(date,@DOB) order by PatientId desc)";
                        break;
                    case "NEWREFILL":
                        command.CommandText = @"update Patient set OTP = @OTP where PatientID in ( Select top 1 PatientID from Patient where FirstName = @FirstName and Email = @Email 
                            and convert(date,DOB) = convert(date,@DOB) order by PatientId desc)";
                        break;
                    case "RETURNINGREFILL":
                        command.CommandText = @"update Patient set OTP = @OTP where PatientID in ( Select top 1 PatientID from Patient a where FirstName = @FirstName and Email = @Email 
                            and exists (Select refillid from Refill where PatientId = a.PatientId  and Status > 0)
                            and convert(date,DOB) = convert(date,@DOB) order by PatientId desc)";
                        break;
                            
                }

               

                command.Parameters.Add(new SqlParameter("@FirstName", patient.FirstName));
                command.Parameters.Add(new SqlParameter("@Email", patient.Email));
                command.Parameters.Add(new SqlParameter("@DOB", patient.DOB.ToString("yyyy-MM-dd")));
                command.Parameters.Add(new SqlParameter("@OTP", otp));
                await dataProvider.CmdExeNonQuery(PMSFolder, command);

                command = new SqlCommand("Select FirstName, LastName, Phone, Email from Patient where FirstName = @FirstName and Email = @Email and convert(date,DOB) = convert(date,@DOB) and OTP = @OTP");
                command.Parameters.Add(new SqlParameter("@FirstName", patient.FirstName));
                command.Parameters.Add(new SqlParameter("@Email", patient.Email));
                command.Parameters.Add(new SqlParameter("@DOB", patient.DOB.ToString("yyyy-MM-dd")));
                command.Parameters.Add(new SqlParameter("@OTP", otp));

                DataTable dataTable = await dataProvider.CmdGetDatatable(PMSFolder, command);
                if (dataTable.Rows.Count > 0)
                {
                    SecureConfig config = await getSecureConfig();

                    try
                    {
                        string mailcontent = config.OTPEmailContent;
                        mailcontent = mailcontent.Replace("[[FirstName]]", patient.FirstName);
                        mailcontent = mailcontent.Replace("[[LastName]]", dataTable.Rows[0]["LastName"].ToString());
                        mailcontent = mailcontent.Replace("[[OTP]]", otp);

                        SendMail sendMail = new SendMail();
                        EmailProperty emailProperty = new EmailProperty();
                        emailProperty.FromAddress = config.MailFrom;
                        emailProperty.ToAddress = dataTable.Rows[0]["Email"].ToString();
                        emailProperty.Subject = "TeleMedicine Registration";
                        sendMail.SendEmail(emailProperty, mailcontent);
                        return true;
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<PatiantInformation> GetPatientAnswers(PatiantInformation patient)
        {
            SqlCommand command = new SqlCommand(@"Select LastSubmissionId, PatientId from Patient 
                where FirstName = @FirstName and Email = @Email and convert(date,DOB) = convert(date,@DOB) and OTP = @OTP");
            command.Parameters.Add(new SqlParameter("@FirstName", patient.FirstName));
            command.Parameters.Add(new SqlParameter("@Email", patient.Email));
            command.Parameters.Add(new SqlParameter("@DOB", patient.DOB.ToString("yyyy-MM-dd")));
            command.Parameters.Add(new SqlParameter("@OTP", patient.OTP));

            DataTable dtPatient = await dataProvider.CmdGetDatatable(PMSFolder, command);

            if (dtPatient.Rows.Count > 0)
            {
                command = new SqlCommand(@"update Patient set OTP = null where FirstName = @FirstName and Email = @Email 
                    and convert(date,DOB) = convert(date,@DOB)and OTP = @OTP");
                command.Parameters.Add(new SqlParameter("@FirstName", patient.FirstName));
                command.Parameters.Add(new SqlParameter("@Email", patient.Email));
                command.Parameters.Add(new SqlParameter("@DOB", patient.DOB.ToString("yyyy-MM-dd")));
                command.Parameters.Add(new SqlParameter("@OTP", patient.OTP));
                await dataProvider.CmdExeNonQuery(PMSFolder, command);

                int submissionId = Convert.ToInt32(dtPatient.Rows[0]["LastSubmissionId"]);
                int patientId = Convert.ToInt32(dtPatient.Rows[0]["PatientId"]);
                command = new SqlCommand("Select a.*, b.Type  from SurveyAnswer a inner join SurveyQuestion b on a.QuestionID = b.QuestionID where SubmissionId = @SubmissionId");
                command.Parameters.Add(new SqlParameter("@SubmissionId", submissionId));
                
                DataTable dataTable = await dataProvider.CmdGetDatatable(PMSFolder, command);
                var answers = (from rw in dataTable.AsEnumerable()
                                select new SurveyAnswer()
                                {
                                    SubmissionID = Convert.ToInt32(rw["SubmissionId"].ToString()),
                                    QuestionID = Convert.ToInt32(rw["QuestionId"].ToString()),
                                    Answer = rw["Answer"].ToString(),
                                    QuestionType = rw["Type"].ToString()
                                }).ToList();
                patient.PatientId = patientId;
                patient.Answers = answers;

                return patient;
            }

            return null;
        }

        public async Task<int> InsertRefill(PatiantInformation patient)
        {
            SqlCommand command = new SqlCommand("Insert into Refill (PatientId, Status) values (@PatientId, 0); select @@identity;");
            command.Parameters.Add(new SqlParameter("@PatientId", patient.PatientId));
            
            var orefillId = await dataProvider.CmdGetScalarval(PMSFolder, command);

            if(orefillId != null) { 
                return Convert.ToInt32(orefillId); ;
            }

            return 0;
        }

        public async Task UpdateRefillAsPaid(PatiantInformation patient)
        {
            SqlCommand command = new SqlCommand("Update Refill set PaymentDate = getdate(), Status = 1 where RefillId = @SubmissionId");
            command.Parameters.Add(new SqlParameter("@SubmissionId", patient.SubmissionId));
            await dataProvider.CmdExeNonQuery(PMSFolder, command);
        }

        public async Task<Rate> GetRate(string type)
        {
            SqlCommand command = new SqlCommand("select * from Rates where RateName=@RateName");
            command.Parameters.Add(new SqlParameter("@RateName", type));

            DataTable dataTable = await dataProvider.CmdGetDatatable(PMSFolder, command);
            var rates = (from rw in dataTable.AsEnumerable()
                           select new Rate()
                           {
                               RateName = rw["RateName"].ToString(),
                               Amount = float.Parse(rw["Amount"].ToString()),
                               TransactionType = rw["TransactionType"].ToString(),
                               PaymentDescription = rw["PaymentDescription"].ToString()
                           }).ToList();


            if (rates.Count > 0)
                return rates[0];
            else
                return null;
        }

    }
}
