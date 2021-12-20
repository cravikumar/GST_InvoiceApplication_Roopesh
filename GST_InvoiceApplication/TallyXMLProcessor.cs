using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GST_InvoiceApplication
{
    public class TallyXMLProcessor
    {
        public string UMTPrefix = "<ENVELOPE><HEADER> <TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY> <IMPORTDATA>  <REQUESTDESC>   <REPORTNAME>All Masters</REPORTNAME>   <STATICVARIABLES>    <SVCURRENTCOMPANY>Umamaheswara Textiles</SVCURRENTCOMPANY>   </STATICVARIABLES>  </REQUESTDESC>  <REQUESTDATA>   <TALLYMESSAGE xmlns:UDF=\"TallyUDF\">   ";
        public string UMTSGSTVocher = "<VOUCHER  VCHTYPE=\"Sales\" ACTION=\"Create\" OBJVIEW=\"Invoice Voucher View\"><OLDAUDITENTRYIDS.LIST TYPE=\"Number\"> <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><DATE>##DATE##</DATE><VOUCHERTYPENAME>Sales</VOUCHERTYPENAME><REFERENCE>##INVOICENO##</REFERENCE><VOUCHERNUMBER>##INVOICENO##</VOUCHERNUMBER><PARTYLEDGERNAME>##CUSTOMERNAME##</PARTYLEDGERNAME><EFFECTIVEDATE>##DATE##</EFFECTIVEDATE><ISINVOICE>Yes</ISINVOICE><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>  <LEDGERNAME>##CUSTOMERNAME##</LEDGERNAME> <GSTCLASS/> <ISPARTYLEDGER>Yes</ISPARTYLEDGER> <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE> <AMOUNT>-##TOTAL##</AMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Sale @ 5 %</LEDGERNAME> <AMOUNT>##AMOUNT##</AMOUNT> <VATEXPAMOUNT>##AMOUNT##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Cgst Output @2.5%</LEDGERNAME> <AMOUNT>##CGST##</AMOUNT> <VATEXPAMOUNT>##CGST##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>        <LEDGERNAME>Sgst Output @ 2.5%</LEDGERNAME> <AMOUNT>##CGST##</AMOUNT> <VATEXPAMOUNT>##CGST##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <ROUNDTYPE>Normal Rounding</ROUNDTYPE> <LEDGERNAME>Round Off</LEDGERNAME> <ROUNDLIMIT> 1</ROUNDLIMIT> <AMOUNT>##ROUND##</AMOUNT> <VATEXPAMOUNT>##ROUND##</VATEXPAMOUNT> </LEDGERENTRIES.LIST>     </VOUCHER>";
        public string UMTIGSTVocher = "<VOUCHER  VCHTYPE=\"Sales\" ACTION=\"Create\" OBJVIEW=\"Invoice Voucher View\"><OLDAUDITENTRYIDS.LIST TYPE=\"Number\"> <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><DATE>##DATE##</DATE><VOUCHERTYPENAME>Sales</VOUCHERTYPENAME><REFERENCE>##INVOICENO##</REFERENCE><VOUCHERNUMBER>##INVOICENO##</VOUCHERNUMBER><PARTYLEDGERNAME>##CUSTOMERNAME##</PARTYLEDGERNAME><EFFECTIVEDATE>##DATE##</EFFECTIVEDATE><ISINVOICE>Yes</ISINVOICE><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>  <LEDGERNAME>##CUSTOMERNAME##</LEDGERNAME> <GSTCLASS/> <ISPARTYLEDGER>Yes</ISPARTYLEDGER> <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE> <AMOUNT>-##TOTAL##</AMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Igst Sales @ 5%</LEDGERNAME> <AMOUNT>##AMOUNT##</AMOUNT> <VATEXPAMOUNT>##AMOUNT##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Igst Output @ 5%</LEDGERNAME> <AMOUNT>##IGST##</AMOUNT> <VATEXPAMOUNT>##IGST##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <ROUNDTYPE>Normal Rounding</ROUNDTYPE> <LEDGERNAME>Round Off</LEDGERNAME> <ROUNDLIMIT> 1</ROUNDLIMIT> <AMOUNT>##ROUND##</AMOUNT> <VATEXPAMOUNT>##ROUND##</VATEXPAMOUNT> </LEDGERENTRIES.LIST>     </VOUCHER>";
        public string UMTSuffix = "</TALLYMESSAGE><TALLYMESSAGE xmlns:UDF=\"TallyUDF\"> <COMPANY>  <REMOTECMPINFO.LIST MERGE=\"Yes\">   <NAME>c11ad8d7-08cd-44d8-bfef-bb63c7c785a0</NAME>   <REMOTECMPNAME>Umamaheswara Textiles</REMOTECMPNAME>   <REMOTECMPSTATE>Andhra Pradesh</REMOTECMPSTATE>  </REMOTECMPINFO.LIST> </COMPANY></TALLYMESSAGE></REQUESTDATA></IMPORTDATA> </BODY></ENVELOPE> ";

        public string SSTPrefix = "<ENVELOPE><HEADER> <TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY> <IMPORTDATA>  <REQUESTDESC>   <REPORTNAME>All Masters</REPORTNAME>   <STATICVARIABLES>    <SVCURRENTCOMPANY>Sri Sita Rama Textiles</SVCURRENTCOMPANY>   </STATICVARIABLES>  </REQUESTDESC>  <REQUESTDATA>   <TALLYMESSAGE xmlns:UDF=\"TallyUDF\">   ";
        public string SSTSGSTVocher = "<VOUCHER  VCHTYPE=\"Sales\" ACTION=\"Create\" OBJVIEW=\"Invoice Voucher View\"><OLDAUDITENTRYIDS.LIST TYPE=\"Number\"> <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><DATE>##DATE##</DATE><VOUCHERTYPENAME>Sales</VOUCHERTYPENAME><REFERENCE>##INVOICENO##</REFERENCE><VOUCHERNUMBER>##INVOICENO##</VOUCHERNUMBER><PARTYLEDGERNAME>##CUSTOMERNAME##</PARTYLEDGERNAME><EFFECTIVEDATE>##DATE##</EFFECTIVEDATE><ISINVOICE>Yes</ISINVOICE><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>  <LEDGERNAME>##CUSTOMERNAME##</LEDGERNAME> <GSTCLASS/> <ISPARTYLEDGER>Yes</ISPARTYLEDGER> <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE> <AMOUNT>-##TOTAL##</AMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Sales @ 5%</LEDGERNAME> <AMOUNT>##AMOUNT##</AMOUNT> <VATEXPAMOUNT>##AMOUNT##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Cgst Output @2.5%</LEDGERNAME> <AMOUNT>##CGST##</AMOUNT> <VATEXPAMOUNT>##CGST##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>        <LEDGERNAME>Sgst Output @2.5%</LEDGERNAME> <AMOUNT>##CGST##</AMOUNT> <VATEXPAMOUNT>##CGST##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <ROUNDTYPE>Normal Rounding</ROUNDTYPE> <LEDGERNAME>Round Off</LEDGERNAME> <ROUNDLIMIT> 1</ROUNDLIMIT> <AMOUNT>##ROUND##</AMOUNT> <VATEXPAMOUNT>##ROUND##</VATEXPAMOUNT> </LEDGERENTRIES.LIST>     </VOUCHER>";
        public string SSTIGSTVocher = "<VOUCHER  VCHTYPE=\"Sales\" ACTION=\"Create\" OBJVIEW=\"Invoice Voucher View\"><OLDAUDITENTRYIDS.LIST TYPE=\"Number\"> <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS></OLDAUDITENTRYIDS.LIST><DATE>##DATE##</DATE><VOUCHERTYPENAME>Sales</VOUCHERTYPENAME><REFERENCE>##INVOICENO##</REFERENCE><VOUCHERNUMBER>##INVOICENO##</VOUCHERNUMBER><PARTYLEDGERNAME>##CUSTOMERNAME##</PARTYLEDGERNAME><EFFECTIVEDATE>##DATE##</EFFECTIVEDATE><ISINVOICE>Yes</ISINVOICE><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>  <LEDGERNAME>##CUSTOMERNAME##</LEDGERNAME> <GSTCLASS/> <ISPARTYLEDGER>Yes</ISPARTYLEDGER> <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE> <AMOUNT>-##TOTAL##</AMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Igst Sales @ 5%</LEDGERNAME> <AMOUNT>##AMOUNT##</AMOUNT> <VATEXPAMOUNT>##AMOUNT##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <LEDGERNAME>Igst Output @ 5%</LEDGERNAME> <AMOUNT>##IGST##</AMOUNT> <VATEXPAMOUNT>##IGST##</VATEXPAMOUNT> </LEDGERENTRIES.LIST><LEDGERENTRIES.LIST><ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>  <ROUNDTYPE>Normal Rounding</ROUNDTYPE> <LEDGERNAME>Round Off</LEDGERNAME> <ROUNDLIMIT> 1</ROUNDLIMIT> <AMOUNT>##ROUND##</AMOUNT> <VATEXPAMOUNT>##ROUND##</VATEXPAMOUNT> </LEDGERENTRIES.LIST>     </VOUCHER>";
        public string SSTSuffix = "</TALLYMESSAGE><TALLYMESSAGE xmlns:UDF=\"TallyUDF\"> <COMPANY>  <REMOTECMPINFO.LIST MERGE=\"Yes\">   <NAME>2e632762-e1c7-42a3-9f3f-f02e20a69e16</NAME>   <REMOTECMPNAME>Sri Sita Rama Textiles</REMOTECMPNAME>   <REMOTECMPSTATE>Andhra Pradesh</REMOTECMPSTATE>  </REMOTECMPINFO.LIST> </COMPANY></TALLYMESSAGE></REQUESTDATA></IMPORTDATA> </BODY></ENVELOPE> ";



        public string GetTallyXML(DataTable dt,int companyID)
        {
            if (companyID == 1)
                return ProcessUMTXML(dt);
            else if (companyID == 2)
                return ProcessSSTXML(dt);
            else
                throw new MissingMethodException();
        }

        public string ProcessUMTXML(DataTable dt)
        {
            string XML = UMTPrefix;
            foreach (DataRow dr in dt.Rows)
            {
                XML = XML +
                GetUMTLedgerData(
                    dr[11].ToString().Replace("&","&amp;"),//custname
                dr[0].ToString(),//billno
                Convert.ToDateTime(dr[1]).ToString("yyyyMMdd"), //date
                dr[4].ToString(),//beforetax
                dr[5].ToString(),//cgst
                dr[7].ToString(),//igst
                dr[8].ToString(),//round
                dr[9].ToString());//grandtotal
            }

            return XML + UMTSuffix;
        }


        public string GetUMTLedgerData(string ledgerName, string billno, string date, string beforetaxtotal, string cgst, string igst, string round, string fulltotal)
        {
            string ledger = UMTSGSTVocher;
            if (!string.IsNullOrEmpty(igst))
            {
                ledger = UMTIGSTVocher;
            }

            ledger = ledger.Replace("##DATE##", date);
            ledger = ledger.Replace("##INVOICENO##", billno);
            ledger = ledger.Replace("##CUSTOMERNAME##", ledgerName);
            ledger = ledger.Replace("##TOTAL##", fulltotal);
            ledger = ledger.Replace("##AMOUNT##", beforetaxtotal);
            ledger = ledger.Replace("##CGST##", cgst);
            ledger = ledger.Replace("##IGST##", igst);
            ledger = ledger.Replace("##ROUND##", round);

            return ledger;
        }


        public string ProcessSSTXML(DataTable dt)
        {
            string XML = SSTPrefix;
            foreach (DataRow dr in dt.Rows)
            {
                XML = XML +
                GetSSTLedgerData(
                    dr[11].ToString().Replace("&", "&amp;"),//custname
                dr[0].ToString(),//billno
                Convert.ToDateTime(dr[1]).ToString("yyyyMMdd"), //date
                dr[4].ToString(),//beforetax
                dr[5].ToString(),//cgst
                dr[7].ToString(),//igst
                dr[8].ToString(),//round
                dr[9].ToString());//grandtotal
            }

            return XML + SSTSuffix;
        }


        public string GetSSTLedgerData(string ledgerName, string billno, string date, string beforetaxtotal, string cgst, string igst, string round, string fulltotal)
        {
            string ledger = SSTSGSTVocher;
            if (!string.IsNullOrEmpty(igst))
            {
                ledger = SSTIGSTVocher;
            }

            ledger = ledger.Replace("##DATE##", date);
            ledger = ledger.Replace("##INVOICENO##", billno);
            ledger = ledger.Replace("##CUSTOMERNAME##", ledgerName);
            ledger = ledger.Replace("##TOTAL##", fulltotal);
            ledger = ledger.Replace("##AMOUNT##", beforetaxtotal);
            ledger = ledger.Replace("##CGST##", cgst);
            ledger = ledger.Replace("##IGST##", igst);
            ledger = ledger.Replace("##ROUND##", round);

            return ledger;
        }
    }
}
