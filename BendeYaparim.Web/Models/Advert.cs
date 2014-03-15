using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using BendeYaparim.Web.Infrastructure.Attributes;

namespace BendeYaparim.Web.Models
{
    public abstract class Advert
    {
        public Advert()
        {
            CreateDate = DateTime.Now;
        }

        public int Id { get; set; }

        [MaxLength(800,ErrorMessage="İçerik 800 karakterden fazla olamaz.")]
        [DataType(DataType.MultilineText)]
        public string ContentBody { get; set; }

        public DateTime CreateDate { get; set; }

        public int DisplayCount { get; set; }

        [Required(ErrorMessage = "Fiyat gerekli.")]
        [DataType(DataType.Currency,ErrorMessage="Geçerli fiyat değil")]
        [PriceValidation]
        public double? Price { get; set; }

        //fiyat haftalikmi yoksa aylik mi yoksa gunluk mu.
        //0 gunluk, 1 haftalik, 2 aylik . default gunluk olucak.
        [Required(ErrorMessage = "Ücret süresi gerekli.")]
        public int PricePeriod { get; set; }
 
        [Required(ErrorMessage = "Şehir gerekli.")]
        public int CityId { get; set; }
      
        public City City { get; set; }

        public int UserId { get; set; }

        public User Owner { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}