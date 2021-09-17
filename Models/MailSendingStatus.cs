using System;
using System.Collections.Generic;

#nullable disable

namespace MailNotificationService.Models
{
    public partial class MailSendingStatus
    {
        public long Id { get; set; }
        public string MessageType { get; set; }
        public string SendingStatus { get; set; }
        public long? UniqueUserCode { get; set; }
    }
}
