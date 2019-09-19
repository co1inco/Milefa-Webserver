using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Milefa_WebServer.Data;
using Milefa_WebServer.Entities;
using Milefa_WebServer.Helpers;
using Milefa_WebServer.Models;
using Milefa_Webserver.Services;

// Part of user authentication
namespace Milefa_WebServer.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        User CreateOrReset(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
        void Delete(string username);
    }

    public class UserService : IUserService
    {

        private CompanyContext _context;
        private readonly AppSettings _appSettings;
        private readonly IRatingService _ratingService;

        public UserService(CompanyContext context, IOptions<AppSettings> appSettings, IRatingService ratingService)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _ratingService = ratingService;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.User.SingleOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return user;

            return null;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        public User GetById(int id)
        {
            return _context.User.Find(id);
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is Required");


            var t = (from u in _context.User where u.Username == user.Username select u).ToList();
            if (t.Count > 0)
            {
                throw new AppException($"Username \"{user.Username}\" is already taken");
            }

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.User.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User CreateOrReset(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is Required");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var existingUser = _context.User.FirstOrDefault(i => i.Username == user.Username);

            if (existingUser != null)
            {
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.PasswordSalt = user.PasswordSalt;
                existingUser.Type = user.Type;
                _context.User.Update(user);
            }
            else
            {
                _context.User.Add(user);
            }
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.User.Find(userParam.ID);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                if (_context.User.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");
            }

            user.Username = userParam.Username;

            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.User.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.User.AsNoTracking().FirstOrDefault(i => i.ID == id);
            if (user != null)
            {
                _ratingService.RemoveRating(user);
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }
        public void Delete(string username)
        {
            var user = _context.User.AsNoTracking().FirstOrDefault(x => x.Username == username);
            if (user != null)
            {
                _ratingService.RemoveRating(user);
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

    }
}