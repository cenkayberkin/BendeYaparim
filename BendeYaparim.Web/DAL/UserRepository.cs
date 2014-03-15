using System;
using System.Collections.Generic;
using BendeYaparim.Web.Models;
using System.Net.Mail;
using System.Web.Security;
using System.Security.Cryptography;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace BendeYaparim.Web.DAL
{
    public class UserRepository
    {
        public BendeyaparimContext db;

        public UserRepository(BendeyaparimContext context)
        {
            db = context;
        }

        public User GetUserWithAdverts(int Id)
        {
            User result = db.Users.
                Include("City").
                Include(p => p.JobSeeks).
                Include(p => p.JobOffers).
                Include("JobOffers.City").
                Include("JobOffers.Category").
                Include("JobSeeks.City").
                Include("JobSeeks.Category").
                Include(a => a.City).
                Where(a => a.UserId == Id).Single();

            return result;
        }

        public User GetUserWithAdverts(string Name)
        {
            User result = db.Users.
                Include(p => p.JobSeeks).
                Include("JobSeeks.City").
                Include("JobSeeks.Category").
                Include("JobOffers.City").
                Include("JobOffers.Category").
                Include(a => a.JobOffers).
                Include(a => a.City).
                Where(a => a.UserName == Name).Single();

            return result;

        }


        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[32];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }

        private static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            string hashedPwd =
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                    saltAndPwd, "sha1");
            return hashedPwd;
        }

        public MembershipUser MyCreateUser(string username, string password,
            string email, int BirthYear, int CityId, bool Gender, string Name, string SirName, string PhoneNumber, bool PhoneVisible, bool EmailVisible)
        {

            User user = new User();

            user.UserName = username;
            user.Email = email;
            user.PhoneVisible = PhoneVisible;
            user.EmailVisible = EmailVisible;
            user.PasswordSalt = CreateSalt();
            user.Password = CreatePasswordHash(password, user.PasswordSalt);
            user.CreatedDate = DateTime.Now;
            user.IsActivated = false;
            user.IsLockedOut = false;
            user.LastLockedOutDate = DateTime.Now;
            user.LastLoginDate = DateTime.Now;
            user.NewEmailKey = GenerateKey();

            //For Roles
            Role customerRole = db.Roles.First(a => a.RoleName == "Customer");
            user.Roles = new List<Role>();
            user.Roles.Add(customerRole);
            //

            //Profile

            user.BirthYear = BirthYear;
            user.City = db.Cities.Find(CityId);
            user.Gender = Gender;
            user.FirstName = Name;
            user.LastName = SirName;
            user.PhoneNumber = PhoneNumber;

            //
            db.Users.Add(user);
            db.SaveChanges();

            //burdan sonrasi email atmak icin.

            string ActivationLink = "bendeyaparim.com hesabınızı aktive etmek için lütfen linke tıklayınız  http://bendeyaparim.com/Account/Activate/" +
                                                  user.UserName + "/" + user.NewEmailKey;

            var message = new MailMessage("info@bendeyaparim.com", user.Email)
            {
                Subject = "Bendeyaparim.com hesabınızı aktive ediniz.",
                Body = ActivationLink
            };

            var client = new SmtpClient("mail.bendeyaparim.com", 587);
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential("admin", "0dpqc88w");
            client.Send(message);

            return GetUser(username);

        }

        public MembershipUser CreateUser(string username, string password, string email)
        {

            User user = new User();

            user.UserName = username;
            user.Email = email;
            user.PasswordSalt = CreateSalt();
            user.Password = CreatePasswordHash(password, user.PasswordSalt);
            user.CreatedDate = DateTime.Now;
            user.IsActivated = false;
            user.IsLockedOut = false;
            user.LastLockedOutDate = DateTime.Now;
            user.LastLoginDate = DateTime.Now;
            user.NewEmailKey = GenerateKey();

            //For Roles
            Role customerRole = db.Roles.First(a => a.RoleName == "Customer");
            user.Roles = new List<Role>();
            user.Roles.Add(customerRole);
            //

            db.Users.Add(user);
            db.SaveChanges();


            //burdan sonrasi email atmak icin.

            string ActivationLink = "http://localhost:8757/Account/Activate/" +
                                                  user.UserName + "/" + user.NewEmailKey;

            var message = new MailMessage("info@denemebiriki.com", user.Email)
            {
                Subject = "Activate your account",
                Body = ActivationLink
            };

            var client = new SmtpClient("mail.denemebiriki.com", 587);
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential("info@denemebiriki.com", "1234");
            client.Send(message);

            return GetUser(username);

        }

        public string GetUserNameByEmail(string email)
        {

            var result = from u in db.Users where (u.Email == email && u.IsActivated == true) select u;

            if (result.Count() != 0)
            {
                var dbuser = result.FirstOrDefault();

                return dbuser.UserName;
            }
            else
            {
                return "";
            }

        }
        public User GetUserModel(string username)
        {
            var result = from u in db.Users where (u.UserName == username && u.IsActivated == true) select u;
            if (result.Count() != 0)
            {
                var dbuser = result.FirstOrDefault();
                return dbuser;
            }
            else
            {
                return null;
            }
        }

        public MembershipUser GetUser(string username)
        {

            var result = from u in db.Users where (u.UserName == username) select u;

            if (result.Count() != 0)
            {
                var dbuser = result.FirstOrDefault();

                string _username = dbuser.UserName;
                int _providerUserKey = dbuser.UserId;
                string _email = dbuser.Email;
                string _passwordQuestion = "";
                string _comment = dbuser.Comments;
                bool _isApproved = dbuser.IsActivated;
                bool _isLockedOut = dbuser.IsLockedOut;
                DateTime _creationDate = dbuser.CreatedDate;
                DateTime _lastLoginDate = dbuser.LastLoginDate;
                DateTime _lastActivityDate = DateTime.Now;
                DateTime _lastPasswordChangedDate = DateTime.Now;
                DateTime _lastLockedOutDate = dbuser.LastLockedOutDate;

                MembershipUser user = new MembershipUser("CustomMembershipProvider",
                                                          _username,
                                                          _providerUserKey,
                                                          _email,
                                                          _passwordQuestion,
                                                          _comment,
                                                          _isApproved,
                                                          _isLockedOut,
                                                          _creationDate,
                                                          _lastLoginDate,
                                                          _lastActivityDate,
                                                          _lastPasswordChangedDate,
                                                          _lastLockedOutDate);

                return user;
            }
            else
            {
                return null;
            }

        }

        public bool ValidateUser(string username, string password)
        {

            var result = from u in db.Users where (u.UserName == username) select u;

            if (result.Count() != 0)
            {
                var dbuser = result.First();

                if (dbuser.Password == CreatePasswordHash(password, dbuser.PasswordSalt) &&
                                                                    dbuser.IsActivated == true)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }

        }

        private static string GenerateKey()
        {
            Guid emailKey = Guid.NewGuid();

            return emailKey.ToString();
        }

        public bool ActivateUser(string username, string key)
        {

            var result = from u in db.Users where (u.UserName == username) select u;

            if (result.Count() != 0)
            {
                var dbuser = result.First();

                if (dbuser.NewEmailKey == key)
                {
                    dbuser.IsActivated = true;
                    dbuser.LastModifiedDate = DateTime.Now;
                    dbuser.NewEmailKey = null;

                    db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        public User GetMyUser(string username)
        {

            var result = db.Users.Where(u => u.UserName == username && u.IsActivated == true).Single();
            return result;

        }


    }
}