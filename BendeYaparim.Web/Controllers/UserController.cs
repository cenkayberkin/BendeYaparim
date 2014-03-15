using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BendeYaparim.Web.DAL;
using BendeYaparim.Web.Models;
using System.Data;
using BendeYaparim.Web.ViewModels;

namespace BendeYaparim.Web.Controllers
{
    public class UserController : Controller
    {
        private UserRepository repository;
        private BendeyaparimContext context;
        private IMessageRepository messageRepository;

        public UserController(UserRepository repository, BendeyaparimContext context, IMessageRepository mesRepository)
        {
            this.context = context;
            this.repository = repository;
            this.messageRepository = mesRepository;
        }

        public ActionResult Profile(int Id)
        {
            var u = repository.GetUserWithAdverts(Id);
            return View(u);
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public ActionResult Edit()
        {
            User s = repository.GetMyUser(User.Identity.Name);
            EditViewModel viewModel = new EditViewModel();
            viewModel.PhoneNumber = s.PhoneNumber;

            ViewBag.PhoneVisible = s.PhoneVisible;
            ViewBag.EmailVisible = s.EmailVisible;

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public ActionResult Edit(FormCollection formCollection)
        {
            User s = repository.GetMyUser(User.Identity.Name);

            try
            {
                s.EmailVisible = formCollection["emailVisible"] == "1" ? true : false;
                s.PhoneVisible = formCollection["phoneVisible"] == "1" ? true : false;
                s.PhoneNumber = formCollection["PhoneNumber"];
                context.Entry(s).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("MyAdverts");
            }
            catch (DataException)
            {
                //Log the error (add a variable name after DataException)
                ModelState.AddModelError("", "Değişiklikler kayıt edilemiyor, lütfen hataları düzeltip tekrar deneyin.");
            }

            return View(formCollection);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult MyAdverts()
        {
            User s = repository.GetUserWithAdverts(User.Identity.Name);
            ViewBag.NumberOfMessages = messageRepository.AllMessagesForUser(s.UserId).Count().ToString();
            return View(s);
        }

        [HttpGet]
        public ActionResult Help()
        {
            SendMessageToAdmin m = new SendMessageToAdmin();
            return View(m);
        }

        [HttpPost]
        public ActionResult Help(SendMessageToAdmin message)
        {
            if (ModelState.IsValid)
            {
                if (EmailManager.SendEmail(message))
                {
                  return RedirectToAction("MessageSent");
                }
            }

            return View();
        }

        public ActionResult MessageSent()
        {
            return View();
        }
    }
}
