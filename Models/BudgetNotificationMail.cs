using System;
using System.Collections.Generic;

#nullable disable

namespace MailNotificationService.Models
{
    public partial class BudgetNotificationMail
    {
        public long Id { get; set; }
        public string InstitutionName { get; set; }
        public decimal? RepaymentAccount { get; set; }
        public decimal? Amount { get; set; }
        public long? UniqueUserCode { get; set; }
        public string RecepientMail { get; set; }
    }
}
