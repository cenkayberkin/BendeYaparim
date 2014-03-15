using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BendeYaparim.Web.Models;
using BendeYaparim.Web.DAL;

namespace BendeYaparim.Web.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageRepository messageRepository;
        private readonly UserRepository userRepository;

        public MessageController(IMessageRepository messageRepository, UserRepository userRepo)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepo;
        }

        [Authorize(Roles = "Customer")]
        public ViewResult Index()
        {
            int UserID = userRepository.GetMyUser(User.Identity.Name).UserId;
            return View(messageRepository.AllMessagesForUser(UserID));
        }

        public ViewResult Details(int id)
        {
            return View(messageRepository.Find(id));
        }

        [Authorize(Roles = "Customer")]
        public ActionResult Create(int UserId)
        {
            //user is sending message to UserId user.
            ViewBag.RecepientUserID = UserId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public ActionResult Create(Message message, int RecepientUserID)
        {
            if (ModelState.IsValid)
            {
                message.FromUserId = userRepository.GetMyUser(User.Identity.Name).UserId;
                message.ToUserId = RecepientUserID;
                messageRepository.Insert(message);
                messageRepository.Save();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.RecepientUserID = RecepientUserID;
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View(messageRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Message message)
        {
            if (ModelState.IsValid)
            {
                messageRepository.Insert(message);
                messageRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Customer")]
        public ActionResult Delete(int id)
        {
            return View(messageRepository.Find(id));
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            string UserName = HttpContext.User.Identity.Name;
            var User = userRepository.GetMyUser(UserName);
            messageRepository.Delete(id, User.UserId);
            messageRepository.Save();
            return RedirectToAction("Index", "Message");
        }
    }
}

