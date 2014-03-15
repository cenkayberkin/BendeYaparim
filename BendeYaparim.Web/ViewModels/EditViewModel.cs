using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using BendeYaparim.Web.Infrastructure.Attributes;

namespace BendeYaparim.Web.ViewModels
{
    public class EditViewModel
    {
        public int EmailShowing { get; set; }

        public int PhoneShowing { get; set; }

        [Required(ErrorMessage = "Telefon numarası gerekli.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

    }
}