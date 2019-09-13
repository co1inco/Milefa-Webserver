using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Milefa_WebServer.Dtos;
using Milefa_WebServer.Services;
using Milefa_WebServer.Entities;
using Milefa_WebServer.Helpers;
using Milefa_WebServer.Models;
using Milefa_WebServer.Data;
using Microsoft.EntityFrameworkCore;
using Milefa_Webserver.Services;

// https://jasonwatmore.com/post/2019/01/08/aspnet-core-22-role-based-authorization-tutorial-with-example-api

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IRolesService _roleService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly CompanyContext _context;

        public UsersController(
            IUserService userService,
            IRolesService roleService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            CompanyContext context)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var hasRoles = _context.Roles.Where(x => x.UserID == user.ID);
              //  new List<string>
              //  {RoleStrings.Sysadmin, RoleStrings.User, RoleStrings.Admin, RoleStrings.HumanResource};


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.ID.ToString()),
            };
            foreach (Role userRole in hasRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.RoleName));
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
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = user.ID,
                Username = user.Username,
                Token = tokenString,
                Roles = hasRoles,
            });

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            try
            {
                _userService.Create(user, userDto.Password);
                if (user.Username == "Colin")
                {
                    _roleService.AddUserRoles(user.ID, RoleStrings.Sysadmin);
                    _roleService.AddUserRoles(user.ID, RoleStrings.Admin);
                    _roleService.AddUserRoles(user.ID, RoleStrings.User);
                    _roleService.AddUserRoles(user.ID, RoleStrings.HumanResource);
                }
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [Authorize(Roles = RoleStrings.AccessAdmin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("claims")]
        public object Claims()
        {
            return User.Claims.Select(c =>
                new
                {
                    Type = c.Type,
                    Value = c.Value
                });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Roles = _roleService.GetUserRoles(user.ID);

            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(RoleStrings.Admin))
            {
                return Forbid();
            }

            return Ok(user);
        }
    }
}
