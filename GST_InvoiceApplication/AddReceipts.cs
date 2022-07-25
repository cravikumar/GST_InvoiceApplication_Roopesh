using FoxLearn.License;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GST_InvoiceApplication
{
    public partial class AddReceipts : Form
    {
        public AddReceipts()
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
            textBox1.Text = ComputerInfo.GetComputerId();
            loadDetails();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new MasterPage();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }
        const int ProductCode = 9;
        private void button1_Click(object sender, EventArgs e)
        {
            KeyManager km = new KeyManager(textBox1.Text);
            KeyValuesClass kv;
            string productKey=string.Empty;
            if (true)
            {
                kv = new KeyValuesClass()
                {
                    Type = LicenseType.TRIAL,
                    Header = Convert.ToByte(9),
                    Footer = Convert.ToByte(6),
                    ProductCode = (byte)ProductCode,
                    Edition = Edition.ENTERPRISE,
                    Version = 1,
                    Expiration = DateTime.Now.Date.AddDays(Convert.ToInt32(textBox2.Text))
                };
                if (!km.GenerateKey(kv, ref productKey))
                    textBox5.Text = "Error";
                else
                {
                    label5.Visible = true;
                    textBox5.Visible = true;
                    button2.Visible = true;
                    button1.Enabled = false;
                }
            }
            textBox5.Text = productKey;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            KeyManager km = new KeyManager(textBox1.Text);
            string productKey = textBox5.Text;
            if (km.ValidKey(ref productKey))
            {
                KeyValuesClass kv = new KeyValuesClass();
                if(km.DisassembleKey(productKey,ref kv))
                {
                    LicenseInfo lic = new LicenseInfo();
                    lic.ProductKey = productKey;
                    lic.FullName = "GST Billing";
                    if(kv.Type== LicenseType.TRIAL)
                    {
                        lic.Day = kv.Expiration.Day;
                        lic.Month = kv.Expiration.Month;
                        lic.Year = kv.Expiration.Year;
                    }
                    km.SaveSuretyFile(String.Format(@"{0}\key.lic", Application.StartupPath), lic);
                    MessageBox.Show("Successfully registered");
                    //this.Close();

                }
            }
            else
                MessageBox.Show("Your Product Key is Invalid.");

            button2.Enabled = false;
            loadDetails();
        }

        private void loadDetails()
        {
            KeyManager km = new KeyManager(textBox1.Text);
            
            LicenseInfo lic = new LicenseInfo();
            int value = km.LoadSuretyFile(String.Format(@"{0}\key.lic", Application.StartupPath), ref lic);
            string productKey = lic.ProductKey;
            if (km.ValidKey(ref productKey))
            {
                KeyValuesClass kv = new KeyValuesClass();
                if (km.DisassembleKey(productKey, ref kv))
                {
                    textBox6.Text = productKey;
                    textBox4.Text = kv.Expiration.ToString();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "Vitece_213")
                button1.Visible = true;
        }
    }
}
