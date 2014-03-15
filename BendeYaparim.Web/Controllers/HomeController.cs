using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BendeYaparim.Web.Models;
using BendeYaparim.Web.DAL;

namespace BendeYaparim.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobSeekRepository jobseekRepository;

        public HomeController(IJobSeekRepository jobseekRepository)
        {
            this.jobseekRepository = jobseekRepository;
        }

        public ViewResult Index()
        {
            return View();
        }
       
        public PartialViewResult IndexAdverts()
        {
            List<JobSeek> seeks = jobseekRepository.AllIncluding(a => a.Owner, a => a.Category, a => a.City).OrderByDescending(a => a.CreateDate)
               .Take(12).ToList();
            return PartialView(seeks);
        }
        
        public ViewResult Trouble()
        {

            return View();
        }

        public ViewResult Lost()
        {
            return View();
        }


        public ViewResult Details(int id)
        {
            return View(jobseekRepository.Find(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(JobSeek jobseek)
        {
            if (ModelState.IsValid)
            {
                jobseekRepository.InsertOrUpdate(jobseek);
                jobseekRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Home/Edit/5

        public ActionResult Edit(int id)
        {
            return View(jobseekRepository.Find(id));
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        public ActionResult Edit(JobSeek jobseek)
        {
            if (ModelState.IsValid)
            {
                jobseekRepository.InsertOrUpdate(jobseek);
                jobseekRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(int id)
        {
            return View(jobseekRepository.Find(id));
        }

        //
        // POST: /Home/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, int UserId)
        {
            jobseekRepository.Delete(id, UserId);
            jobseekRepository.Save();

            return RedirectToAction("Index");
        }
    }
}

