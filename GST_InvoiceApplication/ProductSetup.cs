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
    public partial class ProductSetup : Form
    {
        AutoCompleteStringCollection _productList;
        public AutoCompleteStringCollection productList
        {
            get
            {
                if (_productList != null && _productList.Count > 0)
                    return _productList;
                else
                    _productList = LoadProducts();
                return _productList;
            }
        }

        private AutoCompleteStringCollection LoadProducts()
        {
            string sql = "Select DISTINCT ProductName from ProductMaster;";
            DataSet ds = Functions.RunSelectSql(sql);

            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            foreach (DataRow dr in ds.Tables[0].Rows)
                str.Add(dr["ProductName"].ToString());
            return str;

        }
        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            return productList;
        }
        public ProductSetup()
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();

            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox1.AutoCompleteCustomSource = AutoCompleteLoad();
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

            LoadHSNGrid();
        }

        private void LoadHSNGrid()
        {
            String sql = "Select Id,ProductName,Price,HSNCode from ProductMaster where ProductName like '%" + "HSNCODE-" + "%'";
                

            DataSet ds = Functions.RunSelectSql(sql);
            dataGridView2.DataSource = ds.Tables[0];
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
            String sql = "Select Id,ProductName,Price,HSNCode from ProductMaster where " +
                (string.IsNullOrEmpty(textBox1.Text) ? "1=1" : "ProductName like '%" + textBox1.Text + "%'") + 
                (string.IsNullOrEmpty(textBox2.Text) ? " and 1=1" : " and HSNCode like '%" + textBox2.Text + "%'");

            DataSet ds = Functions.RunSelectSql(sql);
            dataGridView1.DataSource = ds.Tables[0];

        }

        private void ProductSetup_Load(object sender, EventArgs e)
        {
            String sql = "Select Id,ProductName,Price,HSNCode from ProductMaster where " +
                (string.IsNullOrEmpty(textBox1.Text) ? "1=1" : "ProductName like '%" + textBox1.Text + "%'") +
                (string.IsNullOrEmpty(textBox2.Text) ? " and 1=1" : " and HSNCode like '%" + textBox2.Text + "%'");

            DataSet ds = Functions.RunSelectSql(sql);
            dataGridView1.DataSource = ds.Tables[0];
            // TODO: This line of code loads data into the 'gSTDataSet.ProductMaster' table. You can move, or remove it, as needed.
            //this.productMasterTableAdapter.Fill(this.gSTDataSet.ProductMaster);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 4)
            {
                string sql = "Delete * From ProductMaster where Id = " + dataGridView1.CurrentRow.Cells[0].Value;
                try { Functions.RunExecuteNonQuery(sql); }
                catch
                {

                }

                button1_Click(sender, e);

            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex == 3)
            {
                string sql = "Delete * From ProductMaster where Id = " + dataGridView2.CurrentRow.Cells[0].Value;
                try { Functions.RunExecuteNonQuery(sql); }
                catch
                {

                }

                LoadHSNGrid();

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Please enter a valid text");
                return;
            }
            string sql = "insert into ProductMaster " +
                               "(ProductName,Price,HSNCode) values " +
                               " ('" + "HSNCODE-" + textBox3.Text+
                                "','" + "1" +
                                  "','" + textBox4.Text + "');";

            Functions.RunExecuteNonQuery(sql);

            LoadHSNGrid();
            textBox3.Text = "";
            textBox4.Text = "";
        }
    }
}
