using Invoice.DataAccess;
using Invoice.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GST_InvoiceApplication
{
    public partial class TallyExport : Form
    {
        public TallyExport()
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
            LoadCompanyData();
        }



        private void button1_Click(object sender, EventArgs e)
        {

            string objXML = "<ENVELOPE><HEADER><TALLYREQUEST>Export Data</TALLYREQUEST><ID>All Masters</ID></HEADER><BODY>"
                + "<DESC></DESC>"
                + "</BODY></ENVELOPE>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:9000/");
            request.Method = "POST";
            request.ContentLength = objXML.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = Encoding.UTF8.GetBytes(objXML);

            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();


            string filePath = System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString() + "\\Error.txt";


            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                writer.WriteLine(responseFromServer);
            }

            //StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            //streamWriter.Write(objXML);
            //streamWriter.Close();

        }
        public void LoadCompanyData()
        {
            List<CompanyDetails> companies = GetCompanyData();

            SortedDictionary<int, string> userCache = new SortedDictionary<int, string>();
            
            foreach (CompanyDetails curr in companies)
            {
                userCache.Add(curr.CompanyID, curr.CompanyName);

            }

            comboBox3.DataSource = new BindingSource(userCache, null);
            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
        }

        private List<CompanyDetails> GetCompanyData()
        {

            string sql = "select * from CompanyData";

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

        private void button1_Click_1(object sender, EventArgs e)
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


            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                whereCondition = whereCondition + " S.InvoiceId like '%" + textBox2.Text + "%' and";

            }

            if (comboBox3.SelectedItem != null && comboBox3.SelectedValue.ToString() != "0")
            {
                whereCondition = whereCondition + " s.CompanyId =  " + Convert.ToInt32(comboBox3.SelectedValue) + " and";
            }




            sql = "select " +
               "S.InvoiceId," +
               "S.InvoiceDate," +
               "S.CustomerName," +
               "S.CustomerGST," +
               "S.StrTotalAfterDiscountOrBeforeTax as TotalAmount," +
               "S.StrSGST as SGST," +
               "S.StrCGST as CGST," +
               "S.StrIGST as IGST," +
               "S.StrRounded as RoundOff," +
               "S.StrTotalPayable as GrandTotal," +
               "S.IsPaid as IsPaid " +
               
           //"s.*,c.* "+
           " from SalesInvoiceDetail s inner join CompanyData c on c.ID = s.CompanyId where s.InvoiceId not like '%Backup%' and s.InvoiceDate >#" + Convert.ToDateTime(FromDate.ToShortDateString()).ToString("yyyy-MM-dd") +
               "# and " + whereCondition + " s.InvoiceDate <#" + Convert.ToDateTime(ToDate.ToShortDateString()).ToString("yyyy-MM-dd") + "# Order by CompanyId,InvoiceId,CreatedOn";


            DataSet ds = Functions.RunSelectSql(sql);

            

            System.Data.DataTable dt = ds.Tables[0];

            dt.Columns.Add("TallyLedger");
            


            dataGridView1.Columns[0].DataPropertyName = "InvoiceId";
            dataGridView1.Columns[1].DataPropertyName = "InvoiceDate";
            dataGridView1.Columns[2].DataPropertyName = "CustomerName";
            dataGridView1.Columns[3].DataPropertyName = "CustomerGST";
            dataGridView1.Columns[4].DataPropertyName = "TotalAmount";

            dataGridView1.Columns[5].DataPropertyName = "SGST";
            dataGridView1.Columns[6].DataPropertyName = "CGST";
            dataGridView1.Columns[7].DataPropertyName = "IGST";

            dataGridView1.Columns[8].DataPropertyName = "RoundOff";
            dataGridView1.Columns[9].DataPropertyName = "GrandTotal";
            dataGridView1.Columns[10].DataPropertyName = "IsPaid";

            dataGridView1.Columns[11].DataPropertyName = "TallyLedger";




            dataGridView1.DataSource = dt;


            

            if (!(dt.Rows.Count > 0))
                return;

            MapTallyLedgers();
            TallyXMLProcessor pro = new TallyXMLProcessor();
            string xml= pro.GetTallyXML(dt, Convert.ToInt32(comboBox3.SelectedValue));
            string filePath;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {

                    filePath = fbd.SelectedPath+@"\DayBook.xml";
                    if (File.Exists(filePath))
                    {
                        // If file found, delete it    
                        File.Delete(filePath);
                    }
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
                    {
                        writer.WriteLine(xml);
                    }
                    MessageBox.Show("File successfully created..!!" + filePath);
                }
            }

            

        }

        private void MapTallyLedgers()
        {
            DataTable dt =  ReadLedgerData();
            //if (dt != null)
            for (int i = 0; i < dataGridView1.Rows.Count; ++i)
            {
                string billGST = dataGridView1.Rows[i].Cells[3].Value == null ? null : dataGridView1.Rows[i].Cells[3].Value.ToString();
                if (!string.IsNullOrEmpty(billGST))
                {
                    DataRow dr = dt == null ? null : dt.Select("$PartyGSTIN='" + billGST + "'").FirstOrDefault();
                    if (dr != null)
                    {
                        dataGridView1.Rows[i].Cells[11].Value = dr[0].ToString();
                        //    MessageBox.Show("got a row"+dr[0].ToString()+dr[1].ToString());
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[3].Value.ToString()))
                            dataGridView1.Rows[i].Cells[11].Value = "NewAccountLedger";
                        else if (string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[3].Value.ToString()) && Convert.ToBoolean(dataGridView1.Rows[i].Cells[10].Value.ToString()))
                            dataGridView1.Rows[i].Cells[11].Value = "Cash";
                        else
                            dataGridView1.Rows[i].Cells[11].Value = "NoGSTCreditLedger";
                    }

                }
                else
                {
                    if (dataGridView1.Rows[i].Cells[10].Value!=null && Convert.ToBoolean(dataGridView1.Rows[i].Cells[10].Value.ToString()))
                        dataGridView1.Rows[i].Cells[11].Value = "Cash";
                    else
                        dataGridView1.Rows[i].Cells[11].Value = "NoGSTCreditLedger";
                }
            }
        }

        private System.Data.DataTable  ReadLedgerData()
        {
            try
            {
                string query = ConfigurationManager.AppSettings["TallyQuery"].ToString();
                OdbcConnection con = new OdbcConnection(ConfigurationManager.AppSettings["TallyConnection"].ToString());
                OdbcCommand cmd = new OdbcCommand(query, con);
                con.Open();
                OdbcDataAdapter da = new OdbcDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count>0)
                {
                    dataGridView2.DataSource = ds.Tables[0];
                    return ds.Tables[0];
                }
                else
                    MessageBox.Show("tally connection failed; table count zero");
               
                con.Close();
            }
            catch (Exception ex)
            {
                string filePath = System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString() + "\\Error.txt";


                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }

            }
            return null;
            //throw methodn
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new MasterPage();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }
    }
}
