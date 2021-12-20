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
    public partial class AddCustomer : Form
    {
        public AddCustomer()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
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
            String sql = "Select Id,CustomerName,Address,GSTIN from CustomerData where " +
                (string.IsNullOrEmpty(textBox7.Text) ? "1=1" : "CustomerName like '%" + textBox7.Text+"%'")+
                (string.IsNullOrEmpty(textBox8.Text) ? " and 1=1" : " and GSTIN like '%" + textBox8.Text + "%'") +
                (string.IsNullOrEmpty(textBox9.Text) ? " and 1=1" : " and Address like '%" + textBox9.Text + "%'")
                ;

            DataSet ds = Functions.RunSelectSql(sql);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 4)
            {
                string sql = "Select * from CustomerData where Id = " + dataGridView1.CurrentRow.Cells[0].Value;
                DataSet ds = Functions.RunSelectSql(sql);
                DataRow dr = ds.Tables[0].Rows[0];
                textBox1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = true;
                button4.Enabled = true;

                textBox1.Text = dr["Id"].ToString();
                textBox2.Text = dr["CustomerName"].ToString();
                textBox3.Text = dr["CustomerType"].ToString();
                textBox4.Text = dr["Address"].ToString();
                textBox5.Text = dr["GSTIN"].ToString();
                textBox6.Text = dr["Aadhaar"].ToString();
                textBox10.Text = dr["PanNumber"].ToString();
                textBox11.Text = dr["MobilePhone1"].ToString();
                textBox12.Text = dr["MobilePhone2"].ToString();
                textBox13.Text = dr["OfficePhone1"].ToString();
                textBox14.Text = dr["FaxNo"].ToString();
                textBox15.Text = dr["WhatsappNo"].ToString();
                textBox16.Text = dr["CustomerNotes"].ToString();
                textBox17.Text = dr["AdditionField1"].ToString();
                

            }
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            string query = "update CustomerData set " +
                "CustomerName = '" + textBox2.Text + "'" +
                ", CustomerType = '" + textBox3.Text + "'" +
                ", Address = '" + textBox4.Text + "'" +
                ", GSTIN = '" + textBox5.Text + "'" +
                ", Aadhaar = '" + textBox6.Text + "'" +
                ", PanNumber = '" + textBox10.Text + "'" +
                ", MobilePhone1 = '" + textBox11.Text + "'" +
                ", MobilePhone2 = '" + textBox12.Text + "'" +
                ", OfficePhone1 = '" + textBox13.Text + "'" +
                ", FaxNo = '" + textBox14.Text + "'" +
                ", WhatsappNo = '" + textBox15.Text + "'" +
                ", CustomerNotes = '" + textBox16.Text + "'" +
                ", AdditionField1 = '" + textBox17.Text + "'" +
                ", AdditionField2 = '" + "-" + "'" +
                ", AdditionField3 = '" + "-" + "'" +
                ", AdditionField4 = '" + "-" + "'" +
                " where Id = " + textBox1.Text;
            try { Functions.RunExecuteNonQuery(query); }
            catch
            {

            }

            MessageBox.Show("Updated");

        }
        /// <summary>
        /// add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length < 1)
            {
                MessageBox.Show("Please add Customer Name");
                return;
            }

            string query = "insert into CustomerData " +
                "(CustomerName,CustomerType,Address,GSTIN,Aadhaar,PanNumber,MobilePhone1,MobilePhone2,OfficePhone1," +
                "FaxNo,WhatsappNo,CustomerNotes,AdditionField1,AdditionField2,AdditionField3,AdditionField4) Values " +
                 " ('" + textBox2.Text +
                 "','" + textBox3.Text +
                 "','" + textBox4.Text +
                 "','" + textBox5.Text +
                 "','" + textBox6.Text +
                 "','" + textBox10.Text +
                 "','" + textBox11.Text +
                 "','" + textBox12.Text +//mobil2
                 "','" + textBox13.Text +//office1
                 "','" + textBox14.Text +//fax
                 "','" + textBox15.Text +//whatsapp
                 "','" + textBox16.Text +//Notes
                 "','" + textBox17.Text +//add1
                 "','" + "-" +//add2
                 "','" + "-" +//add3
                 "','" + "-" +//add4
                 "')";
            try { Functions.RunExecuteNonQuery(query); }
            catch
            {

            }
            MessageBox.Show("Added");
        }
        /// <summary>
        /// clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox6.Text = string.Empty;
            textBox10.Text = string.Empty;
            textBox11.Text = string.Empty;
            textBox12.Text = string.Empty;
            textBox13.Text = string.Empty;
            textBox14.Text = string.Empty;
            textBox15.Text = string.Empty;
            textBox16.Text = string.Empty;
            textBox17.Text = string.Empty;

            button3.Enabled = false;
            button2.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string query = "Delete * From CustomerData " +
         " where Id = " + textBox1.Text;
            try { Functions.RunExecuteNonQuery(query); }
            catch
            {

            }

            MessageBox.Show("Deleted");

            button4_Click(sender, e);
            button1_Click(sender, e);


        }
    }
}
