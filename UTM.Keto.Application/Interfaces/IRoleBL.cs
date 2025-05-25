using System.Collections.Generic;

namespace UTM.Keto.Application.Interfaces
{
    public interface IRoleBL
    {
        bool IsUserInRole(string email, string roleName);
        string[] GetRolesForUser(string email);
        string[] GetAllRoles();
    }
} 