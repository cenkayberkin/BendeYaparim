using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BendeYaparim.Web.Infrastructure.HtmlHelpers
{
    public class MyHtmlHelper 
    {
        public static MvcHtmlString PricePeriodStr(int period)
        {
            string result = "";

            switch (period)
            {
                case 0:
                    result = "saati";
                    break;
                case 1:
                    result = "günlük";
                    break;
                case 2:
                    result = "haftalık";
                    break;
                case 3:
                    result = "aylık";
                    break;
                default:
                    result = "günlük";
                    break;
            }

            return new MvcHtmlString(result);
        }

        public static MvcHtmlString AdvertContent(string content)
        {
            if (!String.IsNullOrEmpty(content))
            {
               if (content.Length > 200)
                {
                    content = content.Substring(0, 200) + " ...";
                }
            }
            
            return new MvcHtmlString(content);
        }

        public static MvcHtmlString CityNameExtension(int extensionType)
        {
            string result;
            if (extensionType == 1)
            {
                result = "da";
            }
            else
            {
                result = "de";
            }
            return new MvcHtmlString(result);
        }

        public static object ProfileContent(string content)
        {
            if (!String.IsNullOrEmpty(content))
            {
                if (content.Length > 800)
                {
                    content = content.Substring(0, 800) + " ...";
                }
            }

            return new MvcHtmlString(content);
        }

        public static MvcHtmlString PageTitle(string content)
        {
            
            return new MvcHtmlString(content);
        }

        public static MvcHtmlString PageDescription(string content)
        {

            return new MvcHtmlString(content);
        }
    }

    
}