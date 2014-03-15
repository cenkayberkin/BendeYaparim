using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BendeYaparim.Web.Models;

namespace BendeYaparim.Web.DAL
{
    public class RoleRepository : IRoleRepository
    {
        public BendeyaparimContext context;

        public RoleRepository(BendeyaparimContext con)
        {
            this.context = con;
        }

        public User GetUserWithRoles(string username)
        {
            User user = context.Users.Include("Roles").Single(a => a.UserName == username);
            return user;
        }
    }

    public interface IRoleRepository
    {
        User GetUserWithRoles(string username);
    }
}