using Invoice.DataAccess;
using Invoice.Models;
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
    public partial class AddInvoice : Form
    {
        CheckBox headerCheckBox = new CheckBox();
        //AutoComplete(ComboBox cb, System.Windows.Forms.KeyPressEventArgs e, bool blnLimitToList);
        public CompanyDetails _selectedCompany;
        InvoiceDetails _savedInvoice;
        public int publicSaleId;
        bool notlastColumn = true; //class level variable--- to check either last column is reached or not
        public bool _isDiscountEditedManually=false;
        public bool dontUncheckAll = false;
        AutoCompleteStringCollection _productList;
        AutoCompleteStringCollection _hsnList;
        public bool isFormModified = false;
        public bool isFirstSearchClick = false;
        public InvoiceDetails searchInv1;
        public double _defaultTax = Convert.ToDouble(ConfigurationManager.AppSettings["TaxPercent"]);
        
        public AutoCompleteStringCollection productList
        {
            get
            {
                if (_productList != null && _productList.Count>0)
                    return _productList;
                else
                    _productList = LoadProducts();
                return _productList;
            }
        }

        public AutoCompleteStringCollection hsnList
        {
            get
            {
                if (_hsnList != null && _hsnList.Count > 0)
                    return _hsnList;
                else
                    _hsnList = LoadhsnList();
                return _hsnList;
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

        private AutoCompleteStringCollection LoadhsnList()
        {
            string sql = "Select DISTINCT ProductName,HSNCode from ProductMaster where ProductName like '%HSNCODE-%'"; 
            DataSet ds = Functions.RunSelectSql(sql);

            AutoCompleteStringCollection str = new AutoCompleteStringCollection();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                str.Add(dr["ProductName"].ToString().Split(new string[] { "HSNCODE-" }, StringSplitOptions.None)[1]);
            }
            return str;

        }



        private string getCustomerPrice(string ProductName)
        {
            string sql = "Select top 1 Price from ProductMaster where ProductName='" + ProductName + "'";
            DataSet ds = Functions.RunSelectSql(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
                return dr["Price"].ToString();

            return "";
        }

        private string getProductHSN(string ProductName)
        {
            string sql = "Select top 1 HSNCode from ProductMaster where ProductName='" + ProductName + "'";
            DataSet ds = Functions.RunSelectSql(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
                return dr["HSNCode"].ToString();

            return "";
        }


        private void updatePriceMaster(string ProductName, string Price, string HsnCode)
        {
            try {
                if (!string.IsNullOrEmpty(getCustomerPrice(ProductName)))
                {
                    string query = "Update ProductMaster Set Price = '" + Price + "',HsnCode = '"+HsnCode+"' where ProductName = '" + ProductName+"';";
                    Functions.RunExecuteNonQuery(query);

                }
                else
                {
                    string sql = "insert into ProductMaster " +
                                "(ProductName,Price,HSNCode) values " +
                                " ('" + ProductName.Replace("'", "") +
                                 "','" + Price +
                                   "','" + HsnCode + "');";

                    Functions.RunExecuteNonQuery(sql);
 
                }
            }
            catch
            {}
 
        }

        // AutoComplete
        public void AutoComplete(ComboBox cb, System.Windows.Forms.KeyPressEventArgs e)
        {
            this.AutoComplete(cb, e, false);
        }

        public void AutoComplete(ComboBox cb, System.Windows.Forms.KeyPressEventArgs e, bool blnLimitToList)
        {
            string strFindStr = "";

            if (e.KeyChar == (char)8)
            {
                if (cb.SelectionStart <= 1)
                {
                    cb.Text = "";
                    return;
                }

                if (cb.SelectionLength == 0)
                    strFindStr = cb.Text.Substring(0, cb.Text.Length - 1);
                else
                    strFindStr = cb.Text.Substring(0, cb.SelectionStart - 1);
            }
            else
            {
                if (cb.SelectionLength == 0)
                    strFindStr = cb.Text + e.KeyChar;
                else
                    strFindStr = cb.Text.Substring(0, cb.SelectionStart) + e.KeyChar;
            }

            int intIdx = -1;

            // Search the string in the ComboBox list.

            intIdx = cb.FindString(strFindStr);

            if (intIdx != -1)
            {
                cb.SelectedText = "";
                cb.SelectedIndex = intIdx;
                cb.SelectionStart = strFindStr.Length;
                cb.SelectionLength = cb.Text.Length;
                e.Handled = true;
            }
            else
            {
                e.Handled = blnLimitToList;
            }
        }

        public AddInvoice()
        {
            string key = ConfigurationManager.AppSettings["Key"];
            DateTime dt;
            if (DateTime.TryParseExact((Convert.ToDouble(key) / 8).ToString(), "ddMMyyyy", CultureInfo.InvariantCulture,
                                 DateTimeStyles.None, out dt))
            {
                if (dt > DateTime.Now)
                {  }
                else
                { return; }
            }
            else
            { return; }

            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            LoadCustomerData();
            LoadCompanyData();
            searchInv1 = null;

            //CheckBox checkbox = new CheckBox();
            headerCheckBox.Size = new System.Drawing.Size(15, 15);
            headerCheckBox.BackColor = Color.Transparent;

            // Reset properties
            headerCheckBox.Padding = new Padding(0);
            headerCheckBox.Margin = new Padding(0);
            headerCheckBox.Text = "";

            // Add checkbox to datagrid cell
            dataGridView2.Controls.Add(headerCheckBox);
            DataGridViewHeaderCell header = dataGridView2.Columns[12].HeaderCell;
            headerCheckBox.Location = new Point(
                1215,
                5
            );

            headerCheckBox.Click += new EventHandler(HeaderCheckBox_Clicked);
            textBox16.Text = textBox17.Text = (_defaultTax / 2).ToString();
            ChangeCasing();


        }

        public AddInvoice(bool isCashBill)
        {
            string key = ConfigurationManager.AppSettings["Key"];
            DateTime dt;
            if (DateTime.TryParseExact((Convert.ToDouble(key) / 8).ToString(), "ddMMyyyy", CultureInfo.InvariantCulture,
                                 DateTimeStyles.None, out dt))
            {
                if (dt > DateTime.Now)
                { }
                else
                { return; }
            }
            else
            { return; }

            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            LoadCustomerData();
            LoadCompanyData();
            searchInv1 = null;

            //CheckBox checkbox = new CheckBox();
            headerCheckBox.Size = new System.Drawing.Size(15, 15);
            headerCheckBox.BackColor = Color.Transparent;

            // Reset properties
            headerCheckBox.Padding = new Padding(0);
            headerCheckBox.Margin = new Padding(0);
            headerCheckBox.Text = "";

            // Add checkbox to datagrid cell
            dataGridView2.Controls.Add(headerCheckBox);
            DataGridViewHeaderCell header = dataGridView2.Columns[12].HeaderCell;
            headerCheckBox.Location = new Point(
                1215,
                5
            );

            headerCheckBox.Click += new EventHandler(HeaderCheckBox_Clicked);
            textBox16.Text = textBox17.Text = (_defaultTax / 2).ToString();
            ChangeCasing();

            if (isCashBill)
            {
                string sql = "select * from CompanyData where DefaultCashBill = 'True'";
                var ds = Functions.RunSelectSql(sql);
                DataRow dr = ds.Tables[0].Rows[0];

                CompanyDetails currentCompany = new CompanyDetails();
                currentCompany.CompanyID = Convert.ToInt32(dr["ID"]);
                currentCompany.CompanyName = dr["CompanyName"].ToString();
                comboBox1.SelectedValue = currentCompany.CompanyID;

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
                currentCompany.DefaultCashBill = Convert.ToBoolean(dr["DefaultCashBill"]);
                currentCompany.ThermalPrinter = Convert.ToBoolean(dr["ThermalPrinter"]);
                currentCompany.DefaultPrinter = dr["DefaultPrinter"].ToString();

                _selectedCompany = currentCompany;

            }
        }
        private void ChangeCasing()
        {
            textBox1.CharacterCasing = CharacterCasing.Upper;
            textBox2.CharacterCasing = CharacterCasing.Upper;
            textBox6.CharacterCasing = CharacterCasing.Upper;
            textBox5.CharacterCasing = CharacterCasing.Upper;
            textBox19.CharacterCasing = CharacterCasing.Upper;
            textBox4.CharacterCasing = CharacterCasing.Upper;
            textBox3.CharacterCasing = CharacterCasing.Upper;
        }

        private void HeaderCheckBox_Clicked(object sender, EventArgs e)
        {
            //Necessary to end the edit mode of the Cell.
            dataGridView2.EndEdit();

            //Loop and check and uncheck all row CheckBoxes based on Header Cell CheckBox.
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells[8].Value !=null && !String.IsNullOrEmpty(row.Cells[8].Value.ToString()) )
                {
                    DataGridViewCheckBoxCell checkBox = (row.Cells["Select"] as DataGridViewCheckBoxCell);
                    checkBox.Value = headerCheckBox.Checked;
                }
            }
        }
        public AddInvoice(int InvoiceId, int companyId, string year="")
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
            //LoadCompanyData();
            ChangeCasing();
            //comboBox1.SelectedItem = "Test";
            isFirstSearchClick = true;
            string sql = "select * from CompanyData where ID = " + companyId.ToString();
            var ds = Functions.RunSelectSql(sql);
            DataRow dr = ds.Tables[0].Rows[0];

            CompanyDetails currentCompany = new CompanyDetails();
            currentCompany.CompanyID = Convert.ToInt32(dr["ID"]);
            currentCompany.CompanyName = dr["CompanyName"].ToString();
            comboBox1.SelectedItem = currentCompany.CompanyID;

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
            currentCompany.DefaultCashBill = Convert.ToBoolean(dr["DefaultCashBill"]);
            currentCompany.ThermalPrinter = Convert.ToBoolean(dr["ThermalPrinter"]);
            currentCompany.DefaultPrinter = dr["DefaultPrinter"].ToString();

            _selectedCompany = currentCompany;

            if (currentCompany.IsGSTApplicable == false)
            {
                textBox6.Visible = false;
                label10.Visible = false;
                textBox13.Visible = false;
                textBox7.Visible = false;
                textBox8.Visible = false;
                textBox9.Visible = false;
                textBox16.Visible = false;
                textBox17.Visible = false;
                textBox14.Visible = false;
                //inv.StrIGST;
                textBox14.Visible = false;
                textBox11.Visible = false;
                textBox18.Visible = false;

                label17.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label20.Visible = false;
                label13.Visible = false;
                label15.Visible = false;
                label21.Visible = false;
                label22.Visible = false;
                label18.Visible = false;

            }
            else
            {
                textBox13.Visible = false;
                textBox7.Visible = false;
                textBox8.Visible = true;
                textBox9.Visible = true;
                textBox16.Visible = true;
                textBox17.Visible = true;
                textBox14.Visible = false;
                textBox11.Visible = true;
                textBox18.Visible = true;
                //inv.StrIGST;
                textBox14.Visible = false;
                label17.Visible = false;
                label11.Visible = false;
                label12.Visible = true;
                label20.Visible = true;
                label13.Visible = true;
                label15.Visible = true;
                label21.Visible = true;
                label22.Visible = true;
                label18.Visible = false;

                textBox6.Visible = true;
                label10.Visible = true;
            }
            InvoiceDetails searchInv = getInvoiceDetails(_selectedCompany.CompanyID, InvoiceId.ToString(),false,year);

            SortedDictionary<int, string> userCache = new SortedDictionary<int, string>();
            userCache.Add(0, _selectedCompany.CompanyName);
            
            comboBox1.DataSource = new BindingSource(userCache, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.Enabled = false;
            textBox12.Enabled = false;
            button1.Enabled = false;//Save
            button2.Enabled = false;//Save
            button3.Visible = false;//Save
            button4.Visible = false;//Save
            button6.Visible = false;//Save

            comboBox3.Visible = false;
            button8.Visible = false;
            textBox22.Visible = false;

            updateTotalBill();

            if (!string.IsNullOrEmpty(year))
                searchInv1 = searchInv;

        }


        private void LoadCustomerData()
        {
            string sql = "select * from customerdata Order by CustomerName ASC";
            var ds = Functions.RunSelectSql(sql);
            Dictionary<int, string> userCache = new Dictionary<int, string>();
            userCache.Add(0, "--Select--");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                userCache.Add(Convert.ToInt32(dr["ID"]), dr["CustomerName"].ToString() + " - " + dr["Address"].ToString());

            }
            comboBox2.DataSource = new BindingSource(userCache, null);
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
        }

        public void LoadCompanyData()
        {
            List<CompanyDetails> companies = GetCompanyData();

            List<CompanyDetails> allCompanies = GetAllCompanyData();

            SortedDictionary<int, string> userCache = new SortedDictionary<int, string>();
            userCache.Add(0, "--Select--");
            foreach (CompanyDetails curr in companies)
            {
                userCache.Add(curr.CompanyID, curr.CompanyName);

            }
            comboBox1.DataSource = new BindingSource(userCache, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";

            userCache = new SortedDictionary<int, string>();
            userCache.Add(0, "--Select--");
            foreach (CompanyDetails curr in allCompanies)
            {
                userCache.Add(curr.CompanyID, curr.CompanyName);

            }

            comboBox3.DataSource = new BindingSource(userCache, null);
            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
        }

        private bool isGSTApplicableForCompany(int companyId)
        {
            string sql = "select * from CompanyData where ID = " + companyId.ToString();
            var ds = Functions.RunSelectSql(sql);
            DataRow dr = ds.Tables[0].Rows[0];

            return Convert.ToBoolean(dr["IsGSTApplicable"]);

             
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (isFormModified)
            {
                if (MessageBox.Show("Bill has been modified, all the unsaved data will be lost. Are you ok to close?", "Close Form",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    isFormModified = false;
                }
            }
            this.Hide();
            var form2 = new MasterPage();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void AddInvoice_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gSTDataSet.InvoiceProductDetails' table. You can move, or remove it, as needed.
            //this.invoiceProductDetailsTableAdapter.Fill(this.gSTDataSet.InvoiceProductDetails);
            ChangeCasing();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex < 1)
                return;

            string sql = "select * from customerdata where ID = " + comboBox2.SelectedValue;
            var ds = Functions.RunSelectSql(sql);

            textBox1.Text = ds.Tables[0].Rows[0]["CustomerName"].ToString();
            textBox2.Text = ds.Tables[0].Rows[0]["Address"].ToString();
            textBox3.Text = ds.Tables[0].Rows[0]["MobilePhone1"].ToString();
            textBox4.Text = ds.Tables[0].Rows[0]["Aadhaar"].ToString();
            //textBox5.Text = ds.Tables[0].Rows[0]["PanNumber"].ToString();
            textBox6.Text = ds.Tables[0].Rows[0]["GSTIN"].ToString();


            if (textBox6.Text.Length > 3 && !textBox6.Text.StartsWith("37"))
            {
                textBox18.Text = _defaultTax.ToString();
            }
            else
            {
                textBox18.Text = "";
                textBox16.Text = (_defaultTax/2).ToString();
                textBox17.Text = (_defaultTax / 2).ToString();
            }
            updateTotalBill();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int Id = 0;
            bool converted = int.TryParse(dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[11].Value.ToString(), out Id);
            int column = dataGridView2.CurrentCell.ColumnIndex;
            if (column == 9)
            {
                //Code to delete row
                dataGridView2.Rows.RemoveAt(e.RowIndex);
                updateTotalBill();
            }
            if (column == 12)
            {
                bool isChecked = true;
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Select"].EditedFormattedValue) == false)
                    {
                        if (row.Cells[8].Value != null && !String.IsNullOrEmpty(row.Cells[8].Value.ToString()))
                        {
                            isChecked = false;
                            break;
                        }
                            
                    }
                }
                headerCheckBox.Checked = isChecked;
            }

            if (converted && Id > 0 && column == 9)
            {
                //Functions.RunExecuteNonQuery("Delete * From InvoiceProductDetails where Id = " + Id);
            }

        }

        private void updateTotalBill()
        {
            double totalBillValue = 0;
            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                if (!(dataGridView2.Rows[i].Cells[8].Value == DBNull.Value))
                {
                    if (_selectedCompany != null && !_selectedCompany.IsGSTApplicable && dataGridView2.Rows[i].Cells[6].Value!=null)
                    {
                        if (dataGridView2.Rows[i].Cells[7].Value.ToString() != dataGridView2.Rows[i].Cells[6].Value.ToString())
                        {
                            dataGridView2.Rows[i].Cells[7].Value = dataGridView2.Rows[i].Cells[6].Value;


                        }
                    }
                    totalBillValue += Convert.ToDouble(dataGridView2.Rows[i].Cells[8].Value);
                }
            }

            if (totalBillValue > 0)
            {
                double sgst = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToDouble(textBox16.Text);
                double cgst = string.IsNullOrEmpty(textBox17.Text) ? 0 : Convert.ToDouble(textBox17.Text);
                double igst = string.IsNullOrEmpty(textBox18.Text) ? 0 : Convert.ToDouble(textBox18.Text);
                if (_selectedCompany != null && !_selectedCompany.IsGSTApplicable)
                    sgst = cgst = igst = 0;

                double discount=0;

                if (!_isDiscountEditedManually)
                {
                    if (igst > 0)
                    {
                        discount = tallyRound((totalBillValue * (igst / (100 + igst))), 2);

                        //sgst = igst / 2;
                        //cgst = igst / 2;
                    }
                    else
                        discount = tallyRound((totalBillValue * ((sgst + cgst) / (100 + sgst + cgst))), 2);
                }
                else
                {

                    if(!double.TryParse(textBox13.Text, out discount));
                    {
                        if (discount == 0)
                        {
                            MessageBox.Show("Please check Discount price..!!");
                         
                        }
                    }
                    
                }

                discount = 0;//removed discount

                double Billerror = totalBillValue - Math.Truncate(totalBillValue);
                double Discerror = discount - Math.Truncate(discount);
                double error = discount > 0 ? Billerror - Discerror : 0;

                discount = discount + tallyRound(error, 2);
                double totalbeforeTax = totalBillValue;//removed discount - discount;

                double totalsgst = 0;
                double totalcgst = 0;
                double beforeRound = 0;
                double totaligst = 0;
                if (igst > 0)
                {
                    totaligst = tallyRound((totalbeforeTax * igst * 0.01), 2);
                    beforeRound = totalbeforeTax + totaligst;
                }
                else
                {
                    totalsgst = tallyRound((totalbeforeTax * sgst * 0.01), 2);
                    totalcgst = tallyRound((totalbeforeTax * cgst * 0.01), 2);
                    beforeRound = totalbeforeTax + totalsgst + totalcgst;
                }

                double rounded = tallyRound(beforeRound, 0);

                textBox20.Text = String.Format("{0:n}", (totalBillValue));//.ToString("C", CultureInfo.CurrentCulture);//Discount
                textBox13.Text = String.Format("{0:n}", (discount));//.ToString("C", CultureInfo.CurrentCulture);//Discount
                textBox7.Text = String.Format("{0:n}", totalbeforeTax);//.ToString("C", CultureInfo.CurrentCulture);//TotalAfterDiscount


                textBox8.Text = String.Format("{0:n}", (totalsgst));//.ToString("C", CultureInfo.CurrentCulture);//SGST
                textBox9.Text = String.Format("{0:n}", (totalcgst));//.ToString("C", CultureInfo.CurrentCulture); ;//CGST
                if (igst > 0)
                {
                    textBox11.Text = String.Format("{0:n}", totaligst);//IGST
                    textBox8.Text = textBox9.Text = string.Empty;
                }
                else
                    textBox11.Text = string.Empty;

                textBox14.Text = String.Format("{0:n}", (beforeRound));//.ToString("C", CultureInfo.CurrentCulture);//TotalAfterTax
                textBox15.Text = String.Format("{0:n}", (rounded - beforeRound));//.ToString("C", CultureInfo.CurrentCulture);//Rounded

                if (textBox10.Text != String.Format("{0:n}", (rounded)))
                {
                    //MessageBox.Show("Error in Calculation previously, Now updated");
                }

                textBox10.Text = String.Format("{0:n}", (rounded));//.ToString("C", CultureInfo.CurrentCulture);//AmountPayable

                //textBox9.Text = textBox8.Text = ((totalBillValue * 2.5) / 100).ToString("C", CultureInfo.CurrentCulture);

                //textBox10.Text = (double.Parse(textBox7.Text, NumberStyles.Currency, CultureInfo.CurrentCulture) + double.Parse(textBox8.Text, NumberStyles.Currency, CultureInfo.CurrentCulture) + double.Parse(textBox9.Text, NumberStyles.Currency, CultureInfo.CurrentCulture)).ToString("C", CultureInfo.CurrentCulture);
            }
            else
            {
                textBox20.Text = "";
                textBox13.Text = "";
                textBox7.Text = "";
                textBox8.Text = "";
                textBox9.Text = "";
                textBox11.Text = "";
                textBox14.Text = "";
                textBox15.Text = "";
                textBox10.Text = "";

            }

        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SetTextChanged(sender, e);
            if (button1.Enabled || button5.Enabled)
            { }
            else
            {
                MessageBox.Show("Save/Update bill is disabled. Please click on Search button before editing..");
                return;
            }

            if (dataGridView2.CurrentCell == null)
                return;
            if (dataGridView2.CurrentCell.Style.BackColor == Color.Red && !dataGridView2.CurrentRow.IsNewRow && !String.IsNullOrEmpty(dataGridView2.CurrentCell.Value.ToString()))
                dataGridView2.CurrentCell.Style.BackColor = Color.White;

            int column = dataGridView2.CurrentCell.ColumnIndex;
            if (column == 0 )
            {
                
                    //SendKeys.Send("{Up}");
                    //SendKeys.Send("{Right}");
                
                    if (dataGridView2.CurrentRow.Cells[0].Value != null && dataGridView2.CurrentRow.Cells[0].Value.ToString().Length > 0 && ConfigurationManager.AppSettings["PricePredictionEnabled"] == "true")
                    dataGridView2.CurrentRow.Cells[1].Value = getCustomerPrice(dataGridView2.CurrentRow.Cells[0].Value.ToString());

                dataGridView2.CurrentRow.Cells[3].Value = dataGridView2.CurrentRow.Cells[0].Value!=null?   getProductHSN(dataGridView2.CurrentRow.Cells[0].Value.ToString()):"";
                
            }

            if (column == 3 && dataGridView2.CurrentRow.Cells[3] !=null && dataGridView2.CurrentRow.Cells[3].Value != null)
            {
                string hsn = getProductHSN("HSNCODE-" + dataGridView2.CurrentRow.Cells[3].Value.ToString());
                if (!string.IsNullOrEmpty(hsn))
                    dataGridView2.CurrentRow.Cells[3].Value = hsn;
            }

            if (column == 2 && dataGridView2.CurrentRow.Cells[2].Value != null)
            {
                string[] mtsList = dataGridView2.CurrentRow.Cells[2].Value.ToString().Split('+');
                double mts = 0;
                foreach (string s in mtsList)
                {
                    mts = mts + Convert.ToDouble(s);
                }

                dataGridView2.CurrentRow.Cells[4].Value = mtsList.Count();
                dataGridView2.CurrentRow.Cells[5].Value = mts.ToString();
                dataGridView2.CurrentRow.Cells[4].ReadOnly = true;
                dataGridView2.CurrentRow.Cells[5].ReadOnly = true;
            }
            else
            {
                dataGridView2.CurrentRow.Cells[4].ReadOnly = false;
                dataGridView2.CurrentRow.Cells[5].ReadOnly = false;
            }


            //if (column == 3 || column == 4 || column == 5)
            {

                double price = dataGridView2.CurrentRow.Cells[6].Value != null && !string.IsNullOrEmpty(dataGridView2.CurrentRow.Cells[6].Value.ToString()) ? Convert.ToDouble(dataGridView2.CurrentRow.Cells[6].Value.ToString()) : 0;
                double qnt = dataGridView2.CurrentRow.Cells[4].Value != null && !string.IsNullOrEmpty(dataGridView2.CurrentRow.Cells[4].Value.ToString()) ? Convert.ToDouble(dataGridView2.CurrentRow.Cells[4].Value.ToString()) : 0;
                // var gst = dataGridView2.CurrentRow.Cells[6].Value;

                double priceWithoutGST = 0;

                double sgst = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToDouble(textBox16.Text);
                double cgst = string.IsNullOrEmpty(textBox17.Text) ? 0 : Convert.ToDouble(textBox17.Text);
                double igst = string.IsNullOrEmpty(textBox18.Text) ? 0 : Convert.ToDouble(textBox18.Text);
                if (_selectedCompany!= null && !_selectedCompany.IsGSTApplicable)
                    sgst = cgst = igst = 0;


                if (igst > 0)
                {
                    priceWithoutGST = price;// priceRound( price - (price * (igst / (100 + igst))),2);

                    //sgst = igst / 2;
                    //cgst = igst / 2;
                }
                else
                    priceWithoutGST = price;//priceRound(price - (price * ((sgst + cgst) / (100 + sgst + cgst))),2);

                dataGridView2.CurrentRow.Cells[7].Value = priceWithoutGST;


                if (price > 0 && qnt > 0 && priceWithoutGST>0)
                {
                    double rowTotal = 0;
                    if (dataGridView2.CurrentRow.Cells[5].Value != null && !String.IsNullOrEmpty(dataGridView2.CurrentRow.Cells[5].Value.ToString()) && Convert.ToDouble(dataGridView2.CurrentRow.Cells[5].Value.ToString()) > 0)
                    {
                        qnt = Convert.ToDouble(dataGridView2.CurrentRow.Cells[5].Value.ToString());
                         rowTotal =priceRound( Math.Round((priceWithoutGST * qnt), 2),2);
                    }
                    else
                         rowTotal = Math.Round((priceWithoutGST * qnt), 2);
                    dataGridView2.CurrentRow.Cells[8].Value = rowTotal;

                    dataGridView2.CurrentRow.Cells[8].Style.BackColor = Color.White;

                    updateTotalBill();

                }

            }

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new InvoicePrint().Show();
        }

        private List<CompanyDetails> GetCompanyData()
        {

            string sql = "select * from CompanyData";
            if (!checkBox1.Checked)
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
                currentCompany.DefaultCashBill = Convert.ToBoolean(dr["DefaultCashBill"]);
                currentCompany.ThermalPrinter = Convert.ToBoolean(dr["ThermalPrinter"]);
                currentCompany.DefaultPrinter = dr["DefaultPrinter"].ToString();
                companyList.Add(currentCompany);

            }
            return companyList;
        }

        private List<CompanyDetails> GetAllCompanyData()
        {

            string sql = "select * from CompanyData";
            if (false)
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
                currentCompany.DefaultCashBill = Convert.ToBoolean(dr["DefaultCashBill"]);
                currentCompany.ThermalPrinter = Convert.ToBoolean(dr["ThermalPrinter"]);
                currentCompany.DefaultPrinter = dr["DefaultPrinter"].ToString();
                companyList.Add(currentCompany);

            }
            return companyList;
        }

        public bool isValidGrid()
        {
            bool isValid = true;
            if (!(dataGridView2.Rows.Count > 1))
            {
                isValid = false;
                dataGridView2.Rows[0].Cells[6].Style.BackColor = Color.Red;
                dataGridView2.Rows[0].Cells[4].Style.BackColor = Color.Red;
                dataGridView2.Rows[0].Cells[8].Style.BackColor = Color.Red;
                dataGridView2.ClearSelection();
                return isValid;
            }

            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                if (dataGridView2.Rows[i].IsNewRow)
                    continue;

                if (dataGridView2.Rows[i].Cells[6].Value == null || String.IsNullOrEmpty(dataGridView2.Rows[i].Cells[6].Value.ToString()))
                {
                    dataGridView2.Rows[i].Cells[6].Style.BackColor = Color.Red;
                    isValid = false;
                }
                else
                    dataGridView2.Rows[i].Cells[6].Style.BackColor = Color.White;

                if (dataGridView2.Rows[i].Cells[4].Value == null || String.IsNullOrEmpty(dataGridView2.Rows[i].Cells[4].Value.ToString()))
                {
                    dataGridView2.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    isValid = false;
                }
                else
                    dataGridView2.Rows[i].Cells[4].Style.BackColor = Color.White;

                if (dataGridView2.Rows[i].Cells[8].Value == null || String.IsNullOrEmpty(dataGridView2.Rows[i].Cells[8].Value.ToString()))
                {
                    dataGridView2.Rows[i].Cells[8].Style.BackColor = Color.Red;
                    isValid = false;
                }
                else
                    dataGridView2.Rows[i].Cells[8].Style.BackColor = Color.White;

            }
            dataGridView2.ClearSelection();
            return isValid;
        }

        private void recomputeRateByCompany()
        {
            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                double price = dataGridView2.Rows[i].Cells[6].Value != null && !string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[6].Value.ToString()) ? Convert.ToDouble(dataGridView2.Rows[i].Cells[6].Value.ToString()) : 0;
                double qnt = dataGridView2.Rows[i].Cells[4].Value != null && !string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[4].Value.ToString()) ? Convert.ToDouble(dataGridView2.Rows[i].Cells[4].Value.ToString()) : 0;
                // var gst = dataGridView2.CurrentRow.Cells[6].Value;

                double priceWithoutGST = 0;

                double sgst = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToDouble(textBox16.Text);
                double cgst = string.IsNullOrEmpty(textBox17.Text) ? 0 : Convert.ToDouble(textBox17.Text);
                double igst = string.IsNullOrEmpty(textBox18.Text) ? 0 : Convert.ToDouble(textBox18.Text);
                if (!_selectedCompany.IsGSTApplicable)
                    sgst = cgst = igst = 0;


                if (igst > 0)
                {
                    priceWithoutGST = price;// priceRound(price - (price * (igst / (100 + igst))), 2);
                }
                else
                    priceWithoutGST = price;//priceRound(price - (price * ((sgst + cgst) / (100 + sgst + cgst))), 2);
                dataGridView2.Rows[i].Cells[7].Value = priceWithoutGST;

                if (price > 0 && qnt > 0 && priceWithoutGST > 0)
                {
                    double rowTotal = 0;
                    if (dataGridView2.Rows[i].Cells[5].Value != null && !String.IsNullOrEmpty(dataGridView2.Rows[i].Cells[5].Value.ToString()) && Convert.ToDouble(dataGridView2.Rows[i].Cells[5].Value.ToString()) > 0)
                    {
                        qnt = Convert.ToDouble(dataGridView2.Rows[i].Cells[5].Value.ToString());
                        rowTotal = priceRound(Math.Round((priceWithoutGST * qnt), 2), 2);
                    }
                    else
                        rowTotal = Math.Round((priceWithoutGST * qnt), 2);

                    dataGridView2.Rows[i].Cells[8].Value = rowTotal;

                }
            }
        }
        private void doFinalCheck()
        {
            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                if (dataGridView2.Rows[i].Cells[6].Value != null && dataGridView2.Rows[i].Cells[6].Value != DBNull.Value)
                {
                    if (String.IsNullOrEmpty(dataGridView2.Rows[i].Cells[6].Value.ToString()) || String.IsNullOrEmpty(dataGridView2.Rows[i].Cells[4].Value.ToString()))
                    { dataGridView2.Rows.RemoveAt(i); break; }

                    if ((dataGridView2.Rows[i].Cells[7].Value != DBNull.Value) && tallyRound(Convert.ToDouble(dataGridView2.Rows[i].Cells[7].Value) * Convert.ToDouble(dataGridView2.Rows[i].Cells[4].Value) ,2)== Convert.ToDouble(dataGridView2.Rows[i].Cells[8].Value))
                    { }
                    else if ((dataGridView2.Rows[i].Cells[7].Value != DBNull.Value) && dataGridView2.Rows[i].Cells[5].Value !=null && (dataGridView2.Rows[i].Cells[5].Value != DBNull.Value) &&
                        priceRound(Math.Round(Convert.ToDouble(dataGridView2.Rows[i].Cells[7].Value) * 
                        (string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[5].Value.ToString())?0: Convert.ToDouble(dataGridView2.Rows[i].Cells[5].Value)), 2), 2) == Convert.ToDouble(dataGridView2.Rows[i].Cells[8].Value))
                    { } 
                    else
                    {
                        // MessageBox.Show("Error in Calculation previously, Now updated");
                        double price = dataGridView2.Rows[i].Cells[6].Value != null && !string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[6].Value.ToString()) ? Convert.ToDouble(dataGridView2.Rows[i].Cells[6].Value.ToString()) : 0;
                        double qnt = dataGridView2.Rows[i].Cells[4].Value != null && !string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[4].Value.ToString()) ? Convert.ToDouble(dataGridView2.Rows[i].Cells[4].Value.ToString()) : 0;
                        // var gst = dataGridView2.CurrentRow.Cells[6].Value;

                        double priceWithoutGST = 0;

                        double sgst = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToDouble(textBox16.Text);
                        double cgst = string.IsNullOrEmpty(textBox17.Text) ? 0 : Convert.ToDouble(textBox17.Text);
                        double igst = string.IsNullOrEmpty(textBox18.Text) ? 0 : Convert.ToDouble(textBox18.Text);
                        if (!_selectedCompany.IsGSTApplicable)
                            sgst = cgst = igst = 0;


                        if (igst > 0)
                        {
                            priceWithoutGST = price;// priceRound(price - (price * (igst / (100 + igst))), 2);
                        }
                        else
                            priceWithoutGST = price;// priceRound(price - (price * ((sgst + cgst) / (100 + sgst + cgst))), 2);
                        dataGridView2.Rows[i].Cells[7].Value = priceWithoutGST;

                        if (price > 0 && qnt > 0 && priceWithoutGST>0)
                        {
                            double rowTotal = 0;
                            if (dataGridView2.Rows[i].Cells[5].Value != null && !String.IsNullOrEmpty(dataGridView2.Rows[i].Cells[5].Value.ToString()) && Convert.ToDouble(dataGridView2.Rows[i].Cells[5].Value.ToString()) > 0)
                            {
                                qnt = Convert.ToDouble(dataGridView2.Rows[i].Cells[5].Value.ToString());
                                rowTotal = priceRound(Math.Round((priceWithoutGST * qnt), 2), 2);
                            }
                            else
                             rowTotal = Math.Round((priceWithoutGST * qnt), 2);

                            dataGridView2.Rows[i].Cells[8].Value = rowTotal;
                        }
                    }
                }
                else
                {

                    //   dataGridView2.Rows.RemoveAt(i);
                }
            }
            updateTotalBill();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isValidGrid())
            {
                MessageBox.Show("Please provide valid Product Details..!!");
                return;
            }
            doFinalCheck();

            

            InvoiceDetails inv = new InvoiceDetails();
            inv.CompanyId = _selectedCompany.CompanyID;
            inv.InvoiceId = textBox12.Text;
            inv.InvoiceDate = dateTimePicker1.Value;

            inv.StrBillTotal = textBox20.Text;
            inv.StrDiscount = textBox13.Text;
            inv.StrTotalAfterDiscountOrBeforeTax = textBox7.Text;
            inv.StrSGST = textBox8.Text;
            inv.StrCGST = textBox9.Text;
            inv.StrIGST = textBox11.Text;
            inv.StrTotalAfterTax = textBox14.Text;
            inv.StrRounded = textBox15.Text;
            inv.StrTotalPayable = textBox10.Text;
            if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() == "CASH BILL")
                inv.IsPaid = true;

            inv.CustomerId = Convert.ToInt32(comboBox2.SelectedValue);
            inv.CustomerName = textBox1.Text;
            inv.CustomerGST = textBox6.Text;
            inv.CustomerMobile = textBox3.Text;
            inv.CustomerAddress = textBox2.Text;
            inv.CustomerPanAadhaar = textBox4.Text;

            if (inv.CustomerId < 1)
            {
                string query = "insert into CustomerData " +
                "(CustomerName,CustomerType,Address,GSTIN,Aadhaar,PanNumber,MobilePhone1,MobilePhone2,OfficePhone1," +
                "FaxNo,WhatsappNo,CustomerNotes,AdditionField1,AdditionField2,AdditionField3,AdditionField4) Values " +
                 " ('" + inv.CustomerName.Replace("'", "") +
                 "','" + "-" +
                 "','" + inv.CustomerAddress.Replace("'", "") +
                 "','" + inv.CustomerGST.Replace("'", "") +
                 "','" + inv.CustomerPanAadhaar.Replace("'", "") +
                 "','" + "-" +
                 "','" + inv.CustomerMobile.Replace("'", "") +
                 "','" + "-" +//mobil2
                 "','" + "-" +//office1
                 "','" + "-" +//fax
                 "','" + "-" +//whatsapp
                 "','" + "-" +//Notes
                 "','" + "-" +//add1
                 "','" + "-" +//add2
                 "','" + "-" +//add3
                 "','" + "-" +//add4
                 "')";
                try { Functions.RunExecuteNonQuery(query); }
                catch
                {

                }
            }

            inv.StrTransport = textBox5.Text;
            inv.StrLRNo = textBox19.Text;
            inv.CGSTValue = textBox16.Text;
            inv.SGSTValue = textBox17.Text;
            inv.IGSTValue = textBox18.Text;

            try
            {
                inv.SaleId = SaveInvoiceTableData(inv);
            }
            catch
            {

            }

            if (inv.SaleId > 0)
            {
                UpdateInvoiceNoForCompany(inv.CompanyId, inv.InvoiceId);
            }
            else
            {
                return;
            }
            inv.Products = new List<InvoiceProducts>();
            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                InvoiceProducts currProduct = new InvoiceProducts();
                if (dataGridView2.Rows[i].Cells[8].Value != null)
                {
                    currProduct.SaleId = inv.SaleId;
                    currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == null ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                    currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == null ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                    currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == null ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                    currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == null ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                    currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == null ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                    currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == null ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                    currProduct.RateWithoutTax = dataGridView2.Rows[i].Cells[7].Value == null ? "" : dataGridView2.Rows[i].Cells[7].Value.ToString();
                    currProduct.Total = dataGridView2.Rows[i].Cells[8].Value == null ? "" : dataGridView2.Rows[i].Cells[8].Value.ToString();
                    currProduct.PreviousRate = dataGridView2.Rows[i].Cells[1].Value == null ? "" : dataGridView2.Rows[i].Cells[1].Value.ToString();

                    currProduct.Id = SaveInvoiceTableData(currProduct);
                    try
                    {
                        inv.Products.Add(currProduct);
                    }
                    catch
                    { }
                }
            }

            if (inv.SaleId > 0)
            {
                MessageBox.Show("BILL SAVED SUCCUSFULY");
                button1.Enabled = false;//Save
                button2.Enabled = false;//Save
                //button2.Enabled = true;//Print
                button5.Enabled = false;//Update
                button7.Enabled = false;//Update
                comboBox1.Enabled = false;
                textBox12.Enabled = false;

                new InvoicePrint(inv, _selectedCompany).Show();
                _savedInvoice = inv;

                isFormModified = false;
            }
        }

        private void UpdateInvoiceNoForCompany(int companyId, string billNo)
        {
            string query = "Update CompanyData Set BillNo = " + billNo + " where Id = " + companyId;
            Functions.RunExecuteNonQuery(query);
        }

        public int SaveInvoiceTableData(InvoiceProducts pro)
        {
            updatePriceMaster(pro.ProductName, pro.Rate, pro.HSNCode);
            

            string query = "insert into InvoiceProductDetails " +
                "(SaleId,ProductName,HSNCode,Rate,Quantity,Total,MtsDescription,Mts,isActive,CreatedOn,ModifiedOn,RateWithoutTax) Values " +
                 " (" + pro.SaleId +
                 ",'" + pro.ProductName.Replace("'", "") +
                 "','" + pro.HSNCode.Replace("'", "") +
                 "','" + pro.Rate +
                 "','" + pro.Quantity +
                 "','" + pro.Total +
                 "','" + pro.MtsDescription +
                 "','" + pro.Mts +
                 "','" + "1" +
                 "','" + DateTime.Now.ToString() +
                 "','" + DateTime.Now.ToString() +
                 "','" + pro.RateWithoutTax +
                 "')";


            pro.SaleId = Functions.RunExecuteScalarSql_getIdentity(query);

            if (pro.SaleId < 1)
            {
                MessageBox.Show("Error occured..!!");
            }

            return pro.Id;
        }

        public int SaveInvoiceTableData(InvoiceDetails inv)
        {

            bool valid = checkInvoiceId(inv.InvoiceId,inv.CompanyId);

            if (!valid)
            {
                MessageBox.Show("Given Invoice Id already exists in system. Please change Bill Number for company");
                return 0;
            }

            string query = "insert into SalesInvoiceDetail " +
                "(CompanyId     ,     InvoiceId     ,    InvoiceDate    ,StrBillTotal,    StrDiscount    , StrTotalAfterDiscountOrBeforeTax ,StrSGST      ,StrCGST      ,StrIGST      , StrTotalAfterTax  ,    StrRounded     ,  StrTotalPayable  ,IsPaid       ,    CustomerId     ,   CustomerName    ,    CustomerGST    ,  CustomerMobile   ,  CustomerAddress  , CustomerPanAadhaar ,   StrTransport    ,StrLRNo      ,     CGSTValue     ,     SGSTValue     ,     IGSTValue,CreatedOn,ModifiedOn ) values" +
                " ('" + inv.CompanyId +
                "','" + inv.InvoiceId +
                "','" + inv.InvoiceDate +
                "','" + inv.StrBillTotal +
                "','" + inv.StrDiscount +
                "','" + inv.StrTotalAfterDiscountOrBeforeTax +
                "','" + inv.StrSGST +
                "','" + inv.StrCGST +
                "','" + inv.StrIGST +
                "','" + inv.StrTotalAfterTax +
                "','" + inv.StrRounded +
                "','" + inv.StrTotalPayable +
                "','" + inv.IsPaid.ToString() +
                "','" + inv.CustomerId +
                "','" + inv.CustomerName.Replace("'", "") +
                "','" + inv.CustomerGST.Replace("'", "") +
                "','" + inv.CustomerMobile.Replace("'", "") +
                "','" + inv.CustomerAddress.Replace("'", "") +
                "','" + inv.CustomerPanAadhaar.Replace("'", "") +
                "','" + inv.StrTransport +
                "','" + inv.StrLRNo +
                "','" + inv.CGSTValue +
                "','" + inv.SGSTValue +
                "','" + inv.IGSTValue +
                "','" + DateTime.Now +
                "','" + DateTime.Now +
                "')";
            inv.SaleId = Functions.RunExecuteScalarSql_getIdentity(query);
            return inv.SaleId;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _isDiscountEditedManually = false;
            if (comboBox1.SelectedIndex < 1)
                return;

            string sql = "select * from CompanyData where ID = " + comboBox1.SelectedValue;
            var ds = Functions.RunSelectSql(sql);
            DataRow dr = ds.Tables[0].Rows[0];

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
            currentCompany.DefaultCashBill = Convert.ToBoolean(dr["DefaultCashBill"]);
            currentCompany.ThermalPrinter = Convert.ToBoolean(dr["ThermalPrinter"]);
            currentCompany.DefaultPrinter = dr["DefaultPrinter"].ToString();

            _selectedCompany = currentCompany;

            if (currentCompany.IsGSTApplicable == false)
            {
                textBox6.Visible = false;
                label10.Visible = false;
                textBox13.Visible = false;
                textBox7.Visible = false;
                textBox8.Visible = false;
                textBox9.Visible = false;
                textBox16.Visible = false;
                textBox17.Visible = false;
                textBox14.Visible = false;
                //inv.StrIGST;
                textBox14.Visible = false;
                textBox11.Visible = false;
                textBox18.Visible = false;

                label17.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label20.Visible = false;
                label13.Visible = false;
                label15.Visible = false;
                label21.Visible = false;
                label22.Visible = false;
                label18.Visible = false;

            }
            else
            {
                textBox13.Visible = false;
                textBox7.Visible = false;
                textBox8.Visible = true;
                textBox9.Visible = true;
                textBox16.Visible = true;
                textBox17.Visible = true;
                textBox14.Visible = false;
                textBox11.Visible = true;
                textBox18.Visible = true;
                //inv.StrIGST;
                textBox14.Visible = false;
                label17.Visible = false;
                label11.Visible = false;
                label12.Visible = true;
                label20.Visible = true;
                label13.Visible = true;
                label15.Visible = true;
                label21.Visible = true;
                label22.Visible = true;
                label18.Visible = false;

                textBox6.Visible = true;
                label10.Visible = true;
            }

            int currentInvoice = _selectedCompany.BillNo + 1;

            while(!checkInvoiceId(currentInvoice.ToString()))
            {
                currentInvoice = currentInvoice + 1;
            }

            if (_selectedCompany.DefaultCashBill)
                comboBox4.SelectedItem = "CASH BILL";
            else
                comboBox4.SelectedItem = "CREDIT BILL";

            if (_selectedCompany.ThermalPrinter)
            { button1.Visible = false; button5.Visible = false; }
            else
            { button1.Visible = true; button5.Visible = true; }
            

            textBox12.Text = currentInvoice.ToString();
            recomputeRateByCompany();
            doFinalCheck();
        }



        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex < 1)
                return;

            string sql = "select * from CompanyData where ID = " + comboBox3.SelectedValue;
            var ds = Functions.RunSelectSql(sql);
            DataRow dr = ds.Tables[0].Rows[0];

            CompanyDetails currentCompany = new CompanyDetails();
            currentCompany.CompanyID = Convert.ToInt32(dr["ID"]);
            
            currentCompany.BillNo = Convert.ToInt32(dr["BillNo"]);

            int currentInvoice = currentCompany.BillNo + 1;

            while (!checkInvoiceId(currentInvoice.ToString(), Convert.ToInt32(comboBox3.SelectedValue)))
            {
                currentInvoice = currentInvoice + 1;
            }

            textBox22.Text = currentInvoice.ToString();
        }





        public AutoCompleteStringCollection AutoCompleteLoad()
        {
            return productList;
        }
        public AutoCompleteStringCollection AutoCompleteHsnLoad()
        {
            return hsnList;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            isFormModified = false;
            isFirstSearchClick = true;

            int billNo = 0;
            int billYear = 0;

            if (!int.TryParse(textBox12.Text, out billNo))
            { 
                if(textBox12.Text.Contains("-"))
                {
                    if (int.TryParse(textBox12.Text.Split('-')[0], out billNo))
                    {
                        if (int.TryParse(textBox12.Text.Split('-')[1], out billYear))
                        {
                             searchInv1 = getInvoiceDetails(_selectedCompany.CompanyID, billNo.ToString(),false, billYear.ToString());
                        }
                        return;
                    }
                    return;
                }
                return;
            }



            if (isFormModified)
            {
                ShowMsgToSave();
                return;
            }
            if (_selectedCompany == null || _selectedCompany.CompanyID < 1 || textBox12.Text.Length < 1)
            {
                MessageBox.Show("Please select Company and correct bill no to search");
                return;
            }

            InvoiceDetails searchInv = getInvoiceDetails(_selectedCompany.CompanyID, textBox12.Text.ToString());

            comboBox1.Enabled = false;
            textBox12.Enabled = false;
            button1.Enabled = false;//Save
            button2.Enabled = false;//Save

        }

        private InvoiceDetails getInvoiceDetails(int companyId, string billNo,bool lazyLoad = false,string billYear = "")
        {
            string invoiceQuery = "Select * from SalesInvoiceDetail where CompanyId = " + companyId + " and InvoiceId= '" + billNo + "'";
            
            DataSet ds = null;
            if (string.IsNullOrEmpty(billYear))
                ds = Functions.RunSelectSql(invoiceQuery);
            else
                ds = Functions.RunSelectSql(invoiceQuery, Functions.GetYearConnectionString(billYear));

            if ((ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1) && lazyLoad)
            {
                return null;
            }
            else if (ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1)
            {
                MessageBox.Show("Invoice Not Found");
                button5.Enabled = false;//Update
                button7.Enabled = false;//Update
                return null;
            }

            DataRow dr = ds.Tables[0].Rows[0];
            InvoiceDetails inv = new InvoiceDetails();
            inv.CompanyId = Convert.ToInt32(dr["CompanyId"]);
            inv.SaleId = publicSaleId = Convert.ToInt32(dr["SaleId"]);
            
            
            if (lazyLoad)
            {
                inv.InvoiceId = dr["InvoiceId"].ToString();
                return inv;
            
            }

            if (Convert.ToDateTime(dr["InvoiceDate"]) < new DateTime(2021, 08, 01))
            {
                if (Convert.ToInt32(dr["InvoiceId"].ToString()) > 0 && inv.CompanyId > 0 )
                {
                    new AddInvoiceOld(Convert.ToInt32(dr["InvoiceId"].ToString()), inv.CompanyId).Show();
                }

                return inv;
            }
            inv.InvoiceDate = dateTimePicker1.Value = Convert.ToDateTime(dr["InvoiceDate"]);
            inv.InvoiceId = textBox12.Text = dr["InvoiceId"].ToString();
            button5.Enabled = true;//Update
            button7.Enabled = true;//Update

            inv.IGSTValue = textBox18.Text = dr["IGSTValue"] != null ? dr["IGSTValue"].ToString() : "";//FOR iGST BILL IS MODIFIED FOR TEXT CHANGE IN SEARCH

            inv.StrBillTotal = textBox20.Text = dr["StrBillTotal"].ToString();
            inv.StrDiscount = textBox13.Text = dr["StrDiscount"].ToString();
            inv.StrTotalAfterDiscountOrBeforeTax = textBox7.Text = dr["StrTotalAfterDiscountOrBeforeTax"].ToString();
            inv.StrSGST = textBox8.Text = dr["StrSGST"] != null ? dr["StrSGST"].ToString() : "";
            inv.StrCGST = textBox9.Text = dr["StrCGST"] != null ? dr["StrCGST"].ToString() : "";
            inv.StrIGST = textBox11.Text = dr["StrIGST"] != null ? dr["StrIGST"].ToString() : "";
            inv.StrTotalAfterTax = textBox14.Text = dr["StrTotalAfterTax"].ToString();
            inv.StrRounded = textBox15.Text = dr["StrRounded"].ToString();
            inv.StrTotalPayable = textBox10.Text = dr["StrTotalPayable"].ToString();

            inv.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
            if (inv.IsPaid)
                comboBox4.SelectedItem = "CASH BILL";
            else
                comboBox4.SelectedItem = "CREDIT BILL";

            //if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() == "CASH BILL")
            //    inv.IsPaid = true;

            inv.CustomerId = Convert.ToInt32(dr["CustomerId"]);
            inv.CustomerName = textBox1.Text = dr["CustomerName"].ToString();
            inv.CustomerGST = textBox6.Text = dr["CustomerGST"].ToString();
            inv.CustomerMobile = textBox3.Text = dr["CustomerMobile"].ToString();
            inv.CustomerAddress = textBox2.Text = dr["CustomerAddress"].ToString();
            inv.CustomerPanAadhaar = textBox4.Text = dr["CustomerPanAadhaar"].ToString();

            inv.StrTransport = textBox5.Text = dr["StrTransport"] != null ? dr["StrTransport"].ToString() : "";
            inv.StrLRNo = textBox19.Text = dr["StrLRNo"] != null ? dr["StrLRNo"].ToString() : "";
            inv.CGSTValue = textBox16.Text = dr["CGSTValue"] != null ? dr["CGSTValue"].ToString() : "";
            inv.SGSTValue = textBox17.Text = dr["SGSTValue"] != null ? dr["SGSTValue"].ToString() : "";
            

            string prodSql = "Select * from InvoiceProductDetails where SaleId = " + inv.SaleId;
            
            DataSet prod=null;
            if (string.IsNullOrEmpty(billYear))
                prod = Functions.RunSelectSql(prodSql);
            else
                prod = Functions.RunSelectSql(prodSql, Functions.GetYearConnectionString(billYear));

            inv.Products = new List<InvoiceProducts>();
            dataGridView2.DataSource = prod.Tables[0];
            foreach (DataRow currRow in prod.Tables[0].Rows)
            {
                InvoiceProducts currProduct = new InvoiceProducts();
                currProduct.Id = Convert.ToInt32(currRow["Id"]);
                currProduct.SaleId = inv.SaleId;
                currProduct.ProductName = currRow["ProductName"].ToString();
                currProduct.MtsDescription = currRow["MtsDescription"].ToString();
                currProduct.HSNCode = currRow["HSNCode"].ToString();
                currProduct.Quantity = currRow["Quantity"].ToString();
                currProduct.Mts = currRow["Mts"].ToString();
                currProduct.Rate = currRow["Rate"].ToString();
                currProduct.RateWithoutTax = currRow["RateWithoutTax"].ToString();
                currProduct.Total = currRow["Total"].ToString();

                inv.Products.Add(currProduct);
            }
            return inv;
        }

        private void grd_bill_master_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int column = dataGridView2.CurrentCell.ColumnIndex;

            if (column == 0 && ConfigurationManager.AppSettings["isAutoCompleteEnabled"] == "true")
            {
                TextBox tb = e.Control as TextBox;
                tb.CharacterCasing = CharacterCasing.Upper;
                if (tb != null)
                {
                    tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    tb.AutoCompleteCustomSource = AutoCompleteLoad();
                    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                return;
            }
            else if (column == 3)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    tb.AutoCompleteCustomSource = AutoCompleteHsnLoad();
                    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                return;
            }
            else
            {
                productList.Clear();
            }


            if (column == 0 || column == 3)
            {
                return;
            }
            if (column == 2)
            {
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPressForMts);
                return;
            }
            try
            {
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPressForNumber);
            }
            finally
            { }
        }

        private void Control_KeyPressForMts(object sender, KeyPressEventArgs e)
        {
            int column = dataGridView2.CurrentCell.ColumnIndex;
            if (column != 2)
            {
                return;
            }


            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '+')
                e.Handled = false;

        }

        private void Control_KeyPressForNumber(object sender, KeyPressEventArgs e)
        {
            int column = dataGridView2.CurrentCell.ColumnIndex;
            if (column == 0 || column == 3 || column == 2)
            {
                return;
            }

            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.'
                && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;

            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (isFormModified)
            {
                ShowMsgToSave();
                return;
            }
            this.Hide();
            var form2 = new AddInvoice();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox18.Text))
                return;

            textBox17.Text = "";
            textBox16.Text = "";

            updateTotalBill();

            SetTextChanged(sender, e);
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            comboBox2.DroppedDown = false;
            return;
            string name = string.Format("{0}{1}", comboBox2.Text, e.KeyChar.ToString()); //join previous text and new pressed char

            if (name.Contains("Select"))
                return;

            string sql = "select * from customerdata where customerName like '%" + name + "%'";
            var ds = Functions.RunSelectSql(sql);
            SortedDictionary<int, string> userCache = new SortedDictionary<int, string>();
            // userCache.Add(0, "--Select--");

            //ds.Tables[0].DefaultView.Sort = "CustomerName ASC";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                userCache.Add(Convert.ToInt32(dr["ID"]), dr["CustomerName"].ToString() + " - " + dr["Address"].ToString());

            }
            comboBox2.DataSource = null;
            comboBox2.DataSource = new BindingSource(userCache, null);
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";

            //MessageBox.Show(name);

            //comboBox1.DataSource = null;
            //comboBox1.DataSource = filteredTable.DefaultView;
            //comboBox1.DisplayMember = "FieldName";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!isValidGrid())
            {
                MessageBox.Show("Please provide valid Product Details..!!");
                return;
            }
            doFinalCheck();

            InvoiceDetails inv = new InvoiceDetails();
            inv.CompanyId = _selectedCompany.CompanyID;
            inv.InvoiceId = textBox12.Text;
            inv.InvoiceDate = dateTimePicker1.Value;

            inv.StrBillTotal = textBox20.Text;
            inv.StrDiscount = textBox13.Text;
            inv.StrTotalAfterDiscountOrBeforeTax = textBox7.Text;
            inv.StrSGST = textBox8.Text;
            inv.StrCGST = textBox9.Text;
            inv.StrIGST = textBox11.Text;
            inv.StrTotalAfterTax = textBox14.Text;
            inv.StrRounded = textBox15.Text;
            inv.StrTotalPayable = textBox10.Text;
            if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() == "CASH BILL")
                inv.IsPaid = true;

            inv.CustomerId = Convert.ToInt32(comboBox2.SelectedValue);
            inv.CustomerName = textBox1.Text;
            inv.CustomerGST = textBox6.Text;
            inv.CustomerMobile = textBox3.Text;
            inv.CustomerAddress = textBox2.Text;
            inv.CustomerPanAadhaar = textBox4.Text;

            inv.StrTransport = textBox5.Text;
            inv.StrLRNo = textBox19.Text;
            inv.CGSTValue = textBox16.Text;
            inv.SGSTValue = textBox17.Text;
            inv.IGSTValue = textBox18.Text;
            int newSaleId = 0;
            try
            {
                newSaleId = UpdateInvoiceToTable(inv);
            }
            catch
            {

            }
            if (newSaleId < 1)
                return;

            Functions.RunExecuteNonQuery("update InvoiceProductDetails set isActive='0'" +
                        ", ModifiedOn = '" + DateTime.Now + "'" +
                        "where SaleId = " + publicSaleId);

            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                InvoiceProducts currProduct = new InvoiceProducts();
                if (dataGridView2.Rows[i].Cells[8].Value != null)
                {
                    //currProduct.SaleId = inv.SaleId;
                    currProduct.Id = dataGridView2.Rows[i].Cells[10].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[10].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[10].Value.ToString()) ? 0 : Convert.ToInt32(dataGridView2.Rows[i].Cells[10].Value);
                    currProduct.SaleId = newSaleId;
                    currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[0].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[0].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                    currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[2].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[2].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                    currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[3].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[3].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                    currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[4].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[4].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                    currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[5].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[5].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                    currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[6].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[6].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                    currProduct.RateWithoutTax = dataGridView2.Rows[i].Cells[7].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[7].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[7].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[7].Value.ToString();
                    currProduct.Total = dataGridView2.Rows[i].Cells[8].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[8].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[8].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[8].Value.ToString();

                    if (publicSaleId < 1)
                    {
                        MessageBox.Show("Error in Updating, Please keep hard copy of Bill safe");
                    }
                    currProduct.Id = SaveInvoiceTableData(currProduct);
                    //UpdateInvoiceTableData(currProduct);

                }
            }

            inv.Products = new List<InvoiceProducts>();
            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                InvoiceProducts currProduct = new InvoiceProducts();
                if (dataGridView2.Rows[i].Cells[8].Value != null)
                {
                    currProduct.SaleId = inv.SaleId;
                    currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == null ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                    currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == null ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                    currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == null ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                    currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == null ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                    currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == null ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                    currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == null ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                    currProduct.RateWithoutTax = dataGridView2.Rows[i].Cells[7].Value == null ? "" : dataGridView2.Rows[i].Cells[7].Value.ToString();
                    currProduct.Total = dataGridView2.Rows[i].Cells[8].Value == null ? "" : dataGridView2.Rows[i].Cells[8].Value.ToString();
                    currProduct.PreviousRate= dataGridView2.Rows[i].Cells[1].Value == null ? "" : dataGridView2.Rows[i].Cells[1].Value.ToString();

                    inv.Products.Add(currProduct);

                }
            }
            button5.Enabled = false;
            button7.Enabled = false;
            MessageBox.Show("Bill details updated successfully.");
            isFormModified = false;

            new InvoicePrint(inv, _selectedCompany).Show();
            _savedInvoice = inv;
        }

        
        private void UpdateInvoiceTableData(InvoiceProducts currProduct)
        {
            if (currProduct.Id < 1)
            {
                SaveInvoiceTableData(currProduct);
                return;
            }

            string query = "update InvoiceProductDetails set " +
                "ProductName = '" + currProduct.ProductName.Replace("'","") + "'" +
                ", HSNCode = '" + currProduct.HSNCode.Replace("'", "") + "'" +
                 ", Rate = '" + currProduct.Rate + "'" +
                 ", RateWithoutTax = '" + currProduct.RateWithoutTax + "'" +
                 ", Quantity = '" + currProduct.Quantity + "'" +
                 ", Total = '" + currProduct.Total + "'" +
                 ", MtsDescription = '" + currProduct.MtsDescription + "'" +
                 ", Mts = '" + currProduct.Mts + "'" +
                 " where Id = " + currProduct.Id;


            if (publicSaleId < 0)
            {
                MessageBox.Show("Error in Updating, Please keep hard copy of Bill safe");
            }

            Functions.RunExecuteNonQuery(query);
        }

        private int UpdateInvoiceToTable(InvoiceDetails inv)
        {
            string query = "update SalesInvoiceDetail set " +
                //"InvoiceDate = '" + inv.InvoiceDate +"'"+
                //", StrBillTotal = '" + inv.StrBillTotal + "'" +
                //", StrDiscount = '" + inv.StrDiscount + "'" +
                //", StrTotalAfterDiscountOrBeforeTax = '" + inv.StrTotalAfterDiscountOrBeforeTax + "'" +
                //", StrSGST = '" + inv.StrSGST + "'" +
                //", StrCGST = '" + inv.StrCGST + "'" +
                //", StrIGST = '" + inv.StrIGST + "'" +
                //", StrTotalAfterTax = '" + inv.StrTotalAfterTax + "'" +
                //", StrRounded = '" + inv.StrRounded + "'" +
                //", StrTotalPayable = '" + inv.StrTotalPayable + "'" +
                //", IsPaid = '" + inv.IsPaid.ToString() + "'" +
                //", CustomerId = '" + inv.CustomerId + "'" +
                //", CustomerName = '" + inv.CustomerName + "'" +
                //", CustomerGST = '" + inv.CustomerGST + "'" +
                //", CustomerMobile = '" + inv.CustomerMobile + "'" +
                //", CustomerAddress = '" + inv.CustomerAddress + "'" +
                //", CustomerPanAadhaar = '" + inv.CustomerPanAadhaar + "'" +
                //", StrTransport = '" + inv.StrTransport + "'" +
                //", StrLRNo = '" + inv.StrLRNo + "'" +
                //", CGSTValue = '" + inv.CGSTValue + "'" +
                //", SGSTValue = '" + inv.SGSTValue + "'" +
                //", IGSTValue = '" + inv.IGSTValue + "'" +
                "InvoiceId = '" + inv.InvoiceId + "-Backup-" + DateTime.Now.ToString("ddMMM_HH_mm_tt") + "'" +
                ", ModifiedOn = '" + DateTime.Now + "'" +

                " where CompanyId = " + inv.CompanyId + " and InvoiceId = '" + inv.InvoiceId + "'";
            //"(CompanyId     ,     InvoiceId     ,    InvoiceDate    ,    StrDiscount    , StrTotalAfterDiscountOrBeforeTax 
            //,StrSGST      ,StrCGST      ,StrIGST      , StrTotalAfterTax  ,    StrRounded     ,  StrTotalPayable  ,IsPaid       ,    
            //CustomerId     ,   CustomerName    ,    CustomerGST    ,  CustomerMobile   ,  CustomerAddress  , CustomerPanAadhaar ,   
            //StrTransport    ,StrLRNo      ,     CGSTValue     ,     SGSTValue     ,     IGSTValue,CreatedOn,ModifiedOn ) values" +

            Functions.RunExecuteNonQuery(query);

            inv.SaleId = 0;
            inv.SaleId = SaveInvoiceTableData(inv);

            return inv.SaleId;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            LoadCompanyData();
        }

        private double tallyRound(double value, int decimals)
        { 
            return 
                Convert.ToDouble(Math.Round(Convert.ToDecimal(value), decimals, MidpointRounding.AwayFromZero));
        }

        private double priceRound(double value,int decimals)
        {
            if (value == 0)
                return 0;

            double trucateVal= Math.Truncate(100 * value) / 100;//Math.Round(value * 20) / 20;
            string trucateValStr = String.Format("{0:0.00}", (trucateVal));
            string lastDigit= trucateValStr.Substring(trucateValStr.Length - 1);
            
            switch (lastDigit)
            {
                
                case "1":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "0");
                    break;
                case "2":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "5");
                    break;

                case "3":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "5");
                    break;

                case "4":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "5");
                    break;

                case "5":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "5");
                    break;
                case "6":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "5");
                    break;

                case "7":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "0") + 0.10;
                    break;
                
                case "8":
                    trucateVal =  Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "0")+0.10;
                    break;
                case "9":
                    trucateVal = Convert.ToDouble(trucateValStr.Remove(trucateValStr.Length - 1, 1) + "0") + 0.10;
                    break;


                default:
                    break;

            }


                return tallyRound(trucateVal,2);
            }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            SetTextChanged(sender, e);
        }
        private void dgvUserDetails_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView2.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            double result = 0;
            bool converted = double.TryParse(textBox15.Text, out result);
            if (!converted)
                return;
            double grandTotal = Convert.ToDouble(textBox15.Text);

            textBox10.Text = String.Format("{0:n}", grandTotal);
        }

        private bool checkInvoiceId(string invoiceId,int companyId = 0)
        {
            string invoiceQuery = "";
            if (companyId > 0)
                invoiceQuery = "Select * from SalesInvoiceDetail where CompanyId = " + companyId + " and InvoiceId= '" + invoiceId + "'";
            else

                invoiceQuery = "Select * from SalesInvoiceDetail where CompanyId = " + _selectedCompany.CompanyID + " and InvoiceId= '" + invoiceId + "'";
            DataSet ds = Functions.RunSelectSql(invoiceQuery);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return false;
            }
            else
                return true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!isValidGrid())
            {
                MessageBox.Show("Please provide valid Product Details..!!");
                return;
            }
            doFinalCheck();

            bool valid = checkInvoiceId(textBox12.Text);

            if (!valid)
            {
                MessageBox.Show("Given Invoice Id already exists in system. Please change Bill Number for company");
               // return;
            }

            InvoiceDetails inv = new InvoiceDetails();
            inv.CompanyId = _selectedCompany.CompanyID;
            inv.InvoiceId = textBox12.Text;
            inv.InvoiceDate = dateTimePicker1.Value;

            inv.StrBillTotal = textBox20.Text;
            inv.StrDiscount = textBox13.Text;
            inv.StrTotalAfterDiscountOrBeforeTax = textBox7.Text;
            inv.StrSGST = textBox8.Text;
            inv.StrCGST = textBox9.Text;
            inv.StrIGST = textBox11.Text;
            inv.StrTotalAfterTax = textBox14.Text;
            inv.StrRounded = textBox15.Text;
            inv.StrTotalPayable = textBox10.Text;
            if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() == "CASH BILL")
                inv.IsPaid = true;

            inv.CustomerId = Convert.ToInt32(comboBox2.SelectedValue);
            inv.CustomerName = textBox1.Text;
            inv.CustomerGST = textBox6.Text;
            inv.CustomerMobile = textBox3.Text;
            inv.CustomerAddress = textBox2.Text;
            inv.CustomerPanAadhaar = textBox4.Text;

            if (inv.CustomerId < 1)
            {
                string query = "insert into CustomerData " +
                "(CustomerName,CustomerType,Address,GSTIN,Aadhaar,PanNumber,MobilePhone1,MobilePhone2,OfficePhone1," +
                "FaxNo,WhatsappNo,CustomerNotes,AdditionField1,AdditionField2,AdditionField3,AdditionField4) Values " +
                 " ('" + inv.CustomerName.Replace("'", "") +
                 "','" + "-" +
                 "','" + inv.CustomerAddress.Replace("'", "") +
                 "','" + inv.CustomerGST.Replace("'", "") +
                 "','" + inv.CustomerPanAadhaar.Replace("'", "") +
                 "','" + "-" +
                 "','" + inv.CustomerMobile.Replace("'", "") +
                 "','" + "-" +//mobil2
                 "','" + "-" +//office1
                 "','" + "-" +//fax
                 "','" + "-" +//whatsapp
                 "','" + "-" +//Notes
                 "','" + "-" +//add1
                 "','" + "-" +//add2
                 "','" + "-" +//add3
                 "','" + "-" +//add4
                 "')";
                try { Functions.RunExecuteNonQuery(query); }
                catch
                {

                }
            }

            inv.StrTransport = textBox5.Text;
            inv.StrLRNo = textBox19.Text;
            inv.CGSTValue = textBox16.Text;
            inv.SGSTValue = textBox17.Text;
            inv.IGSTValue = textBox18.Text;

            try
            {
                inv.SaleId = SaveInvoiceTableData(inv);
            }
            catch
            {

            }

            if (inv.SaleId > 0)
            {
                UpdateInvoiceNoForCompany(inv.CompanyId, inv.InvoiceId);
            }
            else
                return;
            inv.Products = new List<InvoiceProducts>();
            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                InvoiceProducts currProduct = new InvoiceProducts();
                if (dataGridView2.Rows[i].Cells[8].Value != null)
                {
                    currProduct.SaleId = inv.SaleId;
                    currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == null ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                    currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == null ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                    currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == null ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                    currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == null ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                    currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == null ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                    currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == null ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                    currProduct.RateWithoutTax = dataGridView2.Rows[i].Cells[7].Value == null ? "" : dataGridView2.Rows[i].Cells[7].Value.ToString();
                    currProduct.Total = dataGridView2.Rows[i].Cells[8].Value == null ? "" : dataGridView2.Rows[i].Cells[8].Value.ToString();

                    currProduct.Id = SaveInvoiceTableData(currProduct);
                    try
                    {
                        inv.Products.Add(currProduct);
                    }
                    catch
                    { }
                }
            }

            if (inv.SaleId > 0)
            {
                MessageBox.Show("BILL SAVED SUCCUSFULY");
                button1.Enabled = false;//Save
                button2.Enabled = false;//Save
                button5.Enabled = false;//Update
                button7.Enabled = false;//Update
                comboBox1.Enabled = false;
                textBox12.Enabled = false;



                new InvoicePrint(inv, _selectedCompany, true);
                _savedInvoice = inv;
                isFormModified = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!isValidGrid())
            {
                MessageBox.Show("Please provide valid Product Details..!!");
                return;
            }
            doFinalCheck();

            InvoiceDetails inv = new InvoiceDetails();
            inv.CompanyId = _selectedCompany.CompanyID;
            inv.InvoiceId = textBox12.Text;
            inv.InvoiceDate = dateTimePicker1.Value;

            inv.StrBillTotal = textBox20.Text;
            inv.StrDiscount = textBox13.Text;
            inv.StrTotalAfterDiscountOrBeforeTax = textBox7.Text;
            inv.StrSGST = textBox8.Text;
            inv.StrCGST = textBox9.Text;
            inv.StrIGST = textBox11.Text;
            inv.StrTotalAfterTax = textBox14.Text;
            inv.StrRounded = textBox15.Text;
            inv.StrTotalPayable = textBox10.Text;
            if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() == "CASH BILL")
                inv.IsPaid = true;

            inv.CustomerId = Convert.ToInt32(comboBox2.SelectedValue);
            inv.CustomerName = textBox1.Text;
            inv.CustomerGST = textBox6.Text;
            inv.CustomerMobile = textBox3.Text;
            inv.CustomerAddress = textBox2.Text;
            inv.CustomerPanAadhaar = textBox4.Text;

            inv.StrTransport = textBox5.Text;
            inv.StrLRNo = textBox19.Text;
            inv.CGSTValue = textBox16.Text;
            inv.SGSTValue = textBox17.Text;
            inv.IGSTValue = textBox18.Text;
            int newSaleId = 0;
            try
            {
                newSaleId = UpdateInvoiceToTable(inv);
            }
            catch
            {

            }
            if (newSaleId < 1)
                return;

            Functions.RunExecuteNonQuery("update InvoiceProductDetails set isActive='0'" +
                        ", ModifiedOn = '" + DateTime.Now + "'" +
                        "where SaleId = " + publicSaleId);

            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                InvoiceProducts currProduct = new InvoiceProducts();
                if (dataGridView2.Rows[i].Cells[8].Value != null)
                {
                    //currProduct.SaleId = inv.SaleId;
                    currProduct.Id = dataGridView2.Rows[i].Cells[10].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[10].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[10].Value.ToString()) ? 0 : Convert.ToInt32(dataGridView2.Rows[i].Cells[10].Value);
                    currProduct.SaleId = newSaleId;
                    currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[0].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[0].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                    currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[2].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[2].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                    currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[3].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[3].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                    currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[4].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[4].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                    currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[5].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[5].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                    currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[6].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[6].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                    currProduct.RateWithoutTax = dataGridView2.Rows[i].Cells[7].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[7].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[6].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                    currProduct.Total = dataGridView2.Rows[i].Cells[8].Value == System.DBNull.Value || dataGridView2.Rows[i].Cells[8].Value == null || string.IsNullOrEmpty(dataGridView2.Rows[i].Cells[8].Value.ToString()) ? "" : dataGridView2.Rows[i].Cells[8].Value.ToString();

                    if (publicSaleId < 1)
                    {
                        MessageBox.Show("Error in Updating, Please keep hard copy of Bill safe");
                    }
                    currProduct.Id = SaveInvoiceTableData(currProduct);
                    //UpdateInvoiceTableData(currProduct);

                }
            }

            inv.Products = new List<InvoiceProducts>();
            for (int i = 0; i < dataGridView2.Rows.Count; ++i)
            {
                InvoiceProducts currProduct = new InvoiceProducts();
                if (dataGridView2.Rows[i].Cells[8].Value != null)
                {
                    currProduct.SaleId = inv.SaleId;
                    currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == null ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                    currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == null ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                    currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == null ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                    currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == null ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                    currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == null ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                    currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == null ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                    currProduct.RateWithoutTax = dataGridView2.Rows[i].Cells[7].Value == null ? "" : dataGridView2.Rows[i].Cells[7].Value.ToString();
                    currProduct.Total = dataGridView2.Rows[i].Cells[8].Value == null ? "" : dataGridView2.Rows[i].Cells[8].Value.ToString();
                    currProduct.PreviousRate = dataGridView2.Rows[i].Cells[1].Value == null ? "" : dataGridView2.Rows[i].Cells[1].Value.ToString();

                    inv.Products.Add(currProduct);

                }
            }
            button5.Enabled = false;
            button7.Enabled = false;
            MessageBox.Show("Bill details updated successfully.");
            isFormModified = false;
            new InvoicePrint(inv, _selectedCompany, true);
            _savedInvoice = inv;
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter && notlastColumn) //if not last column move to nex
            //{
            //    SendKeys.Send("{Up}");
            //    SendKeys.Send("{Right}");
            //}
            //else if (e.KeyCode == Keys.Enter)
            //{
            //    SendKeys.Send("{Home}");//go to first column
            //    notlastColumn = true;
            //}
            //e.SuppressKeyPress = true;
            //int iColumn = dataGridView2.CurrentCell.ColumnIndex;
            //int iRow = dataGridView2.CurrentCell.RowIndex;
            //if (iColumn == dataGridView2.ColumnCount - 1)
            //{
            //    if (dataGridView2.RowCount > (iRow + 1))
            //    {
            //        dataGridView2.CurrentCell = dataGridView2[1, iRow + 1];
            //    }
            //    else
            //    {
            //        //focus next control
            //    }
            //}
            //else
            //    dataGridView2.CurrentCell = dataGridView2[iColumn + 1, iRow];
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int companyId = Convert.ToInt32(comboBox3.SelectedValue);
            string billNo = textBox22.Text;

            bool proceedCopy = false;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[12];
                if (chk.Value != null && chk.Value.ToString() == "True" && dataGridView2.Rows[i].Cells[6].Value != null && dataGridView2.Rows[i].Cells[6].Value.ToString() != "")
                {
                    proceedCopy = true;
                }
            }

            if (!proceedCopy)
            {
                MessageBox.Show("Please select products to copy");
                return;
 
            }

            InvoiceDetails searchInv = getInvoiceDetails(companyId, billNo,true);



            if (searchInv == null)
            {
                if (!isValidGrid())
                {
                    MessageBox.Show("Please provide valid Product Details..!!");
                    return;
                }
                //doFinalCheck();

                bool valid = checkInvoiceId(textBox22.Text,Convert.ToInt32(comboBox3.SelectedValue));

                if (!valid)
                {
                    MessageBox.Show("Invoice cannot be copied");
                    return;
                }

                InvoiceDetails inv = new InvoiceDetails();
                inv.CompanyId = Convert.ToInt32(comboBox3.SelectedValue);
                inv.InvoiceId = textBox22.Text;
                inv.InvoiceDate = dateTimePicker1.Value;

                
                if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() == "CASH BILL")
                    inv.IsPaid = true;

                inv.CustomerId = Convert.ToInt32(comboBox2.SelectedValue);
                inv.CustomerName = textBox1.Text;
                inv.CustomerGST = textBox6.Text;
                inv.CustomerMobile = textBox3.Text;
                inv.CustomerAddress = textBox2.Text;
                inv.CustomerPanAadhaar = textBox4.Text;

                
                inv.StrTransport = textBox5.Text;
                inv.StrLRNo = textBox19.Text;

                inv.CGSTValue = textBox16.Text;
                inv.SGSTValue = textBox17.Text;
                inv.IGSTValue = textBox18.Text;

                try
                {
                    inv.SaleId = SaveInvoiceTableData(inv);
                }
                catch
                {

                }

                if (inv.SaleId > 0)
                {
                    UpdateInvoiceNoForCompany(inv.CompanyId, inv.InvoiceId);
                }
                else
                    return;
                inv.Products = new List<InvoiceProducts>();
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    InvoiceProducts currProduct = new InvoiceProducts();
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[12];
                    if (chk.Value != null && chk.Value.ToString() == "True" && dataGridView2.Rows[i].Cells[6].Value != null && dataGridView2.Rows[i].Cells[6].Value.ToString() != "")
                    {
                        currProduct.SaleId = inv.SaleId;
                        currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == null ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                        currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == null ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                        currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == null ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                        currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == null ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                        currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == null ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                        currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == null ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();

                        double price = Convert.ToDouble(currProduct.Rate);
                        double qnt = Convert.ToDouble(currProduct.Quantity);

                        double priceWithoutGST = 0;

                        double sgst = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToDouble(textBox16.Text);
                        double cgst = string.IsNullOrEmpty(textBox17.Text) ? 0 : Convert.ToDouble(textBox17.Text);
                        double igst = string.IsNullOrEmpty(textBox18.Text) ? 0 : Convert.ToDouble(textBox18.Text);
                        if (isGSTApplicableForCompany(inv.CompanyId))
                        {

                            if (!textBox6.Text.StartsWith("37")) igst = _defaultTax;
                            else sgst = cgst = _defaultTax/2;
                        }
                        else
                            sgst = cgst = igst = 0;


                        if (igst > 0)
                        {
                            priceWithoutGST = price; //priceRound(price - (price * (igst / (100 + igst))), 2);

                            //sgst = igst / 2;
                            //cgst = igst / 2;
                        }
                        else
                            priceWithoutGST = price; //priceRound(price - (price * ((sgst + cgst) / (100 + sgst + cgst))), 2);

                        currProduct.RateWithoutTax = priceWithoutGST.ToString();


                        if (price > 0 && qnt > 0 && priceWithoutGST > 0)
                        {
                            double rowTotal = 0;
                            if ( !String.IsNullOrEmpty(currProduct.Mts) && Convert.ToDouble(currProduct.Mts) > 0)
                            {
                                qnt = Convert.ToDouble(currProduct.Mts);
                                rowTotal = priceRound(Math.Round((priceWithoutGST * qnt), 2), 2);
                            }
                            else
                                rowTotal = Math.Round((priceWithoutGST * qnt), 2);

                            currProduct.Total = rowTotal.ToString();
                        }
                        currProduct.Id = SaveInvoiceTableData(currProduct);

                    }

                }
 


            }

            else if (searchInv != null && searchInv.SaleId > 0)
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    InvoiceProducts currProduct = new InvoiceProducts();
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[12];
                    if (chk.Value !=null &&  chk.Value.ToString() == "True" && dataGridView2.Rows[i].Cells[6].Value != null && dataGridView2.Rows[i].Cells[6].Value.ToString() != "")
                    {
                        currProduct.SaleId = searchInv.SaleId;
                        currProduct.ProductName = dataGridView2.Rows[i].Cells[0].Value == null ? "" : dataGridView2.Rows[i].Cells[0].Value.ToString();
                        currProduct.MtsDescription = dataGridView2.Rows[i].Cells[2].Value == null ? "" : dataGridView2.Rows[i].Cells[2].Value.ToString();
                        currProduct.HSNCode = dataGridView2.Rows[i].Cells[3].Value == null ? "" : dataGridView2.Rows[i].Cells[3].Value.ToString();
                        currProduct.Quantity = dataGridView2.Rows[i].Cells[4].Value == null ? "" : dataGridView2.Rows[i].Cells[4].Value.ToString();
                        currProduct.Mts = dataGridView2.Rows[i].Cells[5].Value == null ? "" : dataGridView2.Rows[i].Cells[5].Value.ToString();
                        currProduct.Rate = dataGridView2.Rows[i].Cells[6].Value == null ? "" : dataGridView2.Rows[i].Cells[6].Value.ToString();
                        double price = Convert.ToDouble(currProduct.Rate);
                        double qnt = Convert.ToDouble(currProduct.Quantity);

                        double priceWithoutGST = 0;

                        double sgst = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToDouble(textBox16.Text);
                        double cgst = string.IsNullOrEmpty(textBox17.Text) ? 0 : Convert.ToDouble(textBox17.Text);
                        double igst = string.IsNullOrEmpty(textBox18.Text) ? 0 : Convert.ToDouble(textBox18.Text);
                        if (isGSTApplicableForCompany(searchInv.CompanyId))
                        {
                            
                            if (searchInv.CustomerGST !=null &&   !searchInv.CustomerGST.StartsWith("37")) igst = _defaultTax;
                            else sgst = cgst = _defaultTax/2;

                        }
                        else
                            sgst = cgst = igst = 0;


                        if (igst > 0)
                        {
                            priceWithoutGST = price;// priceRound(price - (price * (igst / (100 + igst))), 2);
                        }
                        else
                            priceWithoutGST = price;//priceRound(price - (price * ((sgst + cgst) / (100 + sgst + cgst))), 2);

                        currProduct.RateWithoutTax = priceWithoutGST.ToString();


                        if (price > 0 && qnt > 0 && priceWithoutGST > 0)
                        {
                            double rowTotal = 0;
                            if (!String.IsNullOrEmpty(currProduct.Mts) && Convert.ToDouble(currProduct.Mts) > 0)
                            {
                                qnt = Convert.ToDouble(currProduct.Mts);
                                rowTotal = priceRound(Math.Round((priceWithoutGST * qnt), 2), 2);
                            }
                            else
                                rowTotal = Math.Round((priceWithoutGST * qnt), 2);

                            currProduct.Total = rowTotal.ToString();
                        }
                        currProduct.Id = SaveInvoiceTableData(currProduct);
                     
                    }

                }
                //new AddInvoice(Convert.ToInt32(billNo), companyId).Show();
            }


            MessageBox.Show("Bill Details copied successfully...!!");
            if (Convert.ToInt32(billNo) > 0 && companyId > 0)
            {
                new AddInvoice(Convert.ToInt32(billNo), companyId).Show();
            }

        }

        private void textBox6_Leave(object sender, EventArgs e)
        {
            if (textBox6.Text.Length>3 &&    !textBox6.Text.StartsWith("37"))
            {
                textBox18.Text = (_defaultTax).ToString();
                textBox16.Text = "";
                textBox17.Text = "";
            }
            else
            {
                textBox18.Text = "";
                textBox16.Text = (_defaultTax / 2).ToString();
                textBox17.Text = (_defaultTax / 2).ToString();
            }
            updateTotalBill();
        }

        private void AddInvoice_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isFormModified)
            {
                ShowMsgToSave();
                e.Cancel = true;
            }
        }

        private void ShowMsgToSave()
        {
            MessageBox.Show("Looks that Bill has modified, Please click Save/Update belore closing..");

        }

        public void SetTextChanged(object sender, EventArgs e)
        {
            if(!isFirstSearchClick)
              isFormModified = true;
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            int billNo;

            DateTime invDate = dateTimePicker1.Value;

            bool isCurrentYearBill = Functions.isCurrentFinancialYear(invDate);

            if (int.TryParse(textBox12.Text.ToString(), out billNo))
            {
                button1.Visible = true;
                button5.Visible = true;
                button2.Visible = true;
                button7.Visible = true;

                button9.Visible = false;
                if (_selectedCompany.ThermalPrinter)
                { button1.Visible = false; button5.Visible = false; }

            }
            else
            {
                button1.Visible = false;
                button5.Visible = false;
                button2.Visible = false;
                button7.Visible = false;

                button9.Visible = true;
            }

            if (isCurrentYearBill == false)
            {
                button1.Visible = false;
                button5.Visible = false;
                button2.Visible = false;
                button7.Visible = false;

                button9.Visible = true;
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            new InvoicePrint(searchInv1, _selectedCompany).Show();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox12.Text = textBox12.Text + "-"+ Functions.getPreviousYear();
            }
            else
            {
                textBox12.Text = textBox12.Text.Split('-')[0];
            }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox13_Leave(object sender, EventArgs e)
        {
            _isDiscountEditedManually = true;
            updateTotalBill();
        }

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled =! (char.IsDigit(e.KeyChar)|| e.KeyChar == '-'||e.KeyChar== '\b');
        }

        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == '\b');
        }

        private void button10_Click(object sender, EventArgs e)
        {
            
            this.Hide();
            var form2 = new AddInvoice(true);
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }
    }
}
