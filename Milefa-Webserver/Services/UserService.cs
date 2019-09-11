using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Milefa_WebServer.Entities;
using Milefa_WebServer.Helpers;

// Part of user authentication
namespace Milefa_WebServer.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>
        {
            new User {ID = 1, Name = "Colin", Username = "Colin", Password = "q", Roles = new List<string>
            {
                Role.Sysadmin,
                Role.Admin,
                Role.User,
                Role.HumanResource
            }},
            new User {ID = 2, Name = "User", Username = "User", Password = "user", Roles = new List<string> {Role.User}},
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            if (user == null)
                return null;
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.ID.ToString()),
            };
            foreach (string userRole in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // Remove data not suposed to be send back
            user.Password = null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _users.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }

        public User GetById(int id)
        {
            var user = _users.FirstOrDefault(x => x.ID == id);

            // return user without password
            if (user != null)
                user.Password = null;

            return user;
        }
    }
}