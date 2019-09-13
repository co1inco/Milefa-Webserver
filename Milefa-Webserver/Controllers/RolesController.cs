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
using Milefa_WebServer.Entities;

namespace Milefa_Webserver.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly CompanyContext _context;

        public RolesController(CompanyContext context)
        {
            _context = context;
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role[]>> GetRole(int id)
        {
            var role = _context.Roles.Where(x => x.UserID == id).AsEnumerable<Role>();

            if (role == null)
            {
                return NotFound();
            }

            return role.ToArray();
        }


        // POST: api/Roles
        [Authorize(Roles = RoleStrings.Admin)]
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            if (_context.Roles.Any(x => x.RoleName == role.RoleName && x.UserID == role.UserID))
                return BadRequest();

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRole", new { id = role.ID }, role);
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
