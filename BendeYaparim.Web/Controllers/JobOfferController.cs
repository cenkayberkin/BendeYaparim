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
    public class JobOfferController : Controller
    {
        private readonly IJobOfferRepository jobofferRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ICityRepository cityRepository;
        private readonly UserRepository userRepository;

        public JobOfferController(IJobOfferRepository jobofferRepository, ICategoryRepository categoryRep, ICityRepository cityRep, UserRepository userRepo)
        {
            this.jobofferRepository = jobofferRepository;
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

            var jobOffers = jobofferRepository.AllIncluding(a => a.Owner, a => a.City, a => a.Category).Where(a => a.Category.Id == CategoryId);

            //if cityid is 0  then it means all Turkey.
            if (CityId != 0)
            {
                jobOffers = jobOffers.Where(a => a.City.Id == CityId);
            }

            if (Request.HttpMethod != "GET" || page == 0)
            {
                page = 1;
            }

            switch (sortOrder)
            {
                case "Price desc":
                    jobOffers = jobOffers.OrderByDescending(s => s.Price);
                    break;
                case "Date":
                    jobOffers = jobOffers.OrderBy(s => s.CreateDate);
                    break;
                case "Date desc":
                    jobOffers = jobOffers.OrderByDescending(s => s.CreateDate);
                    break;
                default:
                    jobOffers = jobOffers.OrderBy(s => s.Price);
                    break;
            }

            int pageSize = 10;
            int pageIndex = (page ?? 1) - 1;

            return View(jobOffers.ToPagedList(pageIndex, pageSize));
        }

        public PartialViewResult Cities(int CategoryId, int SelectedCityId)
        {
            ViewBag.SelectedCity = SelectedCityId;
            ViewBag.CategoryID = CategoryId;

            var offers = jobofferRepository.AllIncluding(a => a.Category).Where(a => a.Category.Id == CategoryId);

            var data = from s in offers
                       group s by s.City into cityGroup
                       select new CityAdvertNumberGroup()
                       {
                           City = cityGroup.Key,
                           AdvertNumber = cityGroup.Count()
                       };

            return PartialView(data.ToList());
        }


        //CreateJobSeek


        public ViewResult Details(int id)
        {
            return View(jobofferRepository.Find(id));
        }

        //
        // GET: /JobSeek/Create
        [Authorize(Roles = "Customer")]
        public ActionResult CreateJobOffer(int CategoryId)
        {
            JobOffer newJobOffer = new JobOffer();

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

            return View(newJobOffer);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public ActionResult CreateJobOffer(int CatId, JobOffer jobOffer)
        {
            User s = userRepository.GetUserModel(HttpContext.User.Identity.Name);
            if (ModelState.IsValid)
            {
                Category c = categoryRepository.CategoryWithParent(CatId);
                c.NumberOfJobOffers++;

                jobOffer.CategoryId = CatId;
                jobOffer.UserId = s.UserId;
                jobofferRepository.Insert(jobOffer);
                jobofferRepository.Save();
                //bu redirect ilani basariyla verdiiniz yazan bir sayfaya gidecek.
                return RedirectToAction("JobOfferAdded");
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

        public ActionResult JobOfferAdded()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(JobOffer joboffer)
        {
            if (ModelState.IsValid)
            {
                jobofferRepository.InsertOrUpdate(joboffer);
                jobofferRepository.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }



        public ActionResult Edit(int id)
        {
            return View(jobofferRepository.Find(id));
        }



        [HttpPost]
        public ActionResult Edit(JobOffer joboffer)
        {
            if (ModelState.IsValid)
            {
                jobofferRepository.InsertOrUpdate(joboffer);
                jobofferRepository.Save();
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
            JobOffer seek = jobofferRepository.Find(Id);
            return View(seek);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            string UserName = HttpContext.User.Identity.Name;
            var User = userRepository.GetMyUser(UserName);
            jobofferRepository.Delete(id, User.UserId);
            jobofferRepository.Save();
            return RedirectToAction("MyAdverts", "User", new { UserName = HttpContext.User.Identity.Name });
        }


        [Authorize(Roles = "Customer")]
        public ActionResult OfferCategoriesForCreate1()
        {
            var categories = categoryRepository.AllFirstLevelCategories();
            return View(categories);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult OfferCategoriesForCreate2(int Id)
        {
            var categories = categoryRepository.CategoryWithChildren(Id);
            return View(categories);
        }
    }
}

