using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Models
{
    public class ReceiptDetails
    {
        public double AmountReceived { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public string RefSalesBillNumber { get; set; }
        public string ReceivedDate { get; set; }
        public string ModeOfTxn { get; set; }
        public string ModeOfTxnRefNo { get; set; }
        public string ModeOfTxnDate { get; set; }
    }
}
