namespace EngineApplication
{
    partial class FrmQuery
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmbLayers = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxValue = new System.Windows.Forms.ListBox();
            this.btnShowAllValue = new System.Windows.Forms.Button();
            this.btnTable = new System.Windows.Forms.Button();
            
            this.groupBox1.SuspendLayout();
            
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "查询图层";
            // 
            // cmbLayers
            // 
            this.cmbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayers.FormattingEnabled = true;
            this.cmbLayers.Location = new System.Drawing.Point(71, 18);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(121, 20);
            this.cmbLayers.TabIndex = 1;
            this.cmbLayers.TextChanged += new System.EventHandler(this.cmbLayers_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "查询字段";
            // 
            // cmbFields
            // 
            this.cmbFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFields.FormattingEnabled = true;
            this.cmbFields.Location = new System.Drawing.Point(71, 64);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(121, 20);
            this.cmbFields.TabIndex = 3;
            this.cmbFields.DropDown += new System.EventHandler(this.cmbFields_DropDown);
            this.cmbFields.TextChanged += new System.EventHandler(this.cmbFields_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "查询值";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(71, 112);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(82, 21);
            this.txtValue.TabIndex = 5;
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(14, 158);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(55, 23);
            this.btnQuery.TabIndex = 6;
            this.btnQuery.Text = "确定";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(137, 158);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(55, 23);
            this.btnCancle.TabIndex = 7;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxValue);
            this.groupBox1.Location = new System.Drawing.Point(198, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(152, 169);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "字段值";
            // 
            // listBoxValue
            // 
            this.listBoxValue.FormattingEnabled = true;
            this.listBoxValue.ItemHeight = 12;
            this.listBoxValue.Location = new System.Drawing.Point(6, 15);
            this.listBoxValue.Name = "listBoxValue";
            this.listBoxValue.Size = new System.Drawing.Size(140, 148);
            this.listBoxValue.TabIndex = 9;
            this.listBoxValue.DoubleClick += new System.EventHandler(this.listBoxValue_DoubleClick);
            // 
            // btnShowAllValue
            // 
            this.btnShowAllValue.Location = new System.Drawing.Point(159, 111);
            this.btnShowAllValue.Name = "btnShowAllValue";
            this.btnShowAllValue.Size = new System.Drawing.Size(33, 22);
            this.btnShowAllValue.TabIndex = 9;
            this.btnShowAllValue.Text = "值";
            this.btnShowAllValue.UseVisualStyleBackColor = true;
            this.btnShowAllValue.Click += new System.EventHandler(this.btnShowAllValue_Click);
            // 
            // btnTable
            // 
            this.btnTable.Location = new System.Drawing.Point(75, 158);
            this.btnTable.Name = "btnTable";
            this.btnTable.Size = new System.Drawing.Size(55, 23);
            this.btnTable.TabIndex = 10;
            this.btnTable.Text = "表";
            this.btnTable.UseVisualStyleBackColor = true;
            this.btnTable.Click += new System.EventHandler(this.btnTable_Click);
            // 
            // htmluiControl1
            // 
           
            // 
            // FrmQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 202);
           
            this.Controls.Add(this.btnTable);
            this.Controls.Add(this.btnShowAllValue);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbFields);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbLayers);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "FrmQuery";
            this.Text = "查询窗口";
            this.Load += new System.EventHandler(this.FrmQuery_Load);
            this.groupBox1.ResumeLayout(false);
          
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLayers;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxValue;
        private System.Windows.Forms.Button btnShowAllValue;
        private System.Windows.Forms.Button btnTable;
        
    }
}