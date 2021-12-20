using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Models
{
    public class PaymentDetails
    {
        public double Id { get; set; }
        public double AmountPaid { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public string RefPurchaseBillNumber { get; set; }
        public string PaymentDate { get; set; }
        public string ModeOfTxn { get; set; }
        public string ModeOfTxnRefNo { get; set; }
        public string ModeOfTxnDate { get; set; }
    }
}
