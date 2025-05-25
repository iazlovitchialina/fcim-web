using System;
using System.Web.Security;
using UTM.Keto.Application;
using UTM.Keto.Application.BLogic;
using UTM.Keto.Application.Interfaces;

namespace UTM.Keto.Web.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        private readonly IUserBL _userBL;
        private readonly IRoleBL _roleBL;

        public CustomRoleProvider()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
            _roleBL = factory.GetRoleBL();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return _roleBL.IsUserInRole(username, roleName);
        }

        public override string[] GetRolesForUser(string username)
        {
            return _roleBL.GetRolesForUser(username);
        }

        public override string[] GetAllRoles()
        {
            return _roleBL.GetAllRoles();
        }

        #region Не реализованные методы

        public override string ApplicationName
        {
            get { return "UTM.Keto.Web"; }
            set { }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
