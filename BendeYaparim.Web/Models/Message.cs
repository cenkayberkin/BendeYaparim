using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BendeYaparim.Web.Models
{
    public class Message
    {
        public Message()
        {
            SentAt = DateTime.Now;
        }

        public int MessageId { get; set; }

        public int FromUserId { get; set; }

        [ForeignKey("FromUserId")]
        public User From { get; set; }

        public int ToUserId { get; set; }

        [ForeignKey("ToUserId")]
        public User To { get; set; }

        public DateTime SentAt { get; set; }

        [MaxLength(500, ErrorMessage = "Mesaj 500 karakterden fazla olamaz.")]
        [DataType(DataType.MultilineText)]
        public string ContentBody { get; set; }
  }
}