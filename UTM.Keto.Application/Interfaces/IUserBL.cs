using System;
using System.Collections.Generic;
using UTM.Keto.Domain;
using UTM.Keto.Domain.DTOs;

namespace UTM.Keto.Application.Interfaces
{
    public interface IUserBL
    {
        AuthResultDto Authenticate(string email, string password);
        AuthResultDto Register(string fullName, string email, string password);
        SignOutResultDto SignOut();
        User GetUserById(int userId);
        User GetUserById(Guid userId);
        User GetUserByEmail(string email);
        List<User> GetAllUsers();
        bool IsUserInRole(string email, string role);
        string[] GetUserRoles(string email);
        void UpdateUser(User user);
        void DeleteUser(Guid userId);
        void DeleteUser(int userId);
    }
} 