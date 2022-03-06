using Invoice.DataAccess;
using Invoice.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Drive.v3.Data;
using System.Net;


namespace GST_InvoiceApplication
{
    public partial class SetupCompany : Form
    {
        long fileSize = 0;
        public SetupCompany()
        {
            progressBar1 = new ProgressBar();

            progressBar1.Minimum = 0;
            progressBar1.Minimum = 100;


            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            getCompanyData();

            String pkInstalledPrinters;
            for (int i = 0; i < System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count; i++)
            {
                pkInstalledPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
                comboBox1.Items.Add(pkInstalledPrinters);
            }
        }

        public void getCompanyData()
        {
            string sql = "select * from CompanyData";
            var ds = Functions.RunSelectSql(sql);

            dataGridView1.DataSource = ds.Tables[0];

            //List<CompanyDetails> companyList = new List<CompanyDetails>();
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    CompanyDetails currentCompany = new CompanyDetails();
            //    currentCompany.CompanyID = Convert.ToInt32(dr["ID"]);
            //    currentCompany.CompanyName = dr["CompanyName"].ToString();
            //    currentCompany.Address = dr["Address"].ToString();
            //    currentCompany.PhoneNumbers = dr["PhoneNo"].ToString();
            //    currentCompany.GSTIN = dr["GSTIN"].ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new MasterPage();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox11.Text))
            {
                MessageBox.Show("Please select company from List and click Save.");
                return;
            }
            string query = "update CompanyData set " +
                "CompanyName = '" + textBox2.Text + "'" +
                ", PropriterName = '" + textBox1.Text + "'" +
                ", Address = '" + richTextBox1.Text + "'" +
                ", IsGSTApplicable = '" + checkBox1.Checked.ToString() + "'" +
                ", DefaultCashBill = '" + checkBox2.Checked.ToString() + "'" +
                ", ThermalPrinter = '" + checkBox3.Checked.ToString() + "'" +
                ", GSTIN = '" + textBox4.Text + "'" +
                ", PANCard = '" + textBox5.Text + "'" +
                ", Aadhaar = '" + textBox6.Text + "'" +
                ", PhoneNo = '" + textBox3.Text + "'" +
                ", DefaultPrinter = '" + textBox14.Text + "'" +
                ", BOAddress = '" + textBox12.Text + "'" +
                ", BillNo = " + textBox13.Text + "" +
                ", BankName = '" + textBox7.Text + "'" +
                ", BankAccNo = '" + textBox8.Text + "'" +
                ", BankBranchAddress = '" + textBox9.Text + "'" +
                ", IFSCCode = '" + textBox10.Text + "'" +
                " where Id = " + textBox11.Text;

            try { Functions.RunExecuteNonQuery(query); }
            catch
            {

            }
            ;
            MessageBox.Show("Updated");
            getCompanyData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 19)
            {
                string sql = "Select * from CompanyData where Id = " + dataGridView1.CurrentRow.Cells[0].Value;
                DataSet ds = Functions.RunSelectSql(sql);
                DataRow dr = ds.Tables[0].Rows[0];
                textBox11.Text = dr["Id"].ToString();
                textBox2.Text = dr["CompanyName"].ToString();
                textBox1.Text = dr["PropriterName"].ToString();
                richTextBox1.Text = dr["Address"].ToString();
                checkBox1.Checked = Convert.ToBoolean(dr["IsGSTApplicable"]);
                checkBox2.Checked = Convert.ToBoolean(dr["DefaultCashBill"]);
                checkBox3.Checked = Convert.ToBoolean(dr["ThermalPrinter"]);
                textBox4.Text = dr["GSTIN"].ToString();
                textBox5.Text = dr["PANCard"].ToString();
                textBox6.Text = dr["Aadhaar"].ToString();
                textBox3.Text = dr["PhoneNo"].ToString();
                textBox12.Text = dr["BOAddress"].ToString();
                textBox13.Text = dr["BillNo"].ToString();

                textBox7.Text = dr["BankName"].ToString();
                textBox8.Text = dr["BankAccNo"].ToString();
                textBox9.Text = dr["BankBranchAddress"].ToString();
                textBox10.Text = dr["IFSCCode"].ToString();
                textBox14.Text = dr["DefaultPrinter"].ToString();

                button2.Enabled = true;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            if (textBox11.Text.Length < 1)
            {
                MessageBox.Show("Please select a company");
                return;
            }

            if (!doDataBackup())
            {
                MessageBox.Show("Databackup failed, Can't proceed to delete Bills");
                return;
            }

            string sql = "Delete * from InvoiceProductDetails where SaleId in (select Saleid from SalesInvoiceDetail where CompanyId = " + textBox11.Text + ")";
            Functions.RunExecuteNonQuery(sql);
            sql = "Delete * from SalesInvoiceDetail where CompanyId = " + textBox11.Text;
            Functions.RunExecuteNonQuery(sql);
            sql = "update CompanyData set BillNo = 0 where Id = " + textBox11.Text;
            Functions.RunExecuteNonQuery(sql);
            MessageBox.Show("Bills Deleted, Next bill start with 0 now.");

        }

        private string downloadLatestFile()
        {
            string currentFile = "";
            string fileId = "";
            try
            {

                UserCredential credential;
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          new ClientSecrets { ClientId = "49588927530-r4nbmm11rvmcftdlugihs46m3klss9p4.apps.googleusercontent.com", ClientSecret = "6CeiaTu3pgIH81DAyM_P3K9z" },
                          new[] { DriveService.Scope.Drive,
                                  DriveService.Scope.DriveFile },
                          "user",
                          CancellationToken.None,
                          new FileDataStore("Drive.Auth.Store")).Result;

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive API Sample",
                });

                List<File> result = new List<File>();
                FilesResource.ListRequest request = service.Files.List();

                FileList files = request.Execute();

                foreach (var file in files.Files)
                {
                    if (file.Name.Contains("Invoice"))
                    {
                        if (String.IsNullOrEmpty(currentFile))
                        {
                            currentFile = file.Name;
                            fileId = file.Id;
                        }

                        else
                        {
                            string currentcreated = file.Name.Replace("InvoiceBackup", "").Replace(".mdb", "");//"InvoiceBackup" + DateTime.Now.ToString("ddMMyyyyhhmmsstt") + ".mdb",
                            string existing = currentFile.Replace("InvoiceBackup", "").Replace(".mdb", "");//"InvoiceBackup" + DateTime.Now.ToString("ddMMyyyyhhmmsstt") + ".mdb",

                            DateTime currentCreate = DateTime.ParseExact(currentcreated, "ddMMyyyyhhmmsstt", System.Globalization.CultureInfo.InvariantCulture);
                            DateTime existingCreate = DateTime.ParseExact(existing, "ddMMyyyyhhmmsstt", System.Globalization.CultureInfo.InvariantCulture);

                            if (currentCreate > existingCreate)
                            {
                                currentFile = file.Name;
                                fileId = file.Id;
                            }

                        }

                    }
                }

                //do
                //{
                //    try
                //    {
                //        FileList files = request.Execute();

                //        result.AddRange(files.);
                //        request.PageToken = files.NextPageToken;
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine("An error occurred: " + e.Message);
                //        request.PageToken = null;
                //    }
                //} while (!String.IsNullOrEmpty(request.PageToken));

            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error Occured..!!");
            }
            return fileId;
        }

        private bool downloadFileById(string fileId)
        {
            try
            {
                UserCredential credential;
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          new ClientSecrets { ClientId = "49588927530-r4nbmm11rvmcftdlugihs46m3klss9p4.apps.googleusercontent.com", ClientSecret = "6CeiaTu3pgIH81DAyM_P3K9z" },
                          new[] { DriveService.Scope.Drive,
                                  DriveService.Scope.DriveFile },
                          "user",
                          CancellationToken.None,
                          new FileDataStore("Drive.Auth.Store")).Result;

                DriveService driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive API Sample",
                });

                var request = driveService.Files.Get(fileId);
                var stream = new System.IO.MemoryStream();
                request.Download(stream);

                //System.IO.FileStream file = new System.IO.FileStream(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                System.IO.FileStream file = new System.IO.FileStream(@"C:\Users\SAIRAM\Desktop\OLDBills\GSTBackup.mdb", System.IO.FileMode.Create, System.IO.FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();

            }
            catch { }
            return true;

        }

        private bool doDataBackup()
        {

            //downloadFileById(downloadLatestFile());

            //return true;

            string fileID = string.Empty;
            try
            {
                UserCredential credential;
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          new ClientSecrets { ClientId = System.Configuration.ConfigurationManager.AppSettings["DriveClientId"].ToString(), ClientSecret = System.Configuration.ConfigurationManager.AppSettings["DriveClientSecret"].ToString() },
                          new[] { DriveService.Scope.Drive,
                                  DriveService.Scope.DriveFile },
                          "user",
                          CancellationToken.None,
                          new FileDataStore("Drive.Auth.Store")).Result;

                DriveService driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive API Sample",
                });
                driveService.HttpClient.Timeout = TimeSpan.FromMinutes(30);

                var fileMetadata = new File()
                {
                    Name = "InvoiceBackup_" + DateTime.Now.ToString("dd_MMM_yy_HH_mm") + ".mdb",
                    MimeType = "application/mdb",
                    Parents = new List<string>
                    {
                        "19mJu5CSj-IBM6mCs5S0FRmwFiWGwYYw0"
                    }
                };
                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString(),
                                        System.IO.FileMode.Open))
                {
                    request = driveService.Files.Create(
                        fileMetadata, stream, "text/csv");

                    fileSize = stream.Length;

                    request.ProgressChanged += Upload_ProgressChanged;
                    request.Fields = "id";

                    request.Upload();
                }
                var file = request.ResponseBody;
                Console.WriteLine("File ID: " + file.Id);
                fileID = file.Id;
            }
            catch (Exception ex)
            {
                string filePath = System.Configuration.ConfigurationManager.AppSettings["PrintReportPath"].ToString()+"\\Error.txt";


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

            if (fileID.Length > 0)
            {
                MessageBox.Show("Data backup successfull");
                progressBar1.Visible = false;
                progressBar1.Value = 0;
                return true;
            }
            return false;
        }

        private void Upload_ProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            int percentage = Convert.ToInt32(Math.Round((Convert.ToDouble(obj.BytesSent) / Convert.ToDouble(fileSize)) * 100, 0));

            this.progressBar1.BeginInvoke(
    (MethodInvoker)delegate()
            {
                progressBar1.Value = percentage;
                progressBar1.Refresh();
                //txtLogDetails.Text = message; 

            });

        }

        private void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            if (!doDataBackup())
                MessageBox.Show("Backup Failed, Please check internet connection");

        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (CheckForInternetConnection())
            {
                MessageBox.Show("Internet is available, Proceed for backup");
            }
            else
                MessageBox.Show("XXXX Internet not available XXXX");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                // The combo box's Text property returns the selected item's text, which is the printer name.
                textBox14.Text = comboBox1.Text;
            }
        }
    }
}
