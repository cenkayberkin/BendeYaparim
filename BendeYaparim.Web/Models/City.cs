using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BendeYaparim.Web.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NameExtension { get; set; }

        public List<JobSeek> JobSeeks { get; set; }
        public List<JobOffer> JobOffers { get; set; }
    }
}