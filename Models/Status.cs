using System;
using System.Collections.Generic;
using System.Text;

namespace MailNotificationService.Models
{
    public enum Status
    {
        Pending,
        Sent
    }

    public enum MessageType
    {
        WelcomeMail,
        BudgetNotificationMail
    }

    public enum SubjectType
    {
        Welcome,
        ThresholdNotification
    }
}
