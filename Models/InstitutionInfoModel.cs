using System;
using System.Collections.Generic;
using System.Text;

namespace MailNotificationService.Models
{
    class InstitutionInfoModel
    {
        public string InstitutionName { get; set; }
        public decimal RepaymentAccount { get; set; }
        public decimal Threshold { get; set; }
        public decimal CurrentAccount { get; set; }
        public string Email { get; set; }
    }
}
