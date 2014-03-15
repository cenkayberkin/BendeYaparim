using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BendeYaparim.Web
{
    public static class CategoryNameConverterToUrl
    {
            public static string ToFriendlyUrl(string urlToEncode)
            {
                urlToEncode = (urlToEncode ?? "").Trim().ToLower();

                StringBuilder url = new StringBuilder();

                foreach (char ch in urlToEncode)
                {
                    switch (ch)
                    {
                        case ' ':
                            url.Append('-');
                            break;
                        case '&':
                            url.Append("ve");
                            break;
                        case '\'':
                            break;
                        case 'ı':
                            url.Append("i");
                            break;
                        case 'ç':
                            url.Append("c");
                            break;
                        case 'ö':
                            url.Append("o");
                            break;
                        case 'ş':
                            url.Append("s");
                            break;
                        case 'ü':
                            url.Append("u");
                            break;
                        case 'ğ':
                            url.Append("g");
                            break;
                        default:
                            if ((ch >= '0' && ch <= '9') ||
                                (ch >= 'a' && ch <= 'z'))
                            {
                                url.Append(ch);
                            }
                            else
                            {
                                url.Append('-');
                            }
                            break;
                    }
                }

                return url.ToString();
            }
        
    }
}