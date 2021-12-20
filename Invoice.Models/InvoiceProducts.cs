using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Models
{
    public class InvoiceProducts
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string ProductName { get; set; }
        public string HSNCode { get; set; }
        public string Rate { get; set; }
        public string Quantity { get; set; }
        public string Total { get; set; }
        public string MtsDescription { get; set; }
        public string Mts { get; set; }
        public string PreviousRate { get; set; }
        public string RateWithoutTax { get; set; }
    }
}

