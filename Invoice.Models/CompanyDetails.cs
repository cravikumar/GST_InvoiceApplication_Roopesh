using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Models
{
    public class CompanyDetails
    {
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public string Address { get; set; }
        public string PhoneNumbers { get; set; }
        public bool IsGSTApplicable { get; set; }
        public string GSTIN { get; set; }
        public string PANCard { get; set; }
        public string Aadhaar { get; set; }
        public string PropriterName { get; set; }
        public string BankName { get; set; }
        public string BankAccNo { get; set; }
        public string BankBranchAddress { get; set; }
        public string IFSCCode { get; set; }
        public string BOAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }

        public int BillNo { get; set; }
        public DateTime BillDate { get; set; }
        public string BillPrefix { get; set; }

    }
}
