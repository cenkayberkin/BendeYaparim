using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BendeYaparim.Web.Models;
using BendeYaparim.Web.DAL;
using PagedList;
using BendeYaparim.Web.ViewModels;

namespace BendeYaparim.Web.Controllers
{
    public class JobSeekController : Controller
    {
        private readonly IJobSeekRepository jobseekRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ICityRepository cityRepository;
        private readonly UserRepository userRepository;

        public JobSeekController(IJobSeekRepository jobseekRepository, ICategoryRepository categoryRep, ICityRepository cityRep, UserRepository userRepo)
        {
            this.jobseekRepository = jobseekRepository;
            this.categoryRepository = categoryRep;
            this.cityRepository = cityRep;
            this.userRepository = userRepo;
        }

        public ViewResult Index(string sortOrder, int CategoryId, int? page, int CityId = 0)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CategoryWithParent = categoryRepository.CategoryWithParent(CategoryId);
            ViewBag.CategoryId = CategoryId;
            ViewBag.PriceSortParm = String.IsNullOrEmpty(sortOrder) ? "Price desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date desc" : "Date";
            ViewBag.SelectedCity = CityId;

            var jobSeeks = jobseekRepository.AllIncluding(a => a.Owner, a => a.City, a => a.Category).Where(a => a.Category.Id == CategoryId);

            //if cityid is 0  then it means all Turkey.
            if (CityId != 0)
            {
                jobSeeks = jobSeeks.Where(a => a.City.Id == CityId);
            }

            if (Request.HttpMethod != "GET" || page == 0)
            {
                page = 1;
            }

            switch (sortOrder)
            {
                case "Price desc":
                    jobSeeks = jobSeeks.OrderByDescending(s => s.Price);
                    break;
                case "Date":
                    jobSeeks = jobSeeks.OrderBy(s => s.CreateDate);
                    break;
                case "Date desc":
                    jobSeeks = jobSeeks.OrderByDescending(s => s.CreateDate);
                    break;
                default:
                    jobSeeks = jobSeeks.OrderBy(s => s.Price);
                    break;
            }

            int pageSize = 10;
            int pageIndex = (page ?? 1) - 1;

            return View(jobSeeks.ToPagedList(pageIndex, pageSize));
        }

        public PartialViewResult Cities(int CategoryId, int SelectedCityId)
        {
            ViewBag.SelectedCity = SelectedCityId;
            ViewBag.CategoryID = CategoryId;

            var seeks = jobseekRepository.AllIncluding(a => a.Category).Where(a => a.Category.Id == CategoryId);

            var sList = seeks.ToList();

            var data = from s in seeks
                       group s by s.City into cityGroup
                       select new CityAdvertNumberGroup()
                       {
                           City = cityGroup.Key,
                           AdvertNumber = cityGroup.Count()
                       };

            var dList = data.ToList();
            return PartialView(dList);
        }

        public ViewResult Details(int id)
        {
            return View(jobseekRepository.Find(id));
        }

        //
        // GET: /JobSeek/Create
        [Authorize(Roles = "Customer")]
        public ActionResult CreateJobSeek(int CategoryId)
        {
            JobSeek newJobSeek = new JobSeek();

            ViewBag.Category = categoryRepository.CategoryWithParent(CategoryId);
            ViewBag.CatID = CategoryId;

            var cities = cityRepository.All.ToList();
            List<SelectListItem> citySelectList = new List<SelectListItem>();
            citySelectList.Add(new SelectListItem() { Text = "Þehir Seç", Value = "" });
            cities.ForEach(a => citySelectList.Add(new SelectListItem() { Text = a.Name, Value = a.Id.ToString() }));
            ViewBag.Cities = citySelectList;

            List<SelectListItem> periodSelectList = new List<SelectListItem>();
            periodSelectList.Add(new SelectListItem() { Text = "Saati", Value = "0" });
            periodSelectList.Add(new SelectListItem() { Text = "Günlük", Value = "1" });
            periodSelectList.Add(new SelectListItem() { Text = "Haftalýk", Value = "2" });
            periodSelectList.Add(new SelectListItem() { Text = "Aylýk", Value = "3" });
            ViewBag.Period = periodSelectList;

            return View(newJobSeek);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public ActionResult CreateJobSeek(int CatId, JobSeek jobseek)
        {
            User s = userRepository.GetUserModel(HttpContext.User.Identity.Name);
            if (ModelState.IsValid)
            {
                Category c = categoryRepository.CategoryWithParent(CatId);
                c.NumberOfJobSeeks++;

                jobseek.CategoryId = CatId;
                jobseek.UserId = s.UserId;
                jobseekRepository.Insert(jobseek);
                jobseekRepository.Save();
                //bu redirect ilani basariyla verdiiniz yazan bir sayfaya gidecek.
                return RedirectToAction("JobSeekAdded");
            }
            else
            {
                ViewBag.Category = categoryRepository.CategoryWithParent(CatId);
                ViewBag.CatID = CatId;

                var cities = cityRepository.All.ToList();
                List<SelectListItem> citySelectList = new List<SelectListItem>();
                citySelectList.Add(new SelectListItem() { Text = "Þehir Seç", Value = "" });
                cities.ForEach(a => citySelectList.Add(new SelectListItem() { Text = a.Name, Value = a.Id.ToString() }));
                ViewBag.Cities = citySelectList;

                List<SelectListItem> periodSelectList = new List<SelectListItem>();
                periodSelectList.Add(new SelectListItem() { Text = "Saati", Value = "0" });
                periodSelectList.Add(new SelectListItem() { Text = "Günlük", Value = "1" });
                periodSelectList.Add(new SelectListItem() { Text = "Haftalýk", Value = "2" });
                periodSelectList.Add(new SelectListItem() { Text = "Aylýk", Value = "3" });
                ViewBag.Period = periodSelectList;

                return View();
            }
        }

        
        public ActionResult JobSeekAdded()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View(jobseekRepository.Find(id));
        }

        //
        // POST: /JobSeek/Edit/5

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

        [Authorize(Roles = "Customer")]
        public ActionResult Delete(int Id)
        {
            JobSeek seek = jobseekRepository.Find(Id);
            return View(seek);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            string UserName = HttpContext.User.Identity.Name;
            var User = userRepository.GetMyUser(UserName);
            jobseekRepository.Delete(id, User.UserId);
            jobseekRepository.Save();
            return RedirectToAction("MyAdverts", "User", new { UserName = HttpContext.User.Identity.Name });
        }

        [Authorize(Roles = "Customer")]
        public ActionResult SeekCategoriesForCreate1()
        {
            var categories = categoryRepository.AllFirstLevelCategories();
            return View(categories);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult SeekCategoriesForCreate2(int Id)
        {
            var categories = categoryRepository.CategoryWithChildren(Id);
            return View(categories);
        }

    }
}

