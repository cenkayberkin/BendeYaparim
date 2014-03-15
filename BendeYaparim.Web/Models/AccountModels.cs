using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Ninject;
using BendeYaparim.Web.Infrastructure;

namespace BendeYaparim.Web.Models
{
    #region Models

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şu anki Şifre")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre (Tekrar)")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ve şifre tekrarı aynı değil.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni hatırla ?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Kullanıcı Adı gerekli.")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email adresi gerekli.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Gecerli degil")]
        [Display(Name = "E-Posta")]
        [Email(ErrorMessage = "{0} geçerli değil.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email adresi tekrarı gerekli.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-Posta Tekrarı")]
        [Compare("Email", ErrorMessage = "E-Posta ve E-Posta tekrarı aynı değil.")]
        [Email(ErrorMessage = "{0} geçerli değil.")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Ad gerekli.")]
        [DataType(DataType.Text)]
        [Display(Name = "Ad")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad gerekli.")]
        [DataType(DataType.Text)]
        [Display(Name = "Soyad")]
        public string SirName { get; set; }

        [Required(ErrorMessage = "Şehir seçiniz.")]
        [Display(Name = "Şehir")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Telefon numarası gerekli.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçiniz.")]
        [Display(Name = "Cinsiyet")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Email adresi gösterimini seçiniz.")]
        [Display(Name = "Email Adresi gösterimi")]
        public int EmailVisible { get; set; }

        [Required(ErrorMessage = "Telefon numarası gösterimini seçiniz.")]
        [Display(Name = "Telefon numarası gösterimi")]
        public int PhoneNumberVisible { get; set; }

        [Required(ErrorMessage = "Doğum yılı seçiniz.")]
        [Display(Name = "Doğum Yılı")]
        public int BirthYear { get; set; }

        [ValidatePasswordLength]
        [Required(ErrorMessage = "Şifre gerekli.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı gerekli.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Yeni şifre ve şifre tekrarı aynı değil.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool AcceptUsage { get; set; }


    }

    #endregion

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        MembershipCreateStatus CreateUser(string userName, string password, string email, int BirthYear, int CityId, bool Gender, string Name, string SirName, string PhoneNumber, bool PhoneVisible, bool EmailVisible);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MyMembershipProvider _provider;

        public AccountMembershipService(MyMembershipProvider provider)
        {
            _provider = provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Alan Boş bırakılamaz.", "Kullanıcı adı");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Alan Boş bırakılamaz.", "Şifre");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email, int BirthYear, int CityId, bool Gender, string Name, string SirName, string PhoneNumber, bool PhoneVisible, bool EmailVisible)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Alan Boş bırakılamaz.", "Kullanıcı adı");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Alan Boş bırakılamaz.", "Şifre");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Alan Boş bırakılamaz.", "email");

            MembershipCreateStatus status;
            _provider.MyCreateUser(userName, password, email, BirthYear, CityId, Gender, Name, SirName, PhoneNumber, PhoneVisible, EmailVisible, out status);

            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Alan Boş bırakılamaz.", "Kullanıcı adı");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Bu kullanıcı adı sistemde var, yeni bir kullanıcı adı deneyiniz";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Bu email adresi daha önce başka bir kullanıcı tarafından kullanımış.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Geçersiz şifre.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Geçersiz email adresi.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Kullanıcı adı geçersiz.";

                case MembershipCreateStatus.ProviderError:
                    return "Hata oluştu.";

                case MembershipCreateStatus.UserRejected:
                    return "Yeni kullanıcı yaratılamadı, lütfen girdiğiniz bilgileri kontrol ediniz.";

                default:
                    return "Bilinmeyen bir hata oluştu lütfen tekrar deneyiniz, yada sistem yöneticisiyle görüşünüz.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "'{0}' en az {1} karakter uzunluğunda olmalıdır.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue)
            };
        }
    }
    #endregion
}
