using ContosoUniversity.Data;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Data.Repositories;
using ContosoUniversity.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.Data.Repositories
{
    public class AdminRepository : EntityBaseRepository<ApplicationUser>, IAdminRepository
    {
        private readonly ContosoContext _context;

        public AdminRepository(ContosoContext context)
            : base(context)
        {
            _context = context;
        }  

        public IEnumerable<ApplicationUser> GetUsers()
        {
            var roleId = _context.Roles
                .Where(x => x.Name == "User")
                .Select(x => x.Id)
                .SingleOrDefault();

            var userRole = _context.UserRoles
                .Where(x => x.RoleId == roleId)
                .Select(x => x.UserId);

            List<ApplicationUser> users = new List<ApplicationUser>();

            foreach (var u in userRole)
            {
                var user = _context.Users.Find(u);
                users.Add(user);
            }

            return users;
        }

        public async void EnableLockOut(string userName)
        {
            var result = _context.Users.Where(x => x.UserName == userName).SingleOrDefault();
            result.LockoutEnd = new DateTime(9999, 12, 30);

            await _context.SaveChangesAsync();
        }

        public async void DisableLockOut(string userName)
        {
            var result = _context.Users.Where(x => x.UserName == userName).SingleOrDefault();
            result.LockoutEnd = null;

            await _context.SaveChangesAsync();
        }
    }
}