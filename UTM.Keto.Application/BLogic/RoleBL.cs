using System;
using System.Linq;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class RoleBL : IRoleBL
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserBL _userBL;

        public RoleBL()
        {
            _db = new ApplicationDbContext();
            _userBL = BusinessLogicFactory.Instance.GetUserBL();
        }

        public bool IsUserInRole(string email, string roleName)
        {
            return _userBL.IsUserInRole(email, roleName);
        }

        public string[] GetRolesForUser(string email)
        {
            return _userBL.GetUserRoles(email);
        }

        public string[] GetAllRoles()
        {
            return Enum.GetNames(typeof(UserRole));
        }
    }
} 