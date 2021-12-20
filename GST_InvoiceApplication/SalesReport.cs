using Invoice.DataAccess;
using Invoice.Models;
using Microsoft.Office.Interop.Excel;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GST_InvoiceApplication
{
    public partial class SalesReport : Form
    {

        private static List<Stream> m_streams;
        private static int m_currentPageIndex = 0;
        private static int productCount = 0;
        public SalesReport()
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();

            LoadCompanyData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new MasterPage();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private List<CompanyDetails> GetCompanyData()
        {

            string sql = "select * from CompanyData";
            if (!checkBox2.Checked)
                sql = sql + " where IsGSTApplicable = 'True'";
            var ds = Functions.RunSelectSql(sql);
            List<CompanyDetails> companyList = new List<CompanyDetails>();


            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CompanyDetails currentCompany = new CompanyDetails();
                currentCompany.CompanyID = Convert.ToInt32(dr["ID"]);
                currentCompany.CompanyName = dr["CompanyName"].ToString();
                currentCompany.Address = dr["Address"].ToString();
                currentCompany.PhoneNumbers = dr["PhoneNo"].ToString();
                currentCompany.GSTIN = dr["GSTIN"].ToString();
                currentCompany.IsGSTApplicable = Convert.ToBoolean(dr["IsGSTApplicable"]);
                currentCompany.GSTIN = dr["GSTIN"].ToString();
                currentCompany.PANCard = dr["PANCard"].ToString();
                currentCompany.Aadhaar = dr["Aadhaar"].ToString();
                currentCompany.PropriterName = dr["PropriterName"].ToString();
                currentCompany.BankName = dr["BankName"].ToString();
                currentCompany.BankAccNo = dr["BankAccNo"].ToString();
                currentCompany.BankBranchAddress = dr["BankBranchAddress"].ToString();
                currentCompany.IFSCCode = dr["IFSCCode"].ToString();
                currentCompany.BOAddress = dr["BOAddress"].ToString();
                currentCompany.BillNo = Convert.ToInt32(dr["BillNo"]);
                currentCompany.BillDate = Convert.ToDateTime(dr["BillDate"]);
                currentCompany.BillPrefix = dr["BillPrefix"].ToString();
                companyList.Add(currentCompany);

            }
            return companyList;
        }


        public void LoadCompanyData()
        {
            List<CompanyDetails> companies = GetCompanyData();

            SortedDictionary<int, string> userCache = new SortedDictionary<int, string>();
            userCache.Add(0, "--Select--");
            foreach (CompanyDetails curr in companies)
            {
                userCache.Add(curr.CompanyID, curr.CompanyName);

            }
            
            comboBox3.DataSource = new BindingSource(userCache, null);
            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime FromDate = dateTimePicker1.Value;
            DateTime ToDate = dateTimePicker2.Value;
            ToDate = ToDate.AddDays(1);

            //string sql = "select "+
            //    "C.CompanyName,C.GSTIN as CompanyGST,S.InvoiceId,S.InvoiceDate,S.CustomerName,S.CustomerGST,S.CustomerPanAadhaar,S.StrBillTotal,S.StrSGST,S.StrCGST,S.StrIGST,S.StrTotalAfterTax,S.StrRounded,S.StrTotalPayable,S.SGSTValue,S.CGSTValue,S.IGSTValue" +
            //    //"s.*,c.* "+
            //" from SalesInvoiceDetail s inner join CompanyData c on c.ID = s.CompanyId where s.InvoiceDate >#" + Convert.ToDateTime(FromDate.ToShortDateString()).ToString("yyyy-MM-dd") +
            //    "# and s.InvoiceDate <#" + Convert.ToDateTime(ToDate.ToShortDateString()).ToString("yyyy-MM-dd") + "# Order by CompanyId,InvoiceDate";
            string sql = "";

            string whereCondition = "";
            if(!checkBox2.Checked)
            {
                whereCondition = " C.IsGSTApplicable = 'True' and";
            }

            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                whereCondition = whereCondition+ " S.InvoiceId like '%" + textBox2.Text + "%' and";
 
            }

            if (comboBox3.SelectedItem != null && comboBox3.SelectedValue.ToString() != "0")       
            {
                whereCondition = whereCondition+ " s.CompanyId =  " + Convert.ToInt32(comboBox3.SelectedValue) + " and";
            }



            if(!checkBox1.Checked)
             sql = "select " +
                "C.CompanyName,C.GSTIN as CompanyGST,S.InvoiceId,S.InvoiceDate,S.CustomerName,S.CustomerGST,S.CustomerPanAadhaar as PANAadhar,S.StrTotalAfterDiscountOrBeforeTax as TotalAmount,S.StrSGST as SGST,S.StrCGST as CGST,S.StrIGST as IGST,S.StrTotalAfterTax as TotalAfterTax,S.StrRounded as RoundOff,S.StrTotalPayable as GrandTotal,S.IsPaid as IsPaid,C.ID as CompanyId " +
                //"s.*,c.* "+
            " from SalesInvoiceDetail s inner join CompanyData c on c.ID = s.CompanyId where s.InvoiceId not like '%Backup%' and s.InvoiceDate >#" + Convert.ToDateTime(FromDate.ToShortDateString()).ToString("yyyy-MM-dd") +
                "# and " + whereCondition + " s.InvoiceDate <#" + Convert.ToDateTime(ToDate.ToShortDateString()).ToString("yyyy-MM-dd") + "# Order by CompanyId,InvoiceId,CreatedOn";
            else
                 sql = "select " +
                "C.CompanyName,C.GSTIN as CompanyGST,S.InvoiceId,S.InvoiceDate,S.CustomerName,S.CustomerGST,S.CustomerPanAadhaar as PANAadhar,S.StrTotalAfterDiscountOrBeforeTax as TotalAmount,S.StrSGST as SGST,S.StrCGST as CGST,S.StrIGST as IGST,S.StrTotalAfterTax as TotalAfterTax,S.StrRounded as RoundOff,S.StrTotalPayable as GrandTotal,S.IsPaid as IsPaid,C.ID as CompanyId " +
                //"s.*,c.* "+
            " from SalesInvoiceDetail s inner join CompanyData c on c.ID = s.CompanyId where s.InvoiceDate >#" + Convert.ToDateTime(FromDate.ToShortDateString()).ToString("yyyy-MM-dd") +
                "# and " + whereCondition + " s.InvoiceDate <#" + Convert.ToDateTime(ToDate.ToShortDateString()).ToString("yyyy-MM-dd") + "# Order by CompanyId,InvoiceId,CreatedOn";


            DataSet ds = Functions.RunSelectSql(sql);

            DataSet ds1 = Functions.RunSelectSql(sql, Functions.GetYearConnectionString(Functions.getPreviousYear()));


            System.Data.DataTable dt = ds.Tables[0];

            dt.Merge(ds1.Tables[0]);

            if (!checkBox1.Checked)
            {
                System.Data.DataTable dtCloned = dt.Clone();
                dtCloned.Columns[2].DataType = typeof(Int32);
                foreach (DataRow row in dt.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                dataGridView1.DataSource = dtCloned;


                //dataGridView1.DataSource = dt;
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            }
            else
                dataGridView1.DataSource = dt;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //progressBar1 = new ProgressBar();
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = dataGridView1.Rows.Count+20;

           

            if (string.IsNullOrEmpty(textBox1.Text)) {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {

                        textBox1.Text = fbd.SelectedPath;
                    }
                }
                //return;
            }

            this.progressBar1.Increment(20);
            string filename = textBox1.Text + @"\output" + DateTime.Now.ToString("MMddyyyyhhmmtt") + ".xlsx";
            textBox1.Text = filename;

            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program  
            app.Visible = false;
            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            // changing the name of active sheet  
            worksheet.Name = "Exported from gridview";
            // storing header part in Excel  
            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
                worksheet.Cells[1, i].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[1, i].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);
            }

            int counter = 1;
            // storing Each row and column value to excel sheet  
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if (j == 3)
                    {
                        worksheet.Cells[i + 2, j + 1] = Convert.ToDateTime(dataGridView1.Rows[i].Cells[j].Value.ToString()).ToString("dd-MMM");
                    }
                    else
                        worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    worksheet.Cells[i + 2, j + 1].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                }
                this.progressBar1.Increment(1);
                counter++;
            }

            worksheet.Columns[1].ColumnWidth = 4.43;
            worksheet.Columns[2].ColumnWidth = 0.1;
            worksheet.Columns[3].ColumnWidth = 5.00;

            worksheet.Columns[4].ColumnWidth = 6.86;
            worksheet.Columns[4].NumberFormat = "dd-MMM";

            worksheet.Columns[5].ColumnWidth = 16.51;
            worksheet.Columns[6].ColumnWidth = 18;

            worksheet.Columns[7].ColumnWidth = 12.86;
            worksheet.Columns[7].NumberFormat = "##################";

            worksheet.Columns[8].ColumnWidth = 10.89;
            
            worksheet.Columns[9].ColumnWidth = 7.43;
            worksheet.Columns[10].ColumnWidth = 7.43;
            worksheet.Columns[11].ColumnWidth = 7.43;
           
            
            worksheet.Columns[12].ColumnWidth = 10.29;
            worksheet.Columns[13].ColumnWidth = 7.43;
            worksheet.Columns[14].ColumnWidth = 10.29;


            worksheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
            worksheet.PageSetup.LeftMargin = 9.7;
            worksheet.PageSetup.RightMargin = 9.7;
            worksheet.PageSetup.TopMargin = 15.0;
            worksheet.PageSetup.BottomMargin = 15.0;







            // save the application  
            workbook.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            // Exit from the application  
            app.Quit();

            

            MessageBox.Show("Created Successfully");
            this.progressBar1.Value = 0;

        }

        static void ShowMessageBox()
        {
            MessageBox.Show("Creating ExcelSheet, Please Wait....", "Information",MessageBoxButtons.OK);
        }

        

        private void textBox1_Enter(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    
                    textBox1.Text = fbd.SelectedPath;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("CompanyName", typeof(string));
            table.Columns.Add("InvoiceId", typeof(Int32));
            table.Columns.Add("InvoiceDate", typeof(string));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("CustomerGST", typeof(string));
            table.Columns.Add("PAN", typeof(string));
            table.Columns.Add("SaleAmount", typeof(string));
            table.Columns.Add("SGST", typeof(string));
            table.Columns.Add("CGST", typeof(string));
            table.Columns.Add("IGST", typeof(string));
            table.Columns.Add("RoundOff", typeof(string));
            table.Columns.Add("Total", typeof(string));
            table.Columns.Add("SaleType", typeof(string));
            //table.Columns.Add("CompanyId", typeof(string));

            double saleAmount = 0;
            double SGST = 0;
            double CGST = 0;
            double Total = 0;

            table.DefaultView.Sort = "CompanyName ASC, InvoiceId ASC";


            for (int i = 0; i < dataGridView1.Rows.Count - 1; ++i)
            {
                string companyName = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);

                table.Rows.Add(companyName.Length>4? companyName.Substring(0, 4):companyName
                    ,dataGridView1.Rows[i].Cells[2].Value
                    , Convert.ToDateTime(dataGridView1.Rows[i].Cells[3].Value).ToString("dd/MMM")
                    , dataGridView1.Rows[i].Cells[4].Value
                    , dataGridView1.Rows[i].Cells[5].Value
                    , dataGridView1.Rows[i].Cells[6].Value
                    , dataGridView1.Rows[i].Cells[7].Value
                    , dataGridView1.Rows[i].Cells[8].Value
                    , dataGridView1.Rows[i].Cells[9].Value
                    , dataGridView1.Rows[i].Cells[10].Value
                    , dataGridView1.Rows[i].Cells[12].Value//round
                    , dataGridView1.Rows[i].Cells[13].Value
                    , Convert.ToBoolean(dataGridView1.Rows[i].Cells[14].Value)?"CASH":"CREDIT"
                    );
            }

            table = table.DefaultView.ToTable();
  
            productCount = dataGridView1.Rows.Count;
            ReportDataSource rdsprod = new ReportDataSource("DaySales", table);


            LocalReport report = new LocalReport();
            report.ReportPath = System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString() + "\\InvoiceDaySales.rdlc";

            report.DataSources.Add(rdsprod);
            
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
                <PageWidth>8.5in</PageWidth>
                <PageHeight>11in</PageHeight>
                <MarginTop>0.23in</MarginTop>
                <MarginLeft>0.13in</MarginLeft>
                <MarginRight>0.03in</MarginRight>
                <MarginBottom>0.23in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;

            if (print)
            {
                Print();
            }
        }


        public static void Print()
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            //printDoc.PrinterSettings.PrinterName = System.Configuration.ConfigurationManager.AppSettings["RetailPrinterRoller"].ToString();
            if (!printDoc.PrinterSettings.IsValid)
            {
                MessageBox.Show("Printer Not Found - " + printDoc.PrinterSettings.PrinterName);
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
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

        public static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);


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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int InvoiceId = 0;
            int CompanyId = 0;

            bool converted = int.TryParse(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString(), out InvoiceId);
            if (converted)
                converted = int.TryParse(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[15].Value.ToString(), out CompanyId);

            DateTime inv = Convert.ToDateTime(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value.ToString());

            bool isCurrentYearBill = Functions.isCurrentFinancialYear(inv);

            if (inv < new DateTime(2021, 08, 01))
            {
                if (InvoiceId > 0 && CompanyId > 0 && isCurrentYearBill)
                {
                    new AddInvoiceOld(InvoiceId, CompanyId).Show();
                }
                else if (InvoiceId > 0 && CompanyId > 0 && !isCurrentYearBill)
                {
                    new AddInvoiceOld(InvoiceId, CompanyId, "19").Show();
                }

            }

            else
            {
                if (InvoiceId > 0 && CompanyId > 0 && isCurrentYearBill)
                {
                    new AddInvoice(InvoiceId, CompanyId).Show();
                }
                else if (InvoiceId > 0 && CompanyId > 0 && !isCurrentYearBill)
                {
                    new AddInvoice(InvoiceId, CompanyId, "19").Show();
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            LoadCompanyData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime FromDate = dateTimePicker1.Value;
            DateTime ToDate = dateTimePicker2.Value;
            ToDate = ToDate.AddDays(1);

            //string sql = "select "+
            //    "C.CompanyName,C.GSTIN as CompanyGST,S.InvoiceId,S.InvoiceDate,S.CustomerName,S.CustomerGST,S.CustomerPanAadhaar,S.StrBillTotal,S.StrSGST,S.StrCGST,S.StrIGST,S.StrTotalAfterTax,S.StrRounded,S.StrTotalPayable,S.SGSTValue,S.CGSTValue,S.IGSTValue" +
            //    //"s.*,c.* "+
            //" from SalesInvoiceDetail s inner join CompanyData c on c.ID = s.CompanyId where s.InvoiceDate >#" + Convert.ToDateTime(FromDate.ToShortDateString()).ToString("yyyy-MM-dd") +
            //    "# and s.InvoiceDate <#" + Convert.ToDateTime(ToDate.ToShortDateString()).ToString("yyyy-MM-dd") + "# Order by CompanyId,InvoiceDate";
            string sql = "";

            string whereCondition = "";
            if (!checkBox2.Checked)
            {
                whereCondition = " C.IsGSTApplicable = 'True' and";
            }

            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                whereCondition = whereCondition + " S.InvoiceId like '%" + textBox2.Text + "%' and";

            }

            if (comboBox3.SelectedItem != null && comboBox3.SelectedValue.ToString() != "0")
            {
                whereCondition = whereCondition + " s.CompanyId =  " + Convert.ToInt32(comboBox3.SelectedValue) + " and";
            }



            if (!checkBox1.Checked)
                sql = "select " +
                   "C.CompanyName,C.GSTIN as CompanyGST,S.InvoiceId,S.InvoiceDate,S.CustomerName,S.CustomerGST,S.CustomerPanAadhaar as PANAadhar,S.StrTotalAfterDiscountOrBeforeTax as TotalAmount,S.StrSGST as SGST,S.StrCGST as CGST,S.StrIGST as IGST,S.StrTotalAfterTax as TotalAfterTax,S.StrRounded as RoundOff,S.StrTotalPayable as GrandTotal,S.IsPaid as IsPaid,C.ID as CompanyId " +
               //"s.*,c.* "+
               " from SalesInvoiceDetail s inner join CompanyData c on c.ID = s.CompanyId where s.saleid in (select saleid from invoiceproductdetails where hsncode ='') and s.InvoiceId not like '%Backup%' and s.InvoiceDate >#" + Convert.ToDateTime(FromDate.ToShortDateString()).ToString("yyyy-MM-dd") +
                   "# and " + whereCondition + " s.InvoiceDate <#" + Convert.ToDateTime(ToDate.ToShortDateString()).ToString("yyyy-MM-dd") + "# Order by CompanyId,InvoiceId,CreatedOn";
            else
                sql = "select " +
               "C.CompanyName,C.GSTIN as CompanyGST,S.InvoiceId,S.InvoiceDate,S.CustomerName,S.CustomerGST,S.CustomerPanAadhaar as PANAadhar,S.StrTotalAfterDiscountOrBeforeTax as TotalAmount,S.StrSGST as SGST,S.StrCGST as CGST,S.StrIGST as IGST,S.StrTotalAfterTax as TotalAfterTax,S.StrRounded as RoundOff,S.StrTotalPayable as GrandTotal,S.IsPaid as IsPaid,C.ID as CompanyId " +
           //"s.*,c.* "+
           " from SalesInvoiceDetail s inner join CompanyData c on c.ID = s.CompanyId where s.saleid in (select saleid from invoiceproductdetails where hsncode ='') and s.InvoiceDate >#" + Convert.ToDateTime(FromDate.ToShortDateString()).ToString("yyyy-MM-dd") +
               "# and " + whereCondition + " s.InvoiceDate <#" + Convert.ToDateTime(ToDate.ToShortDateString()).ToString("yyyy-MM-dd") + "# Order by CompanyId,InvoiceId,CreatedOn";


            DataSet ds = Functions.RunSelectSql(sql);

            


            System.Data.DataTable dt = ds.Tables[0];

            

            if (!checkBox1.Checked)
            {
                System.Data.DataTable dtCloned = dt.Clone();
                dtCloned.Columns[2].DataType = typeof(Int32);
                foreach (DataRow row in dt.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                dataGridView1.DataSource = dtCloned;


                //dataGridView1.DataSource = dt;
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            }
            else
                dataGridView1.DataSource = dt;
        }
    }
}
