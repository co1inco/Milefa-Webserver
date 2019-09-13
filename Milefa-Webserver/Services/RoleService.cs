using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Milefa_WebServer.Data;
using Milefa_WebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milefa_Webserver.Services
{
    public interface IRolesService
    {
        List<Role> GetUserRoles(int id);
        void AddUserRoles(int userId, string role);
        void RemoveUserRole(int id);
    }


    public class RoleService : IRolesService
    {
        private CompanyContext _context;

        public RoleService(CompanyContext context)
        {
            _context = context;
        }

        public List<Role> GetUserRoles(int id)
        {
        return _context.Roles.Where(x => x.UserID == id).ToList();
        }

        public void AddUserRoles(int userId, string role)
        {
            Role newRole = new Role { RoleName = role, UserID = userId };

            if (!_context.Roles.AsNoTracking().Any(x => x.UserID == newRole.UserID && x.RoleName == newRole.RoleName))
            {
                _context.Roles.Add(newRole);
                _context.SaveChanges();
            }
        }

        public async void RemoveUserRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return;
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
