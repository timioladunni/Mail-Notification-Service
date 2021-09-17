using System;
using System.Collections.Generic;

#nullable disable

namespace MailNotificationService.Models
{
    public partial class WelcomeMail
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string RecipientEmail { get; set; }
        public string Password { get; set; }
        public long? UniqueUserCode { get; set; }
    }
}
