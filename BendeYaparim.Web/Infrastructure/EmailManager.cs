using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace BendeYaparim.Web
{
    public static class EmailManager
    {
        public static bool SendEmail(SendMessageToAdmin messageToAdmin)
        {
            var message = new MailMessage("info@bendeyaparim.com", "cenkayberkin@gmail.com")
            {
                Subject = "Kullanicidan mesaj var",
                Body = "baslik " + messageToAdmin.Title + "  icerik :" + messageToAdmin.Content
            };

            var client = new SmtpClient("mail.bendeyaparim.com", 587);
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential("admin", "0dpqc88w");
            client.Send(message);
            return true;

        }
    }
}