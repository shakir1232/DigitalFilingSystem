using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalFilingSystem.ViewModels
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            Roles = new List<UserRole>();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<UserRole> Roles { get; set; }
    }

    public class UserRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class UsersRole
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }

    public class RolesViewModel
    {
        public List<UsersRole> UsersRoles { get; set; } = new List<UsersRole>();
    }
}