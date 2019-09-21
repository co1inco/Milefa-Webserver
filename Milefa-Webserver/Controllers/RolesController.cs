using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Milefa_WebServer.Data;
using Milefa_WebServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Milefa_WebServer.Entities;
using Milefa_Webserver.Services;
using Milefa_WebServer.Services;
using Remotion.Linq.Clauses;

namespace Milefa_Webserver.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly CompanyContext _context;
        private readonly IRolesService _rolesService;
        private readonly IUserService _userService;

        public RolesController(
            CompanyContext context,
            IRolesService rolesService,
            IUserService userService
            )
        {
            _context = context;
            _rolesService = rolesService;
            _userService = userService;
        }



        // GET: api/Roles/5 Get all roles a user as by his id
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRole(int id)
        {
            //var role =  await (from r in _context.Roles where r.UserID == id select r).ToListAsync();
            var role = _rolesService.GetUserRoles(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableRoles()
        {
            if (User.IsInRole(RoleStrings.Sysadmin))
            {
                return RoleStrings.AvailableRolesSysadmin;
            } else if (User.IsInRole(RoleStrings.Admin))
            {
                return RoleStrings.AvailableRolesAdmin;
            } //else if (User.IsInRole(Rol))
            return RoleStrings.AvailableRoles;
        }


        // POST: api/Roles
        [Authorize(Roles = RoleStrings.Admin)]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Role>>> PostRole(Role role)
        {
            if (_context.Roles.Any(x => x.RoleName == role.RoleName && x.UserID == role.UserID))
                return BadRequest();

            if (role.RoleName == RoleStrings.Sysadmin && !User.IsInRole(RoleStrings.Sysadmin))
            {
                return Forbid();
            }

            _rolesService.AddUserRoles(role.UserID, role.RoleName);

            return _rolesService.GetUserRoles(role.UserID);
        }


        // DELETE: api/Roles/5
        [Authorize(Roles = RoleStrings.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Role>> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            if (role.RoleName == RoleStrings.Sysadmin && !User.IsInRole(RoleStrings.Sysadmin))
            {
                return Forbid();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return role;
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.ID == id);
        }
    }
}
