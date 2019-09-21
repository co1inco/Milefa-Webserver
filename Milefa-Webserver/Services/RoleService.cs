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
        Task RemoveUserRoles(int id);
        Task RemoveUserRole(int id, string role);

    }


    public class RoleService : IRolesService
    {
        private readonly CompanyContext _context;

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

            var t = _context.Roles.AsNoTracking()
                .Where(i => i.UserID == newRole.UserID && i.RoleName == newRole.RoleName).ToArray();
            if (t.Length == 0)
            {
                _context.Roles.Add(newRole);
                _context.SaveChanges();
            }
        }

        public async Task RemoveUserRoles(int userID)
        {
            var user = await _context.User.FindAsync(userID);

            if (user == null)
            {
                return;
            }


            foreach (Role userRole in user.Roles)
            {
                _context.Roles.Remove(userRole);

            }

        }

        public async Task RemoveUserRole(int userID, string role)
        {
            var r = await _context.Roles.FirstOrDefaultAsync(i => i.RoleName == role && i.UserID == userID);
            if (r == null)
            {
                return;
            }

            _context.Roles.Remove(r);
        }
    }
}
