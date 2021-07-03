using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Telemedicine.Service.Managers;
using Telemedicine.Service.Models;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        Admin adminService;
        ActivityLog activityLog;
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { Id = 0, Username = "", Password = "", AccessLevel = 0, IsAdmin = false }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            adminService = new Admin();
            LoginDetails loginDetails = new LoginDetails();

            loginDetails.UserName = model.Username;
            loginDetails.Password = model.Password;
            bool isValidUser = await adminService.IsValidUser(loginDetails);

            if (isValidUser)
            {
                loginDetails = await adminService.GetLoginDetails(loginDetails.UserName, loginDetails.Password);
                _users[0].Id = loginDetails.UserId;
                _users[0].Username = loginDetails.UserName;
                _users[0].Password = model.Password; 
                _users[0].AccessLevel = loginDetails.AccessLevel;
                _users[0].IsAdmin = loginDetails.IsAdmin;
            }
            else
            {
                _users[0].Username = string.Empty;
                _users[0].Password = string.Empty;
            }

            var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // return null if user not found
            if (user == null)
            {
                return null;
            }
            else
            {
                activityLog = new ActivityLog
                {
                    Activity = "User Logged in successfully",
                    Details = "",
                    LoginDetails = loginDetails
                };
                //await adminService.InsertActivityLog(activityLog);
            }
               

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}