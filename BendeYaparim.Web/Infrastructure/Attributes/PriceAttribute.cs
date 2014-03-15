using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BendeYaparim.Web.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PriceValidationAttribute : ValidationAttribute
    {
        private double minPrice = 0;
        private double maxPrice = 90000;

        public PriceValidationAttribute()
            : base("Fiyat doğru aralıkta değil")
        {
        }
        public override bool IsValid(object value)
        {
            double price = (double)value;
            if (price < this.minPrice || price > this.maxPrice)
                return false;
            return true;
        }
    }
}