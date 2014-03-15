using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BendeYaparim.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string HtmlClassName { get; set; }
        

        public Category Parent { get; set; }
        public List<Category> Children { get; set; }

        public List<JobSeek> JobSeeks { get; set; }
        public List<JobOffer> JobOffers { get; set; }

        public int NumberOfJobSeeks { get; set; }
        public int NumberOfJobOffers { get; set; }

        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
    }
}