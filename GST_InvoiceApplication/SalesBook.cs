using Invoice.DataAccess;
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
    public partial class SalesBook : Form
    {
        public SalesBook()
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
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
            String sql = "Select Id,CustomerName,GSTIN from CustomerData where " +
               (string.IsNullOrEmpty(textBox7.Text) ? "1=1" : "CustomerName like '%" + textBox7.Text + "%'") +
               (string.IsNullOrEmpty(textBox8.Text) ? " and 1=1" : " and GSTIN like '%" + textBox8.Text + "%'") +
               (string.IsNullOrEmpty(textBox9.Text) ? " and 1=1" : " and Address like '%" + textBox9.Text + "%'")
               ;

            DataSet ds = Functions.RunSelectSql(sql);
            dataGridView2.DataSource = ds.Tables[0];
        }
    }
}
