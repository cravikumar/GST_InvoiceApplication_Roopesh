using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Models
{
    public class InvoiceDetails
    {
        public int SaleId { get; set; }
        public int CompanyId { get; set; }
        public string InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string StrBillTotal { get; set; }
        public string StrDiscount { get; set; }
        public string StrTotalAfterDiscountOrBeforeTax { get; set; }
        public string StrSGST { get; set; }
        public string StrCGST { get; set; }
        public string StrIGST { get; set; }
        public string StrTotalAfterTax { get; set; }
        public string StrRounded { get; set; }
        public string StrTotalPayable { get; set; }

        public int CustomerId { get; set; }
        public bool IsPaid { get; set; }
        public List<InvoiceProducts> Products { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGST { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPanAadhaar { get; set; }

        public string StrTransport { get; set; }
        public string StrLRNo { get; set; }
        public string CGSTValue { get; set; }
        public string SGSTValue { get; set; }
        public string IGSTValue { get; set; }

    }
}
