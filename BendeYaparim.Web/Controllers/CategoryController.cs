using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BendeYaparim.Web.Models;
using BendeYaparim.Web.DAL;

namespace BendeYaparim.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public ViewResult Index()
        {
            return View(categoryRepository.AllFirstLevelCategories());
        }

        public ViewResult SeekSubCategories(int Id)
        {
            return View(categoryRepository.CategoryWithChildren(Id));
        }

        public ViewResult OfferSubCategories(int Id)
        {
            return View(categoryRepository.CategoryWithChildren(Id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.InsertOrUpdate(category);
                categoryRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Category/Edit/5

        public ActionResult Edit(int id)
        {
            return View(categoryRepository.Find(id));
        }

        //
        // POST: /Category/Edit/5

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.InsertOrUpdate(category);
                categoryRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Category/Delete/5

        public ActionResult Delete(int id)
        {
            return View(categoryRepository.Find(id));
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            categoryRepository.Delete(id);
            categoryRepository.Save();

            return RedirectToAction("Index");
        }

        public PartialViewResult MainCategories()
        {
            return PartialView(categoryRepository.AllFirstLevelCategories());
        }

        public PartialViewResult Top20JobOfferCategory()
        {
            return PartialView(categoryRepository.Top20JobOfferCategory());
        }

        [OutputCache(Duration = 3000)]
        public PartialViewResult Top20JobSeekCategory()
        {
            return PartialView(categoryRepository.Top20JobSeekCategory());
        }

        [HttpGet]
        public ActionResult CategoryAdvice()
        {
            SendMessageToAdmin m = new SendMessageToAdmin();
            return View(m);
        }

        [HttpPost]
        public ActionResult CategoryAdvice(SendMessageToAdmin message)
        {
            if (ModelState.IsValid)
            {
                EmailManager.SendEmail(message);
                RedirectToAction("MessageSent");
            }

            return View();
        }
        
        public ActionResult MessageSent()
        {
            return View();
        }

    }
}

