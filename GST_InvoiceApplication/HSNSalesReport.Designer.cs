namespace GST_InvoiceApplication
{
    partial class HSNSalesReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button6 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.HSNCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NoOfBills = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalWithGST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalWithOutGSTApprox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IGSTWithGST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IGSTWithOutGST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SGSTWithGST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SGSTWithOutGST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CGSTWithGST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CGSTWithOutGST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pieces = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Sitka Text", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(1157, 12);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(106, 48);
            this.button6.TabIndex = 8;
            this.button6.Text = "Home";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(115, 22);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(182, 20);
            this.dateTimePicker1.TabIndex = 9;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(343, 22);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(179, 20);
            this.dateTimePicker2.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(317, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "To";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(539, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Get Report";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.HSNCode,
            this.NoOfBills,
            this.TotalWithGST,
            this.TotalWithOutGSTApprox,
            this.IGSTWithGST,
            this.IGSTWithOutGST,
            this.SGSTWithGST,
            this.SGSTWithOutGST,
            this.CGSTWithGST,
            this.CGSTWithOutGST,
            this.Pieces,
            this.Error});
            this.dataGridView1.Location = new System.Drawing.Point(12, 89);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1239, 537);
            this.dataGridView1.TabIndex = 14;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // HSNCode
            // 
            this.HSNCode.HeaderText = "HSNCode";
            this.HSNCode.Name = "HSNCode";
            this.HSNCode.ReadOnly = true;
            // 
            // NoOfBills
            // 
            this.NoOfBills.HeaderText = "NoOfBills";
            this.NoOfBills.Name = "NoOfBills";
            this.NoOfBills.ReadOnly = true;
            // 
            // TotalWithGST
            // 
            this.TotalWithGST.HeaderText = "TotalWithGST";
            this.TotalWithGST.MinimumWidth = 30;
            this.TotalWithGST.Name = "TotalWithGST";
            this.TotalWithGST.ReadOnly = true;
            this.TotalWithGST.Width = 170;
            // 
            // TotalWithOutGSTApprox
            // 
            this.TotalWithOutGSTApprox.HeaderText = "TotalWithOutGST(Approx)";
            this.TotalWithOutGSTApprox.MinimumWidth = 30;
            this.TotalWithOutGSTApprox.Name = "TotalWithOutGSTApprox";
            this.TotalWithOutGSTApprox.ReadOnly = true;
            this.TotalWithOutGSTApprox.Width = 170;
            // 
            // IGSTWithGST
            // 
            this.IGSTWithGST.HeaderText = "IGSTWithGST";
            this.IGSTWithGST.Name = "IGSTWithGST";
            this.IGSTWithGST.ReadOnly = true;
            this.IGSTWithGST.Visible = false;
            this.IGSTWithGST.Width = 170;
            // 
            // IGSTWithOutGST
            // 
            this.IGSTWithOutGST.HeaderText = "IGSTWithOutGST";
            this.IGSTWithOutGST.Name = "IGSTWithOutGST";
            this.IGSTWithOutGST.ReadOnly = true;
            this.IGSTWithOutGST.Width = 150;
            // 
            // SGSTWithGST
            // 
            this.SGSTWithGST.HeaderText = "SGSTWithGST";
            this.SGSTWithGST.Name = "SGSTWithGST";
            this.SGSTWithGST.ReadOnly = true;
            this.SGSTWithGST.Visible = false;
            this.SGSTWithGST.Width = 170;
            // 
            // SGSTWithOutGST
            // 
            this.SGSTWithOutGST.HeaderText = "SGSTWithOutGST";
            this.SGSTWithOutGST.Name = "SGSTWithOutGST";
            this.SGSTWithOutGST.ReadOnly = true;
            this.SGSTWithOutGST.Width = 150;
            // 
            // CGSTWithGST
            // 
            this.CGSTWithGST.HeaderText = "CGSTWithGST";
            this.CGSTWithGST.Name = "CGSTWithGST";
            this.CGSTWithGST.ReadOnly = true;
            this.CGSTWithGST.Visible = false;
            this.CGSTWithGST.Width = 170;
            // 
            // CGSTWithOutGST
            // 
            this.CGSTWithOutGST.HeaderText = "CGSTWithOutGST";
            this.CGSTWithOutGST.Name = "CGSTWithOutGST";
            this.CGSTWithOutGST.ReadOnly = true;
            this.CGSTWithOutGST.Width = 150;
            // 
            // Pieces
            // 
            this.Pieces.HeaderText = "Pieces";
            this.Pieces.Name = "Pieces";
            this.Pieces.ReadOnly = true;
            // 
            // Error
            // 
            this.Error.HeaderText = "Error";
            this.Error.Name = "Error";
            this.Error.ReadOnly = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(151, 664);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(407, 20);
            this.textBox1.TabIndex = 15;
            this.textBox1.Enter += new System.EventHandler(this.textBox1_Enter);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(564, 661);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "Export to Excel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(715, 661);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(467, 23);
            this.progressBar1.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Company";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "CREDIT BILL",
            "CASH BILL"});
            this.comboBox3.Location = new System.Drawing.Point(115, 58);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(179, 21);
            this.comboBox3.TabIndex = 80;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(320, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(202, 35);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "InvoiceType";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(84, 14);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(45, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "B2C";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(33, 14);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(45, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "B2B";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(135, 14);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(47, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Both";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // HSNSalesReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1275, 713);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.button6);
            this.Name = "HSNSalesReport";
            this.Text = "HSNSalesReport";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.DataGridViewTextBoxColumn HSNCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn NoOfBills;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalWithGST;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalWithOutGSTApprox;
        private System.Windows.Forms.DataGridViewTextBoxColumn IGSTWithGST;
        private System.Windows.Forms.DataGridViewTextBoxColumn IGSTWithOutGST;
        private System.Windows.Forms.DataGridViewTextBoxColumn SGSTWithGST;
        private System.Windows.Forms.DataGridViewTextBoxColumn SGSTWithOutGST;
        private System.Windows.Forms.DataGridViewTextBoxColumn CGSTWithGST;
        private System.Windows.Forms.DataGridViewTextBoxColumn CGSTWithOutGST;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pieces;
        private System.Windows.Forms.DataGridViewTextBoxColumn Error;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton3;
    }
}