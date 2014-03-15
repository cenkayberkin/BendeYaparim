using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using BendeYaparim.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Ninject;
using BendeYaparim.Web.Infrastructure;
using BendeYaparim.Web.DAL;

namespace BendeYaparim.Web.Controllers
{

    public class AccountController : Controller
    {
        [Inject]
        public IFormsAuthenticationService FormsService { get; set; }

        [Inject]
        public IMembershipService MembershipService { get; set; }

        public UserRepository _user;
        public ICityRepository cityRepository;

        public AccountController(UserRepository user, ICityRepository cityRep)
        {
            cityRepository = cityRep;
            _user = user;
        }

        protected override void Initialize(RequestContext requestContext)
        {

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            LogOnModel model = new LogOnModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("YouJustLogedIn", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı ya da şifre yanlış.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************
        public ActionResult YouJustLogedIn()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {

            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            RegisterModel model = new RegisterModel();
            var cities = cityRepository.All.ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "Şehir Seç", Value = "" });
            cities.ForEach(a => selectList.Add(new SelectListItem() { Text = a.Name, Value = a.Id.ToString() }));

            List<SelectListItem> genderSelectList = new List<SelectListItem>();
            genderSelectList.Add(new SelectListItem() { Text = "Erkek", Value = "1", Selected = true });
            genderSelectList.Add(new SelectListItem() { Text = "Kadın", Value = "0" });

            //email gosterimi
            List<SelectListItem> phoneNumVisibleSelectList = new List<SelectListItem>();
            phoneNumVisibleSelectList.Add(new SelectListItem() { Text = "istiyorum", Value = "1", Selected = true });
            phoneNumVisibleSelectList.Add(new SelectListItem() { Text = "istemiyorum", Value = "0" });

            //telefon numarasi gosterimi
            List<SelectListItem> emailVisibleSelectList = new List<SelectListItem>();
            emailVisibleSelectList.Add(new SelectListItem() { Text = "istiyorum", Value = "1", Selected = true });
            emailVisibleSelectList.Add(new SelectListItem() { Text = "istemiyorum", Value = "0" });

            ViewBag.PhoneNumVisible = phoneNumVisibleSelectList;
            ViewBag.EmailVisible = emailVisibleSelectList;
            ViewBag.Gender = genderSelectList;
            ViewBag.Cities = selectList;

            List<SelectListItem> years = new List<SelectListItem>();
            years.Add(new SelectListItem() { Text = "Doğum Tarihi Seç", Value = "", Selected = true });
            for (int i = 2000; i > 1940; i--)
            {
                years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            ViewBag.Years = years;

            return View(model);
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                bool gender;
                if (model.Gender == 1)
                {
                    //Erkek
                    gender = true;
                }
                else
                {
                    //Kadin
                    gender = false;
                }

                bool phoneVisible;
                if (model.PhoneNumberVisible == 1)
                {
                    phoneVisible = true;
                }
                else
                {
                    phoneVisible = false;
                }
                bool emailVisible;
                if (model.EmailVisible == 1)
                {
                    emailVisible = true;
                }
                else
                {
                    emailVisible = false;
                }
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(
                    model.UserName,
                    model.Password,
                    model.Email,
                    model.BirthYear,
                    model.CityId,
                    gender,
                    model.Name,
                    model.SirName,
                    model.PhoneNumber, phoneVisible,emailVisible
                    );

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return View("CheckYourEmail");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            //Cities and rest of viewbag datas.

            var cities = cityRepository.All.ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "Şehir Seç", Value = "" });
            cities.ForEach(a => selectList.Add(new SelectListItem() { Text = a.Name, Value = a.Id.ToString() }));

            List<SelectListItem> years = new List<SelectListItem>();
            years.Add(new SelectListItem() { Text = "Doğum Tarihi Seç", Value = "", Selected = true });
            for (int i = 2000; i > 1940; i--)
            {
                years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            List<SelectListItem> genderSelectList = new List<SelectListItem>();
            genderSelectList.Add(new SelectListItem() { Text = "Erkek", Value = "1" });
            genderSelectList.Add(new SelectListItem() { Text = "Kadın", Value = "0" });

            ViewBag.Gender = genderSelectList;
            selectList.Find(a => a.Value == model.CityId.ToString()).Selected = true;
            ViewBag.Cities = selectList;

            years.Find(a => a.Value == model.BirthYear.ToString()).Selected = true;
            ViewBag.Years = years;

            //email gosterimi
            List<SelectListItem> phoneNumVisibleSelectList = new List<SelectListItem>();
            phoneNumVisibleSelectList.Add(new SelectListItem() { Text = "istiyorum", Value = "1" });
            phoneNumVisibleSelectList.Add(new SelectListItem() { Text = "istemiyorum", Value = "0" });
            phoneNumVisibleSelectList.Find(a => a.Value == model.PhoneNumberVisible.ToString()).Selected = true;

            //telefon numarasi gosterimi
            List<SelectListItem> emailVisibleSelectList = new List<SelectListItem>();
            emailVisibleSelectList.Add(new SelectListItem() { Text = "istiyorum", Value = "1" });
            emailVisibleSelectList.Add(new SelectListItem() { Text = "istemiyorum", Value = "0" });
            emailVisibleSelectList.Find(a => a.Value == model.PhoneNumberVisible.ToString()).Selected = true;

            ViewBag.PhoneNumVisible = phoneNumVisibleSelectList;
            ViewBag.EmailVisible = emailVisibleSelectList;

            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult Activate(string username, string key)
        {
            if (_user.ActivateUser(username, key) == false)
                return RedirectToAction("Index", "Home");
            else
                return View("DisplayActiveAccount");
        }

        [Authorize(Roles = "Customer")]
        public ActionResult UserProfileDetail(string UserName)
        {
            User user = _user.GetMyUser(UserName);
            return View(user);
        }

        public ActionResult Help()
        {
            return View();
        }
    }
}
