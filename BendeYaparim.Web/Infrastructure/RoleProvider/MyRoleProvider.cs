using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using BendeYaparim.Web.Models;
using Ninject;
using Ninject.Web.Mvc;
using BendeYaparim.Web.DAL;
using System.Web.Mvc;

namespace BendeYaparim.Web.Infrastructure
{
    public class MyRoleProvider : RoleProvider
    {
       
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            IRoleRepository roleRepository = DependencyResolver.Current.GetService<IRoleRepository>();
            //User user = db.Users.Include("Roles").Single(a => a.UserName == username);
            User user = roleRepository.GetUserWithRoles(username);
            user.Roles.ForEach(a => roles.Add(a.RoleName));


            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool result;
            IRoleRepository roleRepository = DependencyResolver.Current.GetService<IRoleRepository>();
            User user = roleRepository.GetUserWithRoles(username);

            if (user.Roles.Any(a => a.RoleName == roleName))
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }


    }
}