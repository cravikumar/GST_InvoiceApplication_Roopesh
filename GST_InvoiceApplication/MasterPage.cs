using FoxLearn.License;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GST_InvoiceApplication
{
    public partial class MasterPage : Form
    {
        public MasterPage()
        {
            

            InitializeComponent();
            
            this.WindowState = FormWindowState.Maximized;
            
            //DateTime dt = DateTime.TryParseExact((Convert.ToDouble(key)/8).ToString() , "ddMMyyyy",dt);
            //this.FormBorderStyle = FormBorderStyle.None;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new AddInvoice();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private DateTime getExipryDate()
        {
            KeyManager km = new KeyManager(ComputerInfo.GetComputerId());

            LicenseInfo lic = new LicenseInfo();
            int value = km.LoadSuretyFile(String.Format(@"{0}\key.lic", Application.StartupPath), ref lic);
            string productKey = lic.ProductKey;
            if (km.ValidKey(ref productKey))
            {
                KeyValuesClass kv = new KeyValuesClass();
                if (km.DisassembleKey(productKey, ref kv))
                {
                    
                    return kv.Expiration;
                }
            }
            return new DateTime();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new SetupCompany();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private bool licenseCheck()
        {
            DateTime ex = getExipryDate();

            if (ex > DateTime.Now)
            { return true; }
            else {
                MessageBox.Show("Invalid License..!!");
                return false;
            }
            
            bool valid = false;
            bool comValid = false;

            string computerName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            if (ConfigurationManager.AppSettings["Check"] == "84")
            {
                if (computerName == ConfigurationManager.AppSettings["ComputerName"])
                {
                    comValid = true;
                }
                else
                { MessageBox.Show("Please renew software License. contact 9791440009"); }
            }
            else
                MessageBox.Show(computerName);

            string key = ConfigurationManager.AppSettings["Key"];
            DateTime dt;
            if (DateTime.TryParseExact((Convert.ToDouble(key) / 8).ToString(), "ddMMyyyy", CultureInfo.InvariantCulture,
                                 DateTimeStyles.None, out dt))
            {
                if (dt > DateTime.Now)
                {
                    valid = true;
                }
                else
                { MessageBox.Show("Please renew software License. contact 9791440009"); }
            }
            else
            { MessageBox.Show("Please renew software License. contact 9791440009"); }

            return valid && comValid;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new AddCustomer();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new ProductSetup();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new AddReceipts();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new SalesReport();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new SalesBook();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
                
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new TallyUtility();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new HSNSalesReport();
            form2.Closed += (s, args) => this.Close();
            form2.Show();

        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new HSNSalesReportNew();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
            this.Hide();
            var form2 = new TallyExport();
            form2.Closed += (s, args) => this.Close();
            form2.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!licenseCheck())
                return;
        }
    }
}
