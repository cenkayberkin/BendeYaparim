using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BendeYaparim.Web.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Photo { get; set; }
        public List<JobOffer> JobOffers { get; set; }
        public List<JobSeek> JobSeeks { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Comments { get; set; }
        public string LastLoginIp { get; set; }
        public bool IsActivated { get; set; }
        public bool IsLockedOut { get; set; }
        public string NewPasswordKey { get; set; }
        public DateTime NewPasswordRequested { get; set; }
        public string NewEmail { get; set; }
        public string NewEmailKey { get; set; }
        public DateTime NewEmailRequested { get; set; }
        public DateTime LastLockedOutDate { get; set; }
        public List<Role> Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public City City { get; set; }
        public int BirthYear { get; set; }

        public string PhoneNumber { get; set; }
        public bool Gender { get; set; }

        public bool EmailVisible { get; set; }
        public bool PhoneVisible { get; set; }
    }
}