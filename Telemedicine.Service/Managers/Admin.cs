using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telemedicine.Service.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using Telemedicine.Service.DataConnection;

namespace Telemedicine.Service.Managers
{
    public class Admin
    {
        DataConnection.DataProvider dataProvider = new DataConnection.DataProvider();
        string PMSFolder = "";
        ActivityLog activityLog;
        //LoginDetails loginDetails;

        public  async Task<List<PatiantInformation>> GetPatientsList(ReportRequest reportRequest)
        {
            string queryFilter = string.Empty, 
                pharmacyFileter = string.Empty, 
                filterByUser = string.Empty,
                dateFilter = "and SS.SubmissionTime >= '{0}' and SS.SubmissionTime <= DATEADD(day, 1, '{1}') order by ss.SubmissionID desc"; ;

            pharmacyFileter = reportRequest.ReportType == "Pharmacy" ? "and Status in (3,7) " : "";  

            if (!reportRequest.IsAdmin)
            {
                queryFilter = "and L.userId = " + reportRequest.UserId + " ";
                filterByUser = queryFilter;

            }

            if (reportRequest.IsSingleSubmission)
            {
                queryFilter = queryFilter + "and ss.SubmissionId = " + reportRequest.SubmissionId + " order by SS.SubmissionId desc ";
            }
            else
            {
                queryFilter += dateFilter;
            }

            string unionQuery = "";
            if (reportRequest.ReportType == "PrescribedReport")
            { 
                unionQuery = "(select PaymentAmount, SubmissionId, SubmissionTime, FollowupNotes, " +
                    "isnull(pharmacy_name,'Not Assigned') as pharmacyname, pharmacy_sequenceId, Reference, pharmacy_name, " +
                    "PatientId, UserId, 3 as Status, Notes, IsCovid from SurveySubmission where prescribed_date = '{0}' " +
                    "union all select PaymentAmount, SubmissionId, SubmissionTime, FollowupNotes, " +
                    "isnull(pharmacy_name,'Not Assigned') as pharmacyname, pharmacy_sequenceId, Reference, pharmacy_name, " +
                    "PatientId, UserId, 7 as Status, Notes, IsCovid from SurveySubmission where completed_date = '{1}') SS " +
                    "inner join Patient p on ss.patientid = p.PatientId " +
                    "inner join account a on ss.SubmissionId = a.SubmissionId " +
                    "left join Login L on SS.UserId = L.userId where Status > 0 " + filterByUser + "order by ss.SubmissionID desc";
            }
            else
            {
                unionQuery = "SurveySubmission SS inner join Patient p on ss.patientid = p.PatientId " +
                "inner join account a on ss.SubmissionId = a.SubmissionId " +
                "left join Login L on SS.UserId = L.userId where Status > 0 " + pharmacyFileter + queryFilter;
            }

            string sqlQuery = @"select p.*, isnull(PaymentAmount, 0) as Payment_Amount, ss.SubmissionId, ss.SubmissionTime, a.AuthTransID, ss.Status, ss.Notes, ss.FollowupNotes, ss.UserId, 
                ss.pharmacy_name, ss.pharmacy_sequenceId, ss.Reference,
                a.Amount as PaymentAmount, a.AccountDate as PaymentTime,
                isnull(pharmacy_name,'Not Assigned') as pharmacyname, 
                IsCovid,
                (select Question ,Answer from SurveyQuestion SQ 
                inner join SurveyAnswer SA on SQ.QuestionID = Sa.QuestionID 
	            where SA.SubmissionID = SS.SubmissionId and SQ.QuestionID in (7,9,16) for json path) as stateandDob, isnull(upper(L.firstName),'Not Assigned') doctor_name,
                case when (select count(*) from SurveySubmission where PatientId = SS.PatientId and SubmissionId < SS.SubmissionId) > 0 then 1 else 0 end as IsReturningPatient
                from " + unionQuery;

            DataTable dtPatientLists = await dataProvider.GetDataTable(PMSFolder, string.Format(sqlQuery,
            reportRequest.StartDate, reportRequest.EndDate));

            var patientInformationList = new List<PatiantInformation>();
            try
            {
                patientInformationList = (from rw in dtPatientLists.AsEnumerable()
                                          select new PatiantInformation()
                                          {
                                              SubmissionId = Convert.ToInt32(rw["SubmissionId"].ToString()),
                                              SubmissionTime = Convert.ToDateTime(rw["SubmissionTime"].ToString()),
                                              Status = Convert.ToInt32(rw["Status"].ToString()),
                                              FirstName = rw["FirstName"] == DBNull.Value ? null : (string)rw["FirstName"],
                                              LastName = rw["LastName"] == DBNull.Value ? null : (string)rw["LastName"],
                                              Email = rw["Email"] == DBNull.Value ? null : (string)rw["Email"],
                                              Phone = rw["Phone"] == DBNull.Value ? null : (string)rw["Phone"],
                                              PaymentTime = rw["PaymentTime"] == DBNull.Value ? null : (DateTime?)rw["PaymentTime"],
                                              AuthTranId = rw["AuthTransID"] == DBNull.Value ? null : rw["AuthTransID"].ToString(),
                                              Notes = rw["Notes"] == DBNull.Value ? null : rw["Notes"].ToString(),
                                              PharmacyName = (string)rw["pharmacyname"],
                                              PharmacySequenceId = rw["pharmacy_sequenceId"] == DBNull.Value ? 0 : (int)rw["pharmacy_sequenceId"],
                                              IsCovid = Convert.ToBoolean(rw["IsCovid"]),
                                              IsCovidPatient = Convert.ToBoolean(rw["IsCovid"]) ? "YES" : "NO",
                                              PaymentAmount = Convert.ToDouble(rw["PaymentAmount"]),
                                              ProfileDetails = rw["stateandDob"].ToString() == "" ?
                                              "[{\"Question\":\"State\",\"Answer\":\"Not Available\"}," +
                                              "{\"Question\":\"DOB\",\"Answer\":\"1000-01-01T06:00:00.000Z\"}," +
                                              "{\"Question\":\"Weight\",\"Answer\":\"Not Available\"}]" :
                                              rw["stateandDob"].ToString(),
                                              FollowupNotes = rw["FollowupNotes"] == DBNull.Value ? null : (string)rw["FollowupNotes"],
                                              LoginDetails = new LoginDetails()
                                              {
                                                  UserId = rw["UserId"] == DBNull.Value ? 0 : (int)rw["UserId"],
                                              },
                                              DoctorName = rw["doctor_name"].ToString(),
                                              IsReturningPatient = Convert.ToBoolean(rw["IsReturningPatient"])
                                              //LastUpdatedDate = (DateTime)rw["lastUpdatedDate"] 
                                          }).ToList();
            }
            catch(Exception ex) { }
            return patientInformationList;
        }

        public async Task<bool> IsValidUser(LoginDetails loginDetails)
        {
            string dynamicQuery;
            dataProvider.Cmd = new SqlCommand();
            if (!string.IsNullOrEmpty(loginDetails.Email)) 
            {
                dynamicQuery = "email=@email";
                dataProvider.Cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = loginDetails.Email;
            }
            else
            {
                dynamicQuery = "password=dbo.fn_encryption(@password) COLLATE SQL_Latin1_General_CP1_CS_AS";
                dataProvider.Cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = loginDetails.Password;
            }

            dataProvider.Cmd.CommandText = "select * from Login where userName=@userName and isactive=1 and " + dynamicQuery;
            dataProvider.Cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = loginDetails.UserName; 
            return await dataProvider.CheckIfExists(PMSFolder, dataProvider.Cmd);
        }
 
        public async Task<bool> AddUserLoginDetails(LoginDetails loginDetails)
        {
            bool result = false;
            
            if(!await IsUserNameExist(loginDetails))
            {
                dataProvider.Cmd = new SqlCommand();
                dataProvider.Cmd.CommandText = "Insert into Login (firstName, lastName, userName, password, email, login, phone) " +
                    "values(@firstName, @lastName, @userName, dbo.fn_encryption(@password), @email, 'member', @phoneNo)";
                dataProvider.Cmd.Parameters.Add("@firstName", SqlDbType.VarChar).Value = loginDetails.FirstName;
                dataProvider.Cmd.Parameters.Add("@lastName", SqlDbType.VarChar).Value = loginDetails.LastName;
                dataProvider.Cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = loginDetails.UserName;
                dataProvider.Cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = loginDetails.Password;
                dataProvider.Cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = loginDetails.Email;
                dataProvider.Cmd.Parameters.Add("@phoneNo", SqlDbType.VarChar).Value = loginDetails.PhoneNumber;
                result = await dataProvider.CmdExeNonQuery(PMSFolder, dataProvider.Cmd) > 0;
                return result;
            }
            else
            {
                return result;
            } 
        }

        public async Task<bool> IsUserNameExist(LoginDetails loginDetails)
        {
            dataProvider = new DataConnection.DataProvider();
            dataProvider.Cmd.CommandText = "select * from Login where userName=@userName";
            dataProvider.Cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = loginDetails.UserName;
            return await dataProvider.CheckIfExists(PMSFolder, dataProvider.Cmd);
        }                                                                                           

        public async Task<int> UpdateLoginPassword(LoginDetails loginDetails)
        {
            dataProvider = new DataConnection.DataProvider();
            dataProvider.Cmd.CommandText = "Update Login set password=dbo.fn_encryption(@password) where userId=@userId";
            dataProvider.Cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = loginDetails.Password;
            dataProvider.Cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = loginDetails.UserId;
            int result =  await dataProvider.CmdExeNonQuery(PMSFolder, dataProvider.Cmd);

            if (result > 0)
            {
                activityLog = new ActivityLog
                {
                    LoginDetails = loginDetails,
                    Activity = "Password updated",
                    Details = ""
                };
                await InsertActivityLog(activityLog);
            }
            return result;
        }

        public async Task<int> UpdatePatiantDetails(PatiantInformation PatiantInformation)
        {
            //string sqlUpdateQuery;
            //if (!string.IsNullOrEmpty(PatiantInformation.Notes))
            //{
            //    dataProvider.Cmd.Parameters.Add("@notes", SqlDbType.VarChar).Value = PatiantInformation.Notes;
            //    sqlUpdateQuery = "Notes=@notes";
            //}
            //else
            //{
            //    dataProvider.Cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = PatiantInformation.Status;
            //    sqlUpdateQuery = "status=@status";
            //}
            string updateFields = "";
            if (PatiantInformation.IsPrescribed)
            {
                updateFields = "prescribed_date = getDate(), ";
            }
            else if (PatiantInformation.IsCompleted)
            {
                updateFields = "completed_date = getDate(), ";
            }

            dataProvider.Cmd = new SqlCommand
            {
                CommandText = "Update SurveySubmission set notes=@notes, status=@status, " + updateFields +
                "followupNotes=@followupNotes, userId=@userId where submissionId=@submissionId"
            };

            dataProvider.Cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = PatiantInformation.Notes;
            dataProvider.Cmd.Parameters.Add("@status", SqlDbType.Int).Value = PatiantInformation.Status;
            dataProvider.Cmd.Parameters.Add("@followupNotes", SqlDbType.NVarChar).Value = PatiantInformation.FollowupNotes;
            dataProvider.Cmd.Parameters.Add("@userId", SqlDbType.Int).Value = PatiantInformation.UserId;
            dataProvider.Cmd.Parameters.Add("@submissionId", SqlDbType.Int).Value = PatiantInformation.SubmissionId;

            int result =  await dataProvider.CmdExeNonQuery(PMSFolder, dataProvider.Cmd);
            bool isSendMail = result > 0 && PatiantInformation.IsAssigneeChanged && PatiantInformation.UserId != 0;

            if (isSendMail)
            {
                string patientDetails = "Name: " + PatiantInformation.FirstName + " " + PatiantInformation.LastName +", " +
                    "Submission Id: " + PatiantInformation.SubmissionId;
                SendMail sendMail = new SendMail(); 

                EmailProperty emailProperty = new EmailProperty
                {
                    FromAddress = "noreply@frontlinemds.com",
                    ToAddress = PatiantInformation.LoginDetails.Email,
                    Subject = "Patient assigned - For " + PatiantInformation.LoginDetails.UserName,

                    EmailBody = "<div style='background-color:#ccc;display: table;position: absolute;top: 0;left: 0;height: 100%;" +
                    "width: 100%;font-family: '><div style='display: table-cell;vertical-align: middle;padding: 36px 0;'>" +
                    "<div style='margin:auto;background-color:#fff;margin-left: auto;margin-right: auto;width:114%;'> " +
                    "<div style=' padding: 7px 15px;'><img src='' width='140px' " +
                    "height='auto'><h3 style='text-align:center;font-size: 25px;font-weight:600;line-height:0;'>Telemedicine Account Notification</h3>" +
                    "<p style=' text-align: center;padding-top:16px;'>Hi, New patient assigned to your account by the Telemedicine administrator. " +
                    "Please check it by login into the Telemedicine account.</p><center " +
                    "style='/* background-color: #e4781b; */color: #fff;border-radius: 8px;width: 80%;margin: auto;padding: 0px 0px 2px 0;'>" +
                    "<h3 style='color: #131212;font-size: 19px;font-weight: 700;'>User Name: " + PatiantInformation.LoginDetails.UserName + "</h3><h3 style='color: #020202;" +
                    "font-size: 22px;font-weight: 700;'>Patient Details: " + patientDetails + " </h3></center></div></div></div></div>"
                };
                sendMail.SendEmail(emailProperty,""); 
            }

            PatiantInformation.LoginDetails.UserId = PatiantInformation.LoginDetails.AdminUserId;
            PatiantInformation.LoginDetails.UserName = PatiantInformation.LoginDetails.AdminUserName;
            activityLog = new ActivityLog
            {
                LoginDetails = PatiantInformation.LoginDetails,
                Activity = PatiantInformation.Activities,
                Details = PatiantInformation.ActivitiesDetails
            };
            await InsertActivityLog(activityLog);
            return result;
        }

        public async Task<List<Models.SurveyStep>> getAnswers(int submissionId)
        {
            string retval = await dataProvider.GetScalarStringval(PMSFolder,
                           string.Format(@"select a.TabOrder, a.Title, a.SubTitle, 
                            (Select b.QuestionID, b.Question, b.Hint, b.RegEx, b.ItemsInRow, b.Required, b.Type, c.Answer
		                            from SurveyQuestion b
		                            left join SurveyAnswer c on b.QuestionID = c.QuestionID and c.SubmissionID = {0}
		                            where b.StepID in (select StepID from SurveyStep where TabOrder = a.TabOrder)  
                                    and IsActive = 1  order by b.SortOrder for json path ) as Questions
	                            from SurveyStep a where IsActive = 1  
								group by a.TabOrder, a.Title, a.SubTitle
								order by a.TabOrder
		                            for json path", submissionId));
            List<Models.SurveyStep> steps = JsonConvert.DeserializeObject<List<Models.SurveyStep>>(retval);
            return steps;
        }

        public async Task<List<LoginDetails>> GetUserDetails(string sortingColumn, string userName = "")
        {
            string orderBy = sortingColumn == "userId" ? "order by userId desc" : "order by username";
            string sqlQuery = "select * from login where login <> 'admin' " + orderBy;

            if (!string.IsNullOrEmpty(userName))
            {
                sqlQuery = "select * from login where userName = '{0}'";
                sqlQuery = string.Format(sqlQuery, userName);
            }

            DataTable dtPatientLists = await dataProvider.GetDataTable(PMSFolder, sqlQuery);
            var patientInformationList = (from rw in dtPatientLists.AsEnumerable()
                                          select new LoginDetails()
                                          {
                                              UserId = Convert.ToInt32(rw["userId"].ToString()),
                                              UserName = rw["userName"] == DBNull.Value ? null : (string)rw["userName"],
                                              FirstName = rw["FirstName"] == DBNull.Value ? null : (string)rw["FirstName"],
                                              LastName = rw["LastName"] == DBNull.Value ? null : (string)rw["LastName"],
                                              Email = rw["Email"] == DBNull.Value ? null : (string)rw["Email"],
                                              PhoneNumber = rw["phone"] == DBNull.Value ? null : (string)rw["phone"],
                                              IsActiveUser = Convert.ToBoolean(rw["isActive"])
                                          }).ToList();
            return patientInformationList;
        }

        public async Task<int> UpdateUserDetails(LoginDetails loginDetails)
        {
            dataProvider = new DataConnection.DataProvider();
            dataProvider.Cmd.CommandText = "update login set isActive = @isActive where userId = @userId";
            dataProvider.Cmd.Parameters.Add("@isActive", SqlDbType.Bit).Value = loginDetails.IsActiveUser;
            dataProvider.Cmd.Parameters.Add("@userId", SqlDbType.Int).Value = loginDetails.UserId; 
            int result = await dataProvider.CmdExeNonQuery(PMSFolder, dataProvider.Cmd);

            if (result > 0)
            { 
                string accountStatus = loginDetails.IsActiveUser ? "Activated" : "Deactivated";
                SendMail sendMail = new SendMail();
                EmailProperty emailProperty = new EmailProperty
                {
                    FromAddress = "noreply@frontlinemds.com",
                    ToAddress = loginDetails.Email,
                    Subject = "Telemedicine Account Notification - For " + loginDetails.UserName,

                    EmailBody = "<div style='background-color:#ccc;display: table;position: absolute;top: 0;left: 0;height: 100%;" +
                    "width: 100%;font-family: '><div style='display: table-cell;vertical-align: middle;padding: 36px 0;'>" +
                    "<div style='margin:auto;background-color:#fff;margin-left: auto;margin-right: auto;width:114%;'> " +
                    "<div style=' padding: 7px 15px;'><img src='' width='140px' " +
                    "height='auto'><h3 style='text-align:center;font-size: 25px;font-weight:600;line-height:0;'>Telemedicine Account Notification</h3>" +
                    "<p style=' text-align: center;padding-top:16px;'>Hi, Your account has been activated by the Telemedicine administrator.</p><p></p><center " +
                    "style='/* background-color: #e4781b; */color: #fff;border-radius: 8px;width: 80%;margin: auto;padding: 0px 0px 2px 0;'>" +
                    "<h3 style='color: #131212;font-size: 19px;font-weight: 700;'>User Name: " + loginDetails.UserName + "</h3><h3 style='color: #020202;" +
                    "font-size: 22px;font-weight: 700;'>Account Status: " + accountStatus + " </h3></center></div></div></div></div>"
                };
                sendMail.SendEmail(emailProperty,"");

                loginDetails.UserId = loginDetails.AdminUserId;
                activityLog = new ActivityLog
                {
                    LoginDetails = loginDetails,
                    Activity = loginDetails.IsActiveUser ? loginDetails.UserName + " User is activated" :
                    loginDetails.UserName + " User is deactivated",
                    Details = "User is " + accountStatus + " and email has sent to the user."
                };
                loginDetails.UserName = loginDetails.AdminUserName;
                await InsertActivityLog(activityLog);
            }  
            return result;
        }

        public async Task<LoginDetails> GetLoginDetails(string userName, string password)
        {
            string sqlQuery = "select * from Login where userName='{0}' and password=dbo.fn_encryption('{1}')";
            DataTable dtLoginDetails = await dataProvider.GetDataTable(PMSFolder, string.Format(sqlQuery, userName, password));

            var loginDetail = new LoginDetails();
            foreach (DataRow dr in dtLoginDetails.Rows)
            {
                loginDetail.UserId = Convert.ToInt32(dr["userId"].ToString());
                loginDetail.UserName = Convert.ToString(dr["userName"]);
                loginDetail.IsAdmin = dr["login"].ToString() == "admin" ? true : false;
                loginDetail.FirstName = dr["FirstName"] == DBNull.Value ? null : (string)dr["FirstName"];
                loginDetail.LastName = dr["LastName"] == DBNull.Value ? null : (string)dr["LastName"];
                loginDetail.Email = dr["Email"] == DBNull.Value ? null : (string)dr["Email"];
                loginDetail.AccessLevel = Convert.ToInt32(dr["accessLevel"].ToString());
            }
            return loginDetail;
        }

        public async Task<List<ActivityLog>> GetActivityLogs(ReportRequest reportRequest)
        {
            DataTable dtLoginDetails = await dataProvider.GetDataTable(PMSFolder, "UserActivities",
                new DataConnection.DataProvider.Parameter("@Mode", "GetActivities"),
                new DataConnection.DataProvider.Parameter("@UserId", reportRequest.UserId),
                new DataConnection.DataProvider.Parameter("@IsAdmin", reportRequest.IsAdmin),
                new DataConnection.DataProvider.Parameter("@StartDate", reportRequest.StartDate),
                new DataConnection.DataProvider.Parameter("@EndDate", reportRequest.EndDate));

            var activityLogList = (from rw in dtLoginDetails.AsEnumerable()
                                          select new ActivityLog()
                                          {
                                              HistoryDate = rw["HistoryDate"].ToString(),
                                              HistoryTime = rw["HistoryTime"].ToString(),
                                              LoginDetails = new LoginDetails
                                              {
                                                  UserId = Convert.ToInt32(rw["UserId"].ToString()),
                                                  UserName = rw["UserName"].ToString()
                                              },
                                              Activity = rw["Activity"].ToString(),
                                              Details = rw["Details"].ToString()
                                          }).ToList();
            return activityLogList; 
        }

        public async Task<bool> InsertActivityLog(ActivityLog activityLog)
        { 
            return await dataProvider.ExecuteSP(PMSFolder, "UserActivities",
                new DataConnection.DataProvider.Parameter("@UserId", activityLog.LoginDetails.UserId),
                new DataConnection.DataProvider.Parameter("@UserName", activityLog.LoginDetails.UserName),
                new DataConnection.DataProvider.Parameter("@Activity", activityLog.Activity),
                new DataConnection.DataProvider.Parameter("@Details", activityLog.Details),
                new DataConnection.DataProvider.Parameter("@Mode", "InsertActivityLog")
            );
        } 

        public async Task<bool> InsertLoginDetails(LoginDetails loginDetails, string verificationCode)
        {
            return await dataProvider.ExecuteSP(PMSFolder, "UserActivities", 
                new DataConnection.DataProvider.Parameter("@UserName", loginDetails.UserName),
                new DataConnection.DataProvider.Parameter("@EmailId", loginDetails.Email),
                new DataConnection.DataProvider.Parameter("@VerificationCode", verificationCode),
                new DataConnection.DataProvider.Parameter("@Mode", "LoginAuthentication")
            );
        }

        public async Task<bool> IsVerifiedUser(LoginDetails loginDetails, string verificationCode)
        {
            DataTable dtLoginDetails = await dataProvider.GetDataTable(PMSFolder, "UserActivities", 
                new DataConnection.DataProvider.Parameter("@UserName", loginDetails.UserName),
                new DataConnection.DataProvider.Parameter("@EmailId", loginDetails.Email),
                new DataConnection.DataProvider.Parameter("@VerificationCode", verificationCode),
                new DataConnection.DataProvider.Parameter("@Mode", "IsVerifiedUser")
            );
            return dtLoginDetails.Rows.Count > 0 ? true : false;
        }

        public async Task<List<PharmacyDetails>> GetPharmacyDetailsList()
        {
            DataTable dtPharmacyDetails = await dataProvider.GetDataTable(PMSFolder, "sp_pharmacy_info",
                 new DataConnection.DataProvider.Parameter("@mode", "selectPharmacyDetails"));
            var listPharmacyDetails = new List<PharmacyDetails>();
            try
            { 
                listPharmacyDetails = (from rw in dtPharmacyDetails.AsEnumerable()
                                          select new PharmacyDetails()
                                          {
                                              PharmacySequenceId = Convert.ToInt32(rw["pharmacy_sequenceId"].ToString()),
                                              PharmacyLIC = rw["PharmacyLIC"] == DBNull.Value ? null : (string)rw["PharmacyLIC"], 
                                              PharmacyName = rw["PharmacyName"] == DBNull.Value ? null : (string)rw["PharmacyName"],
                                              PharmacyBranch = rw["PharmacyBranch"] == DBNull.Value ? null : (string)rw["PharmacyBranch"],
                                              PharmacyContactNum = rw["PharmacyContactNum"] == DBNull.Value ? null : (string)rw["PharmacyContactNum"],
                                              PharmacyMangerFirstName = rw["PharmacyMangerFirstName"] == DBNull.Value ? null : (string)rw["PharmacyMangerFirstName"],
                                              PharmacyManagerLastName = rw["PharmacyManagerLastName"] == DBNull.Value ? null : (string)rw["PharmacyManagerLastName"],
                                              PharmacyConsultantFirstName = rw["PharmacyConsultantFirstName"] == DBNull.Value ? null : (string)rw["PharmacyConsultantFirstName"],
                                              PharamacyConsultantLastName = rw["PharamacyConsultantLastName"] == DBNull.Value ? null : (string)rw["PharamacyConsultantLastName"],
                                              PharmacyEmailAddress = rw["PharmacyEmailAddress"] == DBNull.Value ? null : (string)rw["PharmacyEmailAddress"],
                                              PharmacyAddress = rw["PharmacyAddress"] == DBNull.Value ? null : (string)rw["PharmacyAddress"],
                                              PharmacyCity = rw["PharmacyCity"] == DBNull.Value ? null : (string)rw["PharmacyCity"],
                                              Pharmacy = rw["Pharmacy"] == DBNull.Value ? null : (string)rw["Pharmacy"],
                                              PharmacyFax = rw["PharmacyFax"] == DBNull.Value ? null : (string)rw["PharmacyFax"],
                                              ParmacySevice1 = rw["PharmacyService1"] == DBNull.Value ? null : (string)rw["PharmacyService1"],
                                              ParmacySevice2 = rw["PharmacyService2"] == DBNull.Value ? null : (string)rw["PharmacyService2"],
                                              ParmacySevice3 = rw["PharmacyService3"] == DBNull.Value ? null : (string)rw["PharmacyService3"],
                                              PharmacyCreatedDate = Convert.ToDateTime(rw["PharmacyCreatedDate"].ToString()),
                                              PharmacyUpdatedDate = Convert.ToDateTime(rw["PharmacyUpdatedDate"].ToString()),
                                              PharmacyCreatedBy = rw["PharmacyCreatedBy"] == DBNull.Value ? null : (string)rw["PharmacyCreatedBy"],
                                              PharmacyUpdatedBy = rw["PharmacyUpdatedBy"] == DBNull.Value ? null : (string)rw["PharmacyUpdatedBy"] 
                                          }).ToList();
            }
            catch(Exception ex) 
            {
                //throw (ex);
            }
            return listPharmacyDetails;
        }

        public async Task<bool> UpdatePharmacyInSubmissionDetails(PharmacyDetails pharmacyDetails, int submissionId)
        {
            return await dataProvider.ExecuteSP(PMSFolder, "sp_pharmacy_info", 
                new DataConnection.DataProvider.Parameter("@pharmacy_sequenceId", pharmacyDetails.PharmacySequenceId),
                new DataConnection.DataProvider.Parameter("@pharmacy_name", pharmacyDetails.PharmacyName),
                new DataConnection.DataProvider.Parameter("@submissionId", submissionId),
                new DataConnection.DataProvider.Parameter("@Mode", "assignPharmacyToPatiant")
            );
        }

        public async Task<Escribe> GetEscibeDetails(int submissionId)
        {
            string sqlQuery = @"select SubmissionId, isnull(SS.UserId,0) UserId, isnull(pharmacy_name,'Not Assigned') as pharmacyname,
                                (select Question ,Answer from SurveyQuestion SQ inner join SurveyAnswer SA on SQ.QuestionID = Sa.QuestionID  
                                where SA.SubmissionID = SS.SubmissionId and SQ.QuestionID in (1,2,3,4,5,6,7,8,9,10,12) for json path) as QA, 
                                isnull(upper(L.firstName),'Not Assigned') doctor_name, PB.first_name, PB.last_name, PB.NPI, dea_number, PB.City, PB.State,
                                PB.ZipCode, PB.Practice_address, PB.mobile_number, PB.fax_number, PB.initial,
                                pharmacyContactNum, pharmacyAddress, pharmacyCity, pharmacyState, pharmacyZipCode, pharmacyCountry, pharmacyFax from SurveySubmission SS 
                                left join Login L on SS.UserId = L.userId
                                left join provider_basic_info PB on PB.user_id = SS.UserId
                                left join dea_registration DR on DR.user_id = SS.UserId
                                left join tblPhramacy TP on TP.pharmacy_sequenceId = SS.pharmacy_sequenceId
                                where Status > 0 and SS.SubmissionId = {0}",

            medicationQuery =  "select * from PrescriptionAdviceItems where SubmisionId = '{0}' and isActive = 1 ";

            var dtMedication = await dataProvider.GetDataTable(PMSFolder, string.Format(medicationQuery, submissionId));

            DataTable dtEscibeLists = await dataProvider.GetDataTable(PMSFolder, string.Format(sqlQuery, submissionId));
            Escribe escribe = new Escribe();
            try
            {
                if(dtEscibeLists.Rows.Count == 0)
                {
                    escribe.PatiantInformation = new PatiantInformation();
                    escribe.ProviderInformation = new ProviderInformation();
                    escribe.PharmacyDetails = new PharmacyDetails();
                }

                foreach (DataRow rw in dtEscibeLists.Rows)
                {
                    escribe = new Escribe()
                    {
                        PatiantInformation = new PatiantInformation
                        {
                            SubmissionId = Convert.ToInt32(rw["SubmissionId"].ToString()),
                            PharmacyName = (string)rw["pharmacyname"],
                            LoginDetails = new LoginDetails()
                            {
                                UserId = rw["UserId"] == DBNull.Value ? 0 : (int)rw["UserId"],
                            },
                            DoctorName = rw["doctor_name"].ToString(),
                            ProfileDetails = rw["QA"].ToString() == "" ?
                                        "[{\"Question\":\"First Name\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"Middle Name\",\"Not Available\":\"\"}," +
                                        "{\"Question\":\"Last Name\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"Gender\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"Street Address\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"City\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"State\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"ZIP\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"DOB\",\"Answer\":\"1000-01-01T06:00:00.000Z\"}," +
                                        "{\"Question\":\"PhoneNo\",\"Answer\":\"Not Available\"}," +
                                        "{\"Question\":\"Email\",\"Answer\":\"Not Available\"}]" : rw["QA"].ToString(),
                        },
                        ProviderInformation = new ProviderInformation
                        {
                            FirstName = rw["first_name"] == DBNull.Value ? null : (string)rw["first_name"],
                            LastName = rw["last_name"] == DBNull.Value ? null : (string)rw["last_name"],
                            Initial = rw["initial"] == DBNull.Value ? null : (string)rw["initial"],
                            NPI = rw["NPI"] == DBNull.Value ? null : (string)rw["NPI"],
                            City = rw["City"] == DBNull.Value ? null : (string)rw["City"],
                            State = rw["State"] == DBNull.Value ? null : (string)rw["State"],
                            ZipCode = rw["ZipCode"] == DBNull.Value ? null : (string)rw["ZipCode"],
                            PracticeAddress = rw["Practice_address"] == DBNull.Value ? null : (string)rw["Practice_address"],
                            PhoneNumber = rw["mobile_number"] == DBNull.Value ? null : (string)rw["mobile_number"],
                            FaxNumber = rw["fax_number"] == DBNull.Value ? null : (string)rw["fax_number"],
                        },
                        PharmacyDetails = new PharmacyDetails
                        {
                            PharmacyContactNum = rw["pharmacyContactNum"] == DBNull.Value ? null : (string)rw["pharmacyContactNum"],
                            PharmacyAddress = rw["pharmacyAddress"] == DBNull.Value ? null : (string)rw["pharmacyAddress"],
                            PharmacyCity = rw["pharmacyCity"] == DBNull.Value ? null : (string)rw["pharmacyCity"],
                            PharmacyState = rw["pharmacyState"] == DBNull.Value ? null : (string)rw["pharmacyState"],
                            PharmacyCountry = rw["pharmacyCountry"] == DBNull.Value ? null : (string)rw["pharmacyCountry"],
                            PharmacyZipCode = rw["pharmacyZipCode"] == DBNull.Value ? null : (string)rw["pharmacyZipCode"],
                            PharmacyFax = rw["pharmacyFax"] == DBNull.Value ? null : (string)rw["pharmacyFax"]
                        },
                        DEA = rw["dea_number"] == DBNull.Value ? null : (string)rw["dea_number"],
                        MessageID = Guid.NewGuid().ToString("N"),
                        PrescriberOrderNumber = Convert.ToInt32(rw["SubmissionId"].ToString()) + Guid.NewGuid().ToString("N")
                    };
                }

                foreach (DataRow rw in dtMedication.Rows)
                {
                    escribe.PrescribedMedicationDiscription = escribe.PrescribedMedicationDiscription + "-" +
                             (string)rw["Name"] + " - " + (string)rw["Notes"] + "\r\n\r\n";
                }

                if (escribe.PrescribedMedicationDiscription.Length > 0)
                {
                    //escribe.PrescribedMedicationDiscription = escribe.PrescribedMedicationDiscription.Substring(1);
                }
            }
            catch (Exception ex) {
                //throw ex;
            }
            return escribe;
        }
        /*
       Developer Name             : Arunraj S
       Date of Modified / Created : DEC-24-2020
       Name of the Class/         : GetMedicineDetails
       Functionality              : it's used to get the medicine details.
       */
        public async Task<List<MedicineDetails>> GetMedicineDetails(int sequenceId)
        {
            var medicineDetails = new List<MedicineDetails>();
            dataProvider.Cmd = new SqlCommand();
            DataTable medicineTable = new DataTable();
            string medicineQuery = "Select ISNULL(IsActive,0) as IsActive, b.MedicineId,MedicineName,isnull(Notes,'') as Notes from(Select * from PrescriptionAdviceItems p where SubmisionId = " + sequenceId + ") a right join pharmacymedicines b on a.medicineid = b.medicineid";
            medicineTable = await dataProvider.GetDataTable(PMSFolder, string.Format(medicineQuery));
            foreach (DataRow dr in medicineTable.Rows)
            {
                MedicineDetails medicineDetails1 = new MedicineDetails();
                medicineDetails1.Medicine = (string)dr["MedicineName"];
                medicineDetails1.MedicineId = (int)dr["MedicineId"];
                medicineDetails1.Value = (bool)dr["IsActive"];
                medicineDetails1.Description = (string)dr["Notes"];
                //medicineDetails1.PharmacyId = sequenceId;
                medicineDetails.Add(medicineDetails1);
            }
            return medicineDetails;
        }

        /*
        Developer Name             : Arunraj S
        Date of Modified / Created : DEC-24-2020
        Name of the Class/         : InsertMedicineDetails
        Functionality              : it's used to insert and update the medicine details.
        */
        public async Task<bool> InsertMedicineDetails(List<MedicineDetails> medicineDetails, int userId)
        {
            bool result = false;
            foreach (var med in medicineDetails)
            {
                MedicineDetails medicine = new MedicineDetails();
                medicine.Medicine = med.Medicine;
                medicine.MedicineId = med.MedicineId;
                medicine.Description = med.Value == true ? med.Description : "";
                medicine.Value = med.Value;
                result = await insertTheMedicine(medicine, userId);
            }
            return result;
        }

        /*
        Developer Name             : Arunraj S
        Date of Modified / Created : DEC-24-2020
        Name of the Class/         : insertTheMedicine
        Functionality              : it's used to insert and update the Medicine.
        */
        public async Task<bool> insertTheMedicine(MedicineDetails medicine, int userID)
        {
            try
            {
                return await dataProvider.ExecuteSP(PMSFolder, "PharmacyUpdate",
                    new DataConnection.DataProvider.Parameter("@SubmisionId", userID),
                    new DataConnection.DataProvider.Parameter("@MedicineId", medicine.MedicineId),
                    new DataConnection.DataProvider.Parameter("@Notes", medicine.Description),
                    new DataConnection.DataProvider.Parameter("@Name", medicine.Medicine),
                    new DataConnection.DataProvider.Parameter("@isActive", medicine.Value)
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
        Developer Name             : Arunraj S
        Date of Modified / Created : DEC-24-2020
        Name of the Class/         : GetPriscription
        Functionality              : it's used to get the priscription details.
        */
        public async Task<List<MedicineDetails>> GetPriscription(int submissionId)
        {
            var medicineDetails = new List<MedicineDetails>();
            dataProvider.Cmd = new SqlCommand();
            DataTable medicineTable = new DataTable();
            string medicineQuery = "select * from PrescriptionAdviceItems where SubmisionId = '{0}' and isActive = 1 ";
            medicineTable = await dataProvider.GetDataTable(PMSFolder, string.Format(medicineQuery, submissionId));
            foreach (DataRow dr in medicineTable.Rows)
            {
                MedicineDetails medicineDetails1 = new MedicineDetails();
                medicineDetails1.Medicine = (string)dr["Name"];
                medicineDetails1.Description = (string)dr["Notes"];
                medicineDetails1.MedicineId = (int)dr["MedicineId"];
                medicineDetails.Add(medicineDetails1);
            }
            return medicineDetails;
        }


        /*
       Developer Name             : Prakash
       Date of Modified / Created : DEC-24-2020
       Name of the Class/         : Get Refills
       Functionality              : it's used to get the priscription details.
       */
        public async Task<List<Refill>> GetRefills(SearchFilter searchFilter)
        {
            dataProvider.Cmd = new SqlCommand();
            DataTable medicineTable = new DataTable();
            dataProvider.Cmd = new SqlCommand(@"select a.RefillId, a.PaymentDate, a.RefillDate, a.Status, a.Notes, a.UserId,
                b.FirstName, b.LastName, b.Address, b.City, b.State, b.zip, b.Email, b.Phone,
                c.Amount, c.AuthTransId,
                l.firstName as LoginFirstName,
                l.LastName as LoginLastName
                from Refill a 
                inner join Patient b on a.PatientID = b.PatientID  
                inner join Account c on a.RefillId = c.SubmissionId and c.AccountType = 'REFILL'
                left join Login l on a.UserId = l.userId 
                where a.Status > 0 and a.PaymentDate >= @StartDate and a.PaymentDate <= dateadd(day,1,@EndDate) ");
            dataProvider.Cmd.Parameters.AddWithValue("StartDate", searchFilter.StartDate);
            dataProvider.Cmd.Parameters.AddWithValue("EndDate", searchFilter.EndDate);
            medicineTable = await dataProvider.CmdGetDatatable(PMSFolder, dataProvider.Cmd);
            var medicineDetails = (from rw in medicineTable.AsEnumerable()
                                   select new Refill()
                                   {
                                       RefillId = Convert.ToInt32(rw["RefillId"]),
                                       PaymentTime = Convert.ToDateTime(rw["PaymentDate"]),
                                       RefillDate = rw["RefillDate"] == DBNull.Value ? null : (DateTime?)rw["RefillDate"],
                                       Status = Convert.ToInt32(rw["Status"]),
                                       Notes = rw["Notes"].ToString(),
                                       UserId = rw["UserId"] == DBNull.Value ? 0 : (int)rw["UserId"],
                                       FirstName = rw["FirstName"].ToString(),
                                       LastName = rw["LastName"].ToString(),
                                       Address = rw["Address"].ToString(),
                                       City = rw["City"].ToString(),
                                       State = rw["State"].ToString(),
                                       Zip = rw["Zip"].ToString(),
                                       Email = rw["Email"].ToString(),
                                       Phone = rw["Phone"].ToString(),
                                       PaymentAmount = Double.Parse(rw["Amount"].ToString()),
                                       AuthTranId = rw["AuthTransId"].ToString(),
                                       LoginDetails = new LoginDetails()
                                       {
                                           UserId = rw["UserId"] == DBNull.Value ? 0 : (int)rw["UserId"],
                                           FirstName = rw["LoginFirstName"].ToString(),
                                           LastName = rw["LoginLastName"].ToString(),
                                       }
                                   });
            return medicineDetails.ToList();
        }

        /*
        Developer Name             : Arunraj
        Date of Modified / Created : FEB-03-2021
        Name of the Class/         : GetStatus
        Functionality              : it's used to get the Patient Status.
        */
        public async Task<List<StatusDetails>> GetStatus()
        {
            dataProvider.Cmd = new SqlCommand();
            DataTable statusTable = new DataTable();
            dataProvider.Cmd = new SqlCommand(@"SELECT * FROM Patientstatus  Order By Case id When 10 Then 1 When 1 Then 2 When 2 Then 3 when 3 Then 4 When 9 then 5 When 4 then 6 When 5 then 7 When 6 then 8 When 7 then 9 Else 10 End");
            statusTable = await dataProvider.CmdGetDatatable(PMSFolder, dataProvider.Cmd);
            var statusList = (from rw in statusTable.AsEnumerable()
                               select new StatusDetails()
                               {
                                    Name = rw["Status"].ToString(),
                                    Value = Convert.ToInt32(rw["Id"])                                                                       
                               });
            return statusList.ToList();
        }

        /*
        Developer Name             : Arunraj
        Date of Modified / Created : FEB-25-2021
        Name of the Class/         : GetReferrals
        Functionality              : it's used to get the Referral Counts.
        */
        public async Task<List<Referrals>> GetReferrals(SearchFilter searchFilter)
        {
            dataProvider.Cmd = new SqlCommand();
            DataTable referralDetails = new DataTable();
            dataProvider.Cmd = new SqlCommand(@"select ROW_NUMBER() over ( order by Reference desc) as id,reference as ReferralName, 'https://drstella.gracesoft.com/intakeform?ref=' + Reference as FullLink,NoOfRef , ROW_NUMBER() over ( order by NoOfRef desc) as Ranking  from (select ROW_NUMBER() over ( order by Reference desc) as id,reference,count(*) as NoOfRef from surveysubmission where status >= 1 and reference is not null and reference != '' and SubmissionTime between @StartDate and dateadd(day,1,@EndDate) group by reference)a order by NoOfRef desc");
            dataProvider.Cmd.Parameters.AddWithValue("StartDate", searchFilter.StartDate);
            dataProvider.Cmd.Parameters.AddWithValue("EndDate", searchFilter.EndDate);
            referralDetails = await dataProvider.CmdGetDatatable(PMSFolder, dataProvider.Cmd);
            var Referrals = (from rw in referralDetails.AsEnumerable()
                             select new Referrals()
                             {
                                 id =Convert.ToInt32(rw["id"]),
                                 ReferralName = rw["ReferralName"].ToString(),
                                 FullLink = rw["FullLink"].ToString(),                                
                                 Ranking = Convert.ToInt32(rw["Ranking"]),
                                 NoOfReferance = Convert.ToInt32(rw["NoOfRef"]),
                             });
            return Referrals.ToList();
        }
    }
}
