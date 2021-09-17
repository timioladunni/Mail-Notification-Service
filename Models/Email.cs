using System;
using System.Collections.Generic;

#nullable disable

namespace MailNotificationService.Models
{
    public partial class Email
    {
        public long Id { get; set; }
        public string MessageType { get; set; }
        public string Status { get; set; }
        public string Body { get; set; }
    }
}
