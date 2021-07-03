using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicine.Service.Managers;
using Telemedicine.Service.Models;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        Admin admin = new Admin();

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("authenticateAsync")]
        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model)
        {
            return await _userService.Authenticate(model);
        }


        [HttpPost("sendVerificationCode")]
        public async Task<bool> SendVerificationCode(LoginDetails loginDetails)
        {
            string verificationCode = GenerateRandomOTP();
            SendMail sendMail = new SendMail(); 

            EmailProperty emailProperty = new EmailProperty
            {
                FromAddress = "noreply@frontlinemds.com",
                ToAddress = loginDetails.Email,
                Subject = "User Verification Code - For " + loginDetails.UserName,
                
                EmailBody = "<div style='background-color:#ccc;display: table;position: absolute;top: 0;left: 0;height: 100%;" +
                "width: 100%;font-family: '><div style='display: table-cell;vertical-align: middle;padding: 36px 0;'>" +
                "<div style='margin:auto;background-color:#fff;margin-left: auto;margin-right: auto;width:68%;'> " +
                "<div style=' padding: 7px 15px;'><img src='' width='140px' " +
                "height='auto'><h3 style='text-align:center;font-size: 25px;font-weight:600;line-height:0;'>Security Alert!!</h3>" +
                "<p style=' text-align: center;padding-top:16px;'>Since you have logged in first time in this device, We need to verify " +
                "whether it is you. To ensure we're keeping the most secure environment possible for both you and your guests, please " +
                "enter the below verification code in the Telemedicine verification screen  to complete the login.</p><p></p><center " +
                "style='/* background-color: #e4781b; */color: #fff;border-radius: 8px;width: 80%;margin: auto;padding: 0px 0px 2px 0;'>" +
                "<h3 style='color: #131212;font-size: 19px;font-weight: 700;'>User Name: " + loginDetails.UserName + "</h3><h3 style='color: #020202;" +
                "font-size: 22px;font-weight: 700;'>Verification Code: " + verificationCode + "</h3></center></div></div></div></div>"
            }; 
           
            var result = await admin.InsertLoginDetails(loginDetails, verificationCode);
            sendMail.SendEmail(emailProperty,"");
            return result;
        }


        [HttpPost("sendForgetPwdVerificationCode")]
        public async Task<AuthProperties> SendForgetPwdVerificationCode(AuthProperties authProperties)
        {
            string verificationCode = GenerateRandomOTP();
            SendMail sendMail = new SendMail();

            bool isValidUser = await admin.IsValidUser(authProperties.LoginDetails);
            if (isValidUser)
            {
                authProperties.IsVerifiedUser = true;
                EmailProperty emailProperty = new EmailProperty
                {
                    FromAddress = "noreply@frontlinemds.com",
                    ToAddress = authProperties.LoginDetails.Email,
                    Subject = "User Verification Code - For " + authProperties.LoginDetails.UserName,
                    EmailBody = "<div style='background-color:#ccc;display: table;position: absolute;top: 0;left: 0;height: 100%;" +
                    "width: 100%;font-family: '><div style='display: table-cell;vertical-align: middle;padding: 36px 0;'>" +
                    "<div style='margin:auto;background-color:#fff;margin-left: auto;margin-right: auto;width:68%;'> " +
                    "<div style=' padding: 7px 15px;'><img src='' width='140px' " +
                    "height='auto'><h3 style='text-align:center;font-size: 25px;font-weight:600;line-height:0;'>Security Alert!!</h3>" +
                    "<p style=' text-align: center;padding-top:16px;'>Please use the below verification code in the Telemedicine verification screen to update login password.</p><p></p><center " +
                    "style='/* background-color: #e4781b; */color: #fff;border-radius: 8px;width: 80%;margin: auto;padding: 0px 0px 2px 0;'>" +
                    "<h3 style='color: #131212;font-size: 19px;font-weight: 700;'>User Name: " + authProperties.LoginDetails.UserName + "</h3><h3 style='color: #020202;" +
                    "font-size: 22px;font-weight: 700;'>Verification Code: " + verificationCode + "</h3></center></div></div></div></div>"
                };

                await admin.InsertLoginDetails(authProperties.LoginDetails, verificationCode);
                var loginDetails = await admin.GetUserDetails(string.Empty, authProperties.LoginDetails.UserName);
                authProperties.LoginDetails = loginDetails[0];                
                sendMail.SendEmail(emailProperty,"");
            }
            else
            {
                authProperties.IsVerifiedUser = false;
            }
            return authProperties;
        }

        [HttpPost("checkVerificationCode")]
        public async Task<bool> IsVerifiedUser(LoginDetails loginDetails, string verificationCode)
        {
            bool result = await admin.IsVerifiedUser(loginDetails, verificationCode);
            if (result)
            {
                await admin.AddUserLoginDetails(loginDetails);
            }
            return result;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
         
        public string GenerateRandomOTP()
        {
            int iOTPLength = 6;
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = string.Empty; 
            string sTempChars = string.Empty; 
            Random rand = new Random();

            for (int i = 0; i < iOTPLength; i++)
            { 
                int p = rand.Next(0, saAllowedCharacters.Length); 
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)]; 
                sOTP += sTempChars; 
            }
            return sOTP;
        }  
    }
}
