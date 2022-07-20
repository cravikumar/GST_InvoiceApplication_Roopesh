using Invoice.Models;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GST_InvoiceApplication
{
    public partial class InvoicePrint : Form
    {
        string invoiceid;
        bool IsGstForm;
        InvoiceDetails _currentInvoice;
        private static List<Stream> m_streams;
        private static int m_currentPageIndex = 0;
        private static int productCount = 0;
        
        CompanyDetails _currentCompany;
        public InvoicePrint(InvoiceDetails inv,CompanyDetails currentCompany,bool withOutLoading = false)
        {
            this.WindowState = FormWindowState.Maximized;

            this._currentInvoice = inv;
            int count = 0;
            foreach(var prod in _currentInvoice.Products)
            {
                count = count + (int)Math.Ceiling((double)prod.ProductName.Length / 18);
            }

            productCount = count;
            this._currentCompany = currentCompany;
            if (withOutLoading)
            {
                if (_currentCompany.ThermalPrinter)
                {
                    PrintWithoutLoading();
                    return;
                }
                PrintWithoutLoadingA4();
                return;
            }
            InitializeComponent();
        }

        private void PrintWithoutLoadingA4() {
            LocalReport report = new LocalReport();
            var asem = Assembly.GetExecutingAssembly().Location;
            var path = System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString();

            string originalInvoicePath = "InvoiceOriginal";
            if(_currentInvoice.InvoiceDate < new DateTime(2021, 08, 01))
            {
                originalInvoicePath = "\\InvoiceOriginal.rdlc";
            }
            else if(!string.IsNullOrEmpty(_currentInvoice.Notes) && _currentInvoice.Notes.Trim().Length>0)
                originalInvoicePath = "\\InvoiceOriginal_WithNotesV2.rdlc";
            else
                originalInvoicePath = "\\InvoiceOriginalV2.rdlc";


            if (!File.Exists(path + originalInvoicePath))
                MessageBox.Show("File Not present at " + path + originalInvoicePath);

            if (_currentCompany.IsGSTApplicable)
                report.ReportPath = path + originalInvoicePath;
            else
                report.ReportPath = path + "\\InvoiceOriginalNoGST.rdlc";
            report.EnableExternalImages = true;
            DataTable table = new DataTable();
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("HSNCode", typeof(string));
            table.Columns.Add("Rate", typeof(string));
            table.Columns.Add("Quantity", typeof(string));
            table.Columns.Add("MtsDescription", typeof(string));
            table.Columns.Add("Mts", typeof(string));
            table.Columns.Add("Total", typeof(string));



            foreach (InvoiceProducts p in _currentInvoice.Products)
            {
                table.Rows.Add(
                    p.ProductName + (string.IsNullOrEmpty(p.MtsDescription) ? "" : "   (" + p.MtsDescription + ")")
                    , p.HSNCode + "---" + isVarientHigh(p.PreviousRate, p.Rate)
                    , string.Format("{0:n}", Convert.ToDouble(p.Rate)) + "---" + string.Format("{0:n}", Convert.ToDouble(p.RateWithoutTax))
                    //, string.Format("{0:n}", Convert.ToDouble(p.Rate))//Convert.ToDouble(p.Rate).ToString("C", CultureInfo.CurrentCulture)
                    , p.Quantity
                    , p.MtsDescription
                    , p.Mts
                    , string.Format("{0:n}", Convert.ToDouble(p.Total)//Convert.ToDouble(p.Total).ToString("C", CultureInfo.CurrentCulture
                    ));
            }

            ReportDataSource rdsprod = new ReportDataSource("InvoiceProductSet", table);
            report.DataSources.Add(rdsprod);


            ReportParameter p1 = new ReportParameter("CompanyName", _currentCompany.CompanyName);
            ReportParameter p2 = new ReportParameter("CompanyAddressOneLine", _currentCompany.Address);
            ReportParameter p3 = new ReportParameter("PhoneNumbers", _currentCompany.PhoneNumbers);
            ReportParameter p4 = new ReportParameter("CompanyGSTNo", _currentCompany.GSTIN);
            ReportParameter p5 = new ReportParameter("BillType", _currentInvoice.IsPaid ? "CASH" : "CREDIT");

            ReportParameter p6 = new ReportParameter("BillNo", _currentInvoice.InvoiceId);
            ReportParameter p7 = new ReportParameter("BillDate", _currentInvoice.InvoiceDate.ToShortDateString());
            ReportParameter p8 = new ReportParameter("TransportName", string.IsNullOrEmpty(_currentInvoice.StrTransport) ? "__________________" : _currentInvoice.StrTransport);
            ReportParameter p9 = new ReportParameter("LRNo", string.IsNullOrEmpty(_currentInvoice.StrLRNo) ? "__________________" : _currentInvoice.StrLRNo);

            ReportParameter p10 = new ReportParameter("CustomerName", string.IsNullOrEmpty(_currentInvoice.CustomerName) ? "-" : _currentInvoice.CustomerName);
            ReportParameter p11 = new ReportParameter("CustomerAddress", string.IsNullOrEmpty(_currentInvoice.CustomerAddress) ? "-" : _currentInvoice.CustomerAddress);
            ReportParameter p12 = new ReportParameter("CustomerGST", string.IsNullOrEmpty(_currentInvoice.CustomerGST) ? "-" : _currentInvoice.CustomerGST);
            ReportParameter p13 = new ReportParameter("AadhaarPAN", string.IsNullOrEmpty(_currentInvoice.CustomerPanAadhaar) ? "-" : _currentInvoice.CustomerPanAadhaar);

            ReportParameter p14 = new ReportParameter("BankName", _currentCompany.BankName + " - " + _currentCompany.BankBranchAddress);
            ReportParameter p15 = new ReportParameter("AccNo", _currentCompany.BankAccNo);
            ReportParameter p16 = new ReportParameter("IFSCCode", _currentCompany.IFSCCode);

            ReportParameter p31 = new ReportParameter("StrBillTotal", _currentInvoice.StrBillTotal);
            ReportParameter p17 = new ReportParameter("Discount", _currentInvoice.StrDiscount);
            ReportParameter p18 = new ReportParameter("AmountBeforeTax", _currentInvoice.StrTotalAfterDiscountOrBeforeTax);
            ReportParameter p19 = new ReportParameter("SGST", string.IsNullOrEmpty(_currentInvoice.StrSGST) ? "0" : _currentInvoice.StrSGST);
            ReportParameter p20 = new ReportParameter("CGST", string.IsNullOrEmpty(_currentInvoice.StrCGST) ? "0" : _currentInvoice.StrCGST);
            ReportParameter p21 = new ReportParameter("IGST", string.IsNullOrEmpty(_currentInvoice.StrIGST) ? "0" : _currentInvoice.StrIGST);
            ReportParameter p22 = new ReportParameter("AmountAfterTax", _currentInvoice.StrTotalAfterTax);
            ReportParameter p23 = new ReportParameter("RoundValue", Convert.ToDouble(_currentInvoice.StrRounded) > 0 ? "+ " + _currentInvoice.StrRounded : _currentInvoice.StrRounded);
            ReportParameter p24 = new ReportParameter("TotalPayable", _currentInvoice.StrTotalPayable);

            ReportParameter p25 = new ReportParameter("CustomerPhone", string.IsNullOrEmpty(_currentInvoice.CustomerMobile) ? "-" : _currentInvoice.CustomerMobile);

            ReportParameter p26 = new ReportParameter("CGSTPer", string.IsNullOrEmpty(_currentInvoice.CGSTValue) ? "-" : "( " + _currentInvoice.CGSTValue + " % )");
            ReportParameter p27 = new ReportParameter("SGSTPer", string.IsNullOrEmpty(_currentInvoice.SGSTValue) ? "-" : "( " + _currentInvoice.SGSTValue + " % )");
            ReportParameter p28 = new ReportParameter("IGSTPer", string.IsNullOrEmpty(_currentInvoice.IGSTValue) ? "-" : "( " + _currentInvoice.IGSTValue + " % )");

            ReportParameter p29 = new ReportParameter("BOAddress", string.IsNullOrEmpty(_currentCompany.BOAddress) ? "." : _currentCompany.BOAddress);
            ReportParameter p30 = new ReportParameter("Totalinwords", "-");
            ReportParameter p32 = new ReportParameter("Notes", string.IsNullOrEmpty(_currentInvoice.Notes) ? "-" : _currentInvoice.Notes);

            report.SetParameters(new ReportParameter[] { 
                p1, p2, p3, p4, p5,
                p6, p7, p8, p9, p10,
                p11, p12, p13, p14, p15,
                p16, p17, p18, p19, p20,
                p21, p22, p23, p24, p25,
                p26,p27,p28, p29, p30,p31,p32
            });

            PrintToPrinter(report);
            
        
        
        }


        public InvoicePrint()
        {
            this.WindowState = FormWindowState.Maximized;
            this.IsGstForm = true;
            this.invoiceid = "UMT-0001";
            InitializeComponent();
        }

        private void PrintWithoutLoading() {
            LocalReport report = new LocalReport();
            report.ReportPath = System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString() + "\\InvoiceRollerOriginal.rdlc";
           
            DataTable table = new DataTable();
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("HSNCode", typeof(string));
            table.Columns.Add("Rate", typeof(string));
            table.Columns.Add("Quantity", typeof(string));
            table.Columns.Add("MtsDescription", typeof(string));
            table.Columns.Add("Mts", typeof(string));
            table.Columns.Add("Total", typeof(string));



            foreach (InvoiceProducts p in _currentInvoice.Products)
            {
                table.Rows.Add(
                    p.ProductName + (string.IsNullOrEmpty(p.MtsDescription) ? "" : "   (" + p.MtsDescription + ")")
                    , p.HSNCode + "---" + isVarientHigh(p.PreviousRate, p.Rate)
                    , string.Format("{0:n}", Convert.ToDouble(p.Rate)) + "---" + string.Format("{0:n}", Convert.ToDouble(p.RateWithoutTax))
                    , p.Quantity
                    , p.MtsDescription
                    , p.Mts
                    , string.Format("{0:n}", Convert.ToDouble(p.Total)//Convert.ToDouble(p.Total).ToString("C", CultureInfo.CurrentCulture
                    ));
            }

            
            ReportDataSource rdsprod = new ReportDataSource("InvoiceProductSet", table);
            report.DataSources.Add(rdsprod);

            ReportParameter p1 = new ReportParameter("CompanyName", _currentCompany.CompanyName.Split('-')[0]);
            ReportParameter p2 = new ReportParameter("CompanyAddressOneLine", _currentCompany.Address);
            ReportParameter p3 = new ReportParameter("PhoneNumbers", _currentCompany.PhoneNumbers);
            ReportParameter p4 = new ReportParameter("CompanyGSTNo", _currentCompany.GSTIN);
            ReportParameter p5 = new ReportParameter("BillType", _currentInvoice.IsPaid ? "CASH" : "CREDIT");

            ReportParameter p6 = new ReportParameter("BillNo", _currentInvoice.InvoiceId);
            ReportParameter p7 = new ReportParameter("BillDate", _currentInvoice.InvoiceDate.ToShortDateString());
            ReportParameter p8 = new ReportParameter("TransportName", string.IsNullOrEmpty(_currentInvoice.StrTransport) ? "__________________" : _currentInvoice.StrTransport);
            ReportParameter p9 = new ReportParameter("LRNo", string.IsNullOrEmpty(_currentInvoice.StrLRNo) ? "__________________" : _currentInvoice.StrLRNo);

            ReportParameter p10 = new ReportParameter("CustomerName", string.IsNullOrEmpty(_currentInvoice.CustomerName) ? "-" : _currentInvoice.CustomerName);
            ReportParameter p11 = new ReportParameter("CustomerAddress", string.IsNullOrEmpty(_currentInvoice.CustomerAddress) ? "-" : _currentInvoice.CustomerAddress);
            ReportParameter p12 = new ReportParameter("CustomerGST", string.IsNullOrEmpty(_currentInvoice.CustomerGST) ? "-" : _currentInvoice.CustomerGST);
            ReportParameter p13 = new ReportParameter("AadhaarPAN", string.IsNullOrEmpty(_currentInvoice.CustomerPanAadhaar) ? "-" : _currentInvoice.CustomerPanAadhaar);

            ReportParameter p14 = new ReportParameter("BankName", _currentCompany.BankName + " - " + _currentCompany.BankBranchAddress);
            ReportParameter p15 = new ReportParameter("AccNo", _currentCompany.BankAccNo);
            ReportParameter p16 = new ReportParameter("IFSCCode", _currentCompany.IFSCCode);

            ReportParameter p31 = new ReportParameter("StrBillTotal", _currentInvoice.StrBillTotal);
            ReportParameter p17 = new ReportParameter("Discount", _currentInvoice.StrDiscount);
            ReportParameter p18 = new ReportParameter("AmountBeforeTax", _currentInvoice.StrTotalAfterDiscountOrBeforeTax);
            ReportParameter p19 = new ReportParameter("SGST", string.IsNullOrEmpty(_currentInvoice.StrSGST) ? "0" : _currentInvoice.StrSGST);
            ReportParameter p20 = new ReportParameter("CGST", string.IsNullOrEmpty(_currentInvoice.StrCGST) ? "0" : _currentInvoice.StrCGST);
            ReportParameter p21 = new ReportParameter("IGST", string.IsNullOrEmpty(_currentInvoice.StrIGST) ? "0" : _currentInvoice.StrIGST);
            ReportParameter p22 = new ReportParameter("AmountAfterTax", _currentInvoice.StrTotalAfterTax);
            ReportParameter p23 = new ReportParameter("RoundValue", Convert.ToDouble(_currentInvoice.StrRounded) > 0 ? "+ " + _currentInvoice.StrRounded : _currentInvoice.StrRounded);
            ReportParameter p24 = new ReportParameter("TotalPayable", _currentInvoice.StrTotalPayable);

            ReportParameter p25 = new ReportParameter("CustomerPhone", string.IsNullOrEmpty(_currentInvoice.CustomerMobile) ? "-" : _currentInvoice.CustomerMobile);

            ReportParameter p26 = new ReportParameter("CGSTPer", string.IsNullOrEmpty(_currentInvoice.CGSTValue) ? "-" : "( " + _currentInvoice.CGSTValue + " % )");
            ReportParameter p27 = new ReportParameter("SGSTPer", string.IsNullOrEmpty(_currentInvoice.SGSTValue) ? "-" : "( " + _currentInvoice.SGSTValue + " % )");
            ReportParameter p28 = new ReportParameter("IGSTPer", string.IsNullOrEmpty(_currentInvoice.IGSTValue) ? "-" : "( " + _currentInvoice.IGSTValue + " % )");

            ReportParameter p29 = new ReportParameter("BOAddress", string.IsNullOrEmpty(_currentCompany.BOAddress) ? "." : _currentCompany.BOAddress);
            ReportParameter p30 = new ReportParameter("Totalinwords", "-");
            ReportParameter p32 = new ReportParameter("Notes", string.IsNullOrEmpty(_currentInvoice.Notes) ? "-" : _currentInvoice.Notes);

            report.SetParameters(new ReportParameter[] { 
                p1, p2, p3, p4, p5,
                p6, p7, p8, p9, p10,
                p11, p12, p13, p14, p15,
                p16, p17, p18, p19, p20,
                p21, p22, p23, p24, p25,
                p26,p27,p28, p29, p30,p31,p32
            });

            PrintToPrinter(report);
        
        }

        public void PrintToPrinter(LocalReport report)
        {
            Export(report);

        }
        public void Export(LocalReport report, bool print = true)
        {
            string deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>8.27in</PageWidth>
                <PageHeight>11.69in</PageHeight>
                <MarginTop>0.2in</MarginTop>
                <MarginLeft>0.4in</MarginLeft>
                <MarginRight>0.4in</MarginRight>
                <MarginBottom>0.2in</MarginBottom>
            </DeviceInfo>";

            if (!_currentCompany.IsGSTApplicable)
            {
                deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>14in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.2in</MarginTop>
                <MarginLeft>0.4in</MarginLeft>
                <MarginRight>0.4in</MarginRight>
                <MarginBottom>0.2in</MarginBottom>
            </DeviceInfo>";

            }

            if (_currentCompany.ThermalPrinter)
            {
                double height = 3.6 + (productCount * 0.2);

                deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>2.9in</PageWidth>
                <PageHeight>"
                +height.ToString()+
                "in</PageHeight>" +
                "<MarginTop>0.1in</MarginTop>" +
                "<MarginLeft>0.01in</MarginLeft>" +
                "<MarginRight>0.01in</MarginRight>" +
                "<MarginBottom>0.1in</MarginBottom>" +
                "</DeviceInfo>";

            }


            Warning[] warnings;
            m_streams = new List<Stream>();
            //Code for roller printer
            //report.Render("Image", deviceInfo, CreateStream, out warnings);

            report.Render("Image", deviceInfo, CreateStream, out warnings);

            

            foreach (Stream stream in m_streams)
                stream.Position = 0;

            if (print)
            {
                Print();
            }
        }


        public  void Print()
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                if(_currentCompany.ThermalPrinter)
                printDoc.PrinterSettings.PrinterName = _currentCompany.DefaultPrinter;
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                if (!_currentCompany.IsGSTApplicable)
                {
                    PrinterSettings ps = new PrinterSettings();
                    printDoc.DefaultPageSettings.Landscape = true;
                    IEnumerable<PaperSize> paperSizes = ps.PaperSizes.Cast<PaperSize>();
                    PaperSize sizeLegal = paperSizes.First<PaperSize>(size => size.Kind == PaperKind.Legal); // setting paper size to A4 size
                    printDoc.DefaultPageSettings.PaperSize = sizeLegal;
                }
                m_currentPageIndex = 0;
                printDoc.Print();


                //string GS = Convert.ToString((char)29);
                //string ESC = Convert.ToString((char)27);

                //string COMMAND = "";
                //COMMAND = ESC + "@";
                //COMMAND += GS + "V" + (char)1;

                //PrintDialog pd = new PrintDialog();
                //pd.PrinterSettings = new PrinterSettings();
                //if (DialogResult.OK == pd.ShowDialog(this))
                //{
                //    RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, COMMAND);
                //}

            }
        }

        public static Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            //Rectangle adjustedRect = new Rectangle(
            //    ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            //    ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            //    ev.PageBounds.Width,
            //    320 + productCount * 17);

            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                 ev.PageBounds.Height);

            if(_currentCompany.ThermalPrinter)
            {
                adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                360 + productCount * 20);
            }

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);



            //float x = 10;
            //float y = 5;
            //float width = 270.0F; // max width I found through trial and error
            //float height = 0F;

            //Font drawFontArial12Bold = new Font("Arial", 12, FontStyle.Bold);
            //Font drawFontArial10Regular = new Font("Arial", 10, FontStyle.Regular);
            //SolidBrush drawBrush = new SolidBrush(Color.Black);

            //// Set format of string.
            //StringFormat drawFormatCenter = new StringFormat();
            //drawFormatCenter.Alignment = StringAlignment.Center;
            //StringFormat drawFormatLeft = new StringFormat();
            //drawFormatLeft.Alignment = StringAlignment.Near;
            //StringFormat drawFormatRight = new StringFormat();
            //drawFormatRight.Alignment = StringAlignment.Far;

            // Draw string to screen.
            //string text = "Company Name";
            //e.Graphics.DrawString(text, drawFontArial12Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            //y += e.Graphics.MeasureString(text, drawFontArial12Bold).Height;

            //text = "Address";
            //e.Graphics.DrawString(pageImage, drawFontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            //y += e.Graphics.MeasureString(pageImage, drawFontArial10Regular).Height;
        }

        public static void DisposePrint()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }



        private void InvoicePrint_Load(object sender, EventArgs e)
        {
            //TestPrint();
            //PrintWithoutLoading();
            //return;
            this.reportViewer1.RefreshReport();
            reportViewer1.Visible = true;
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            var asem = Assembly.GetExecutingAssembly().Location;
            var path = System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString();
            //System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString();
            //@"C:\Users\SAIRAM\Documents\Visual Studio 2013\Projects\GST_InvoiceApplication\GST_InvoiceApplication";//Path.GetDirectoryName(asem);

            string originalInvoicePath = "InvoiceOriginal";
            if (_currentInvoice.InvoiceDate < new DateTime(2021, 08, 01))
            {
                originalInvoicePath = "\\InvoiceOriginal.rdlc";
            }
            else if (!string.IsNullOrEmpty(_currentInvoice.Notes) && _currentInvoice.Notes.Trim().Length > 0)
                originalInvoicePath = "\\InvoiceOriginal_WithNotesV2.rdlc";
            else
                originalInvoicePath = "\\InvoiceOriginalV2.rdlc";


            if (!File.Exists(path + originalInvoicePath))
                MessageBox.Show("File Not present at " + path + originalInvoicePath);



            if(_currentCompany.IsGSTApplicable)
            this.reportViewer1.LocalReport.ReportPath = path + originalInvoicePath;
            else
                this.reportViewer1.LocalReport.ReportPath = path + "\\InvoiceOriginalNoGST.rdlc";

            //this.reportViewer1.LocalReport.ReportPath = path + "\\InvoiceRollerOriginal.rdlc";
            this.reportViewer1.LocalReport.EnableExternalImages = true;
            DataTable table = new DataTable();
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("HSNCode", typeof(string));
            table.Columns.Add("Rate", typeof(string));
            table.Columns.Add("Quantity", typeof(string));
            table.Columns.Add("MtsDescription", typeof(string));
            table.Columns.Add("Mts", typeof(string));
            table.Columns.Add("Total", typeof(string));

            foreach (InvoiceProducts p in _currentInvoice.Products)
            {
                table.Rows.Add(
                    p.ProductName + (string.IsNullOrEmpty(p.MtsDescription)?"": "   ("+p.MtsDescription+")")
                    ,p.HSNCode+ "---"+ isVarientHigh(p.PreviousRate,p.Rate)
                    , string.Format("{0:n}", Convert.ToDouble(p.Rate)) + "---" + string.Format("{0:n}", Convert.ToDouble(p.RateWithoutTax))
                    //Convert.ToDouble(p.Rate).ToString("C", CultureInfo.CurrentCulture)
                    , p.Quantity
                    ,p.MtsDescription
                    ,p.Mts
                    , string.Format("{0:n}", Convert.ToDouble(p.Total)//Convert.ToDouble(p.Total).ToString("C", CultureInfo.CurrentCulture
                    ));
            }

            ReportDataSource rdsprod = new ReportDataSource("InvoiceProductSet", table);
            reportViewer1.LocalReport.DataSources.Add(rdsprod);            
            ReportParameter p1 = new ReportParameter("CompanyName", _currentCompany.CompanyName);
            ReportParameter p2 = new ReportParameter("CompanyAddressOneLine", _currentCompany.Address);
            ReportParameter p3 = new ReportParameter("PhoneNumbers", _currentCompany.PhoneNumbers);
            ReportParameter p4 = new ReportParameter("CompanyGSTNo", _currentCompany.GSTIN);
            ReportParameter p5 = new ReportParameter("BillType", _currentInvoice.IsPaid?"CASH":"CREDIT");

            ReportParameter p6 = new ReportParameter("BillNo", _currentInvoice.InvoiceId);
            ReportParameter p7 = new ReportParameter("BillDate", _currentInvoice.InvoiceDate.ToShortDateString());
            ReportParameter p8 = new ReportParameter("TransportName",string.IsNullOrEmpty(_currentInvoice.StrTransport)? "__________________":_currentInvoice.StrTransport);
            ReportParameter p9 = new ReportParameter("LRNo",string.IsNullOrEmpty(_currentInvoice.StrLRNo)? "__________________":_currentInvoice.StrLRNo);

            ReportParameter p10 = new ReportParameter("CustomerName", string.IsNullOrEmpty(_currentInvoice.CustomerName)?"-":_currentInvoice.CustomerName);
            ReportParameter p11 = new ReportParameter("CustomerAddress", string.IsNullOrEmpty(_currentInvoice.CustomerAddress)?"-":_currentInvoice.CustomerAddress);
            ReportParameter p12 = new ReportParameter("CustomerGST", string.IsNullOrEmpty(_currentInvoice.CustomerGST) ? "-" : _currentInvoice.CustomerGST);
            ReportParameter p13 = new ReportParameter("AadhaarPAN", string.IsNullOrEmpty(_currentInvoice.CustomerPanAadhaar) ? "-" : _currentInvoice.CustomerPanAadhaar);

            ReportParameter p14 = new ReportParameter("BankName", _currentCompany.BankName + " - "+_currentCompany.BankBranchAddress);
            ReportParameter p15 = new ReportParameter("AccNo",_currentCompany.BankAccNo);
            ReportParameter p16 = new ReportParameter("IFSCCode", _currentCompany.IFSCCode);

            ReportParameter p31 = new ReportParameter("StrBillTotal", _currentInvoice.StrBillTotal);
            ReportParameter p17 = new ReportParameter("Discount", _currentInvoice.StrDiscount);
            ReportParameter p18 = new ReportParameter("AmountBeforeTax", _currentInvoice.StrTotalAfterDiscountOrBeforeTax);
            ReportParameter p19 = new ReportParameter("SGST", string.IsNullOrEmpty(_currentInvoice.StrSGST)?"0":_currentInvoice.StrSGST);
            ReportParameter p20 = new ReportParameter("CGST", string.IsNullOrEmpty(_currentInvoice.StrCGST) ? "0" : _currentInvoice.StrCGST);
            ReportParameter p21 = new ReportParameter("IGST", string.IsNullOrEmpty(_currentInvoice.StrIGST) ? "0" : _currentInvoice.StrIGST);
            ReportParameter p22 = new ReportParameter("AmountAfterTax", _currentInvoice.StrTotalAfterTax);
            ReportParameter p23 = new ReportParameter("RoundValue", Convert.ToDouble(_currentInvoice.StrRounded) > 0 ? "+ " + _currentInvoice.StrRounded : _currentInvoice.StrRounded);
            ReportParameter p24 = new ReportParameter("TotalPayable", _currentInvoice.StrTotalPayable);

            ReportParameter p25 = new ReportParameter("CustomerPhone", string.IsNullOrEmpty(_currentInvoice.CustomerMobile) ? "-" : _currentInvoice.CustomerMobile);

            ReportParameter p26 = new ReportParameter("CGSTPer", string.IsNullOrEmpty(_currentInvoice.CGSTValue) ? "-" : "( "+_currentInvoice.CGSTValue+" % )");
            ReportParameter p27 = new ReportParameter("SGSTPer", string.IsNullOrEmpty(_currentInvoice.SGSTValue) ? "-" : "( " + _currentInvoice.SGSTValue + " % )");
            ReportParameter p28 = new ReportParameter("IGSTPer", string.IsNullOrEmpty(_currentInvoice.IGSTValue) ? "-" : "( " + _currentInvoice.IGSTValue + " % )");

            ReportParameter p29 = new ReportParameter("BOAddress", string.IsNullOrEmpty(_currentCompany.BOAddress) ? "." : _currentCompany.BOAddress);
            ReportParameter p30 = new ReportParameter("Totalinwords", "-");
            ReportParameter p32 = new ReportParameter("Notes", string.IsNullOrEmpty(_currentInvoice.Notes)?"-": _currentInvoice.Notes);

            this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { 
                p1, p2, p3, p4, p5,
                p6, p7, p8, p9, p10,
                p11, p12, p13, p14, p15,
                p16, p17, p18, p19, p20,
                p21, p22, p23, p24, p25,
                p26,p27,p28, p29, p30,p31,p32
            }); 

            this.reportViewer1.RefreshReport();

            

        }
        private string isVarientHigh(string previousPrice, string currentPrice)
        {
            return false.ToString();
            bool value = false;
            value = string.IsNullOrEmpty(previousPrice) || string.IsNullOrEmpty(currentPrice) ? false : Math.Abs((((Convert.ToDouble(previousPrice) - Convert.ToDouble(currentPrice)) / Math.Abs(Convert.ToDouble(previousPrice))) * 100)) > 10 ? true : false;

            return value.ToString();
        }
        private void TestPrint()
        {
            //generateReceipt();
            //printReceipt();
        }


        private void reportViewer1_Print(object sender, ReportPrintEventArgs e)
        {
            this.reportViewer1.PrintDialog();
            PageSetupDialog setupDlg = new PageSetupDialog();
            PrintDocument printDoc = new PrintDocument();
            setupDlg.Document = printDoc;
            setupDlg.AllowMargins = true;
            setupDlg.AllowOrientation = false;
            setupDlg.AllowPaper = false;
            setupDlg.AllowPrinter = false;
            setupDlg.Reset();
        }
    }
}
