using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BendeYaparim.Web.Infrastructure.Attributes
{
    public class PhoneAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public PhoneAttribute()
            : base(@"\(?(\d{3})\)?-?(\d{3})-(\d{4})")
        {
        }

        IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = FormatErrorMessage(metadata.GetDisplayName());

            yield return new EmailValidationRule(errorMessage);
        }
    }

    public class EmailValidationRule : ModelClientValidationRule
    {
        public EmailValidationRule(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ValidationType = "phone";
        }
    }
}