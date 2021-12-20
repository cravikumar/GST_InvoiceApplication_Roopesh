using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Invoice.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GST_InvoiceApplication
{
    public partial class TallyUtility : Form
    {
        public string latestFileId="";
        public TallyUtility()
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();

            getLatestFileData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new MasterPage();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //SearchFiles();
            GetListofAvailableBackupFiles();

            //ProcessDownloadedFile("");
        }

        private void SearchFiles()
        {
            string path = @"C:\Users\SAIRAM\Downloads\2019 Shop Bills-20200407T073800Z-001\2019 Shop Bills";
            //string path = @"C:\Users\SAIRAM\Downloads\2019 Shop Bills-20200407T073800Z-001\";
            string[] filePaths = Directory.GetFiles(path);

            StringBuilder filesList = new StringBuilder();
            filesList.Append("SearchResult::::::");
            foreach (string con in filePaths)
            {
                string sql = "Select InvoiceId,CompanyID from SalesInvoiceDetail where StrTotalPayable like '%" + "" + "%'";
                DataSet ds = Functions.RunSelectSqlWithCon(sql, con);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        filesList.Append("\n\r Found-----" + Path.GetFileName(con) + "---" + dr["CompanyID"] + "--" + dr["InvoiceId"]);
                }

            }

            foreach (string con in filePaths)
            {
                string sql = "Select InvoiceId,CompanyID from SalesInvoiceDetail where StrTotalAfterDiscountOrBeforeTax like '%" + "" + "%'";
                DataSet ds = Functions.RunSelectSqlWithCon(sql, con);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        filesList.Append("\n\r Found-----" + Path.GetFileName(con) + "---" + dr["CompanyID"] + "--" + dr["InvoiceId"]);
                }

            }

            MessageBox.Show(filesList.ToString());
            MessageBox.Show("Completed Search");
        }

        private void ProcessDownloadedFile(string path)
        {



        }


        private void getLatestFileData()
        {

            try
            {
                //fileId = "1jiJPklSRgho4DDx4XZIn5foTidxya6FF";
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

                FilesResource.ListRequest listRequest = driveService.Files.List();
                listRequest.Q = "'1-32FlVNQHVUFz9Qr0MxCWEDuxHjfOCdl' in parents and mimeType = 'application/mdb'";
                listRequest.OrderBy = "modifiedTime desc";
                var files = listRequest.Execute();
                latestFileId = files.Files[0].Id;
                label1.Text = files.Files[0].Name;

            }
            catch (Exception ex)
            { }
        }
    

        private bool GetListofAvailableBackupFiles()
        {
            try
            {
                //fileId = "1jiJPklSRgho4DDx4XZIn5foTidxya6FF";
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

                FilesResource.ListRequest listRequest = driveService.Files.List();
                listRequest.Q = "'1-32FlVNQHVUFz9Qr0MxCWEDuxHjfOCdl' in parents and mimeType = 'application/mdb'";
                var files = listRequest.Execute();
                if (files.Files.Count > 0)
                {
                    string Output = "";
                    DataTable dt = new DataTable();

                    dt.Columns.Add("File Name");
                    dt.Columns.Add("File Type");
                    dt.Columns.Add("File ID");
                    foreach (Google.Apis.Drive.v3.Data.File localfile in files.Files)
                    {
                        DataRow dr = dt.NewRow();
                        dr["File Name"] = localfile.Name;
                        dr["File Type"] = localfile.MimeType;
                        dr["File ID"] = localfile.Id;
                        dt.Rows.Add(dr);
                    }
                    dataGridView1.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("No file found");
                }

                return true;

                //var request = driveService.Files.Get(fileId);
                //var stream = new System.IO.MemoryStream();
                //request.Download(stream);

                ////System.IO.FileStream file = new System.IO.FileStream(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                //System.IO.FileStream file = new System.IO.FileStream(localPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                //stream.WriteTo(file);
                //file.Close();
                //stream.Close();

            }
            catch(Exception ex)
            { }
            return true;

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = 50;
            progressBar1.Refresh();


            if (!doDataBackup())
            {
                MessageBox.Show("Can't Proceed as backup of existing is failing..");
                progressBar1.Value = 100;
                progressBar1.Refresh();
                return;
            }
            try
            {
                //fileId = "1jiJPklSRgho4DDx4XZIn5foTidxya6FF";
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


                var request = driveService.Files.Get(latestFileId);
                var stream = new System.IO.MemoryStream();
                request.Download(stream);

                string localPath = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
                bool exists = Directory.Exists(localPath);

                //System.IO.FileStream file = new System.IO.FileStream(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write);
                System.IO.FileStream file = new System.IO.FileStream(localPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();

                progressBar1.Value = 100;
                progressBar1.Refresh();
                MessageBox.Show("File download success...");
            }
            catch (Exception ex)
            {
                progressBar1.Value = 100;
                progressBar1.Refresh();
                MessageBox.Show("File download Error..Please verify Internet");
            }
            

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

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = "InvoiceBackup_" + DateTime.Now.ToString("dd_MMM_yy_HH_mm") + ".mdb",
                    MimeType = "application/mdb",
                    Parents = new List<string>
                    {
                        "1u_iyM9dKwJamabwuqmgiE3889MOdLlkO"
                    }
                };
                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString(),
                                        System.IO.FileMode.Open))
                {
                    request = driveService.Files.Create(
                        fileMetadata, stream, "text/csv");

                    
                    request.Fields = "id";

                    request.Upload();
                }
                var file = request.ResponseBody;
                Console.WriteLine("File ID: " + file.Id);
                fileID = file.Id;
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

            if (fileID.Length > 0)
            {
                
                return true;
            }
            return false;
        }

        //  public static System.IO.Stream DownloadFile(
        //IAuthenticator authenticator, File file)
        //  {
        //      if (!String.IsNullOrEmpty(file.DownloadUrl))
        //      {
        //          try
        //          {
        //              HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
        //                  new Uri(file.DownloadUrl));
        //              authenticator.ApplyAuthenticationToRequest(request);
        //              HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //              if (response.StatusCode == HttpStatusCode.OK)
        //              {
        //                  return response.GetResponseStream();
        //              }
        //              else
        //              {
        //                  Console.WriteLine(
        //                      "An error occurred: " + response.StatusDescription);
        //                  return null;
        //              }
        //          }
        //          catch (Exception e)
        //          {
        //              Console.WriteLine("An error occurred: " + e.Message);
        //              return null;
        //          }
        //      }
        //      else
        //      {
        //          // The file doesn't have any content stored on Drive.
        //          return null;
        //      }
        //  }
        //  private bool SaveToDir(string downloadUrl, string filename)
        //  {
        //      string filePath = @"C:\Users\SAIRAM\Desktop\bills-shop\";
        //      bool resp = false;
        //      DriveService ds = new DriveService();
        //      Uri temp = new Uri(downloadUrl);
        //      string fileId = HttpUtility.ParseQueryString(temp.Query).Get("id");
        //      var req = ds.Files.Get(fileId.Trim());
        //      var stream = new MemoryStream();
        //      req.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress dp) =>
        //      {
        //          switch (dp.Status)
        //          {
        //              case Google.Apis.Download.DownloadStatus.Downloading:
        //                  MessageBox.Show("downloading, please wait....");
        //                  break;
        //              case Google.Apis.Download.DownloadStatus.Completed:
        //                  using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //                  {
        //                      stream.WriteTo(file);
        //                      MessageBox.Show("File Downloaded successfully.");
        //                  }
        //                  resp = true;
        //                  break;
        //              case Google.Apis.Download.DownloadStatus.Failed:
        //                  MessageBox.Show("Failed to Download.");
        //                  resp = false;
        //                  break;
        //          }
        //      };
        //      req.Download(stream);


        //  }
    }
}
