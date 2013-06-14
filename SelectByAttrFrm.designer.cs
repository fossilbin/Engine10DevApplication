namespace EngineApplication
{
    partial class SelectByAttrFrm
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
            this.buttonGetValue = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxZoomtoSelected = new System.Windows.Forms.CheckBox();
            this.buttonChar = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.checkBoxShowVectorOnly = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxWhereClause = new System.Windows.Forms.TextBox();
            this.labelDescription2 = new System.Windows.Forms.Label();
            this.labelDescription3 = new System.Windows.Forms.Label();
            this.labelDescription1 = new System.Windows.Forms.Label();
            this.buttonChars = new System.Windows.Forms.Button();
            this.buttonIs = new System.Windows.Forms.Button();
            this.buttonNot = new System.Windows.Forms.Button();
            this.buttonBrace = new System.Windows.Forms.Button();
            this.buttonOr = new System.Windows.Forms.Button();
            this.buttonBig = new System.Windows.Forms.Button();
            this.buttonBigEqual = new System.Windows.Forms.Button();
            this.buttonSmallEqual = new System.Windows.Forms.Button();
            this.buttonAnd = new System.Windows.Forms.Button();
            this.buttonSmall = new System.Windows.Forms.Button();
            this.buttonNotEqual = new System.Windows.Forms.Button();
            this.buttonLike = new System.Windows.Forms.Button();
            this.buttonEqual = new System.Windows.Forms.Button();
            this.listBoxValues = new System.Windows.Forms.ListBox();
            this.listBoxFields = new System.Windows.Forms.ListBox();
            this.comboBoxMethod = new System.Windows.Forms.ComboBox();
            this.Method = new System.Windows.Forms.Label();
            this.comboBoxLayers = new System.Windows.Forms.ComboBox();
            this.LabelLayers = new System.Windows.Forms.Label();
            this.labelLayer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonGetValue
            // 
            this.buttonGetValue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonGetValue.Location = new System.Drawing.Point(66, 297);
            this.buttonGetValue.Name = "buttonGetValue";
            this.buttonGetValue.Size = new System.Drawing.Size(90, 23);
            this.buttonGetValue.TabIndex = 70;
            this.buttonGetValue.Text = "获得属性值";
            this.buttonGetValue.UseVisualStyleBackColor = true;
            this.buttonGetValue.Click += new System.EventHandler(this.buttonGetValue_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(325, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 69;
            // 
            // checkBoxZoomtoSelected
            // 
            this.checkBoxZoomtoSelected.AutoSize = true;
            this.checkBoxZoomtoSelected.Checked = true;
            this.checkBoxZoomtoSelected.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxZoomtoSelected.Location = new System.Drawing.Point(287, 355);
            this.checkBoxZoomtoSelected.Name = "checkBoxZoomtoSelected";
            this.checkBoxZoomtoSelected.Size = new System.Drawing.Size(108, 16);
            this.checkBoxZoomtoSelected.TabIndex = 68;
            this.checkBoxZoomtoSelected.Text = "定位到查询结果";
            this.checkBoxZoomtoSelected.UseVisualStyleBackColor = true;
            this.checkBoxZoomtoSelected.Visible = false;
            // 
            // buttonChar
            // 
            this.buttonChar.Location = new System.Drawing.Point(39, 268);
            this.buttonChar.Name = "buttonChar";
            this.buttonChar.Size = new System.Drawing.Size(21, 23);
            this.buttonChar.TabIndex = 67;
            this.buttonChar.Text = "_";
            this.buttonChar.UseVisualStyleBackColor = true;
            this.buttonChar.Click += new System.EventHandler(this.buttonChar_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(147, 495);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 65;
            this.buttonApply.Text = "应用";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // checkBoxShowVectorOnly
            // 
            this.checkBoxShowVectorOnly.AutoSize = true;
            this.checkBoxShowVectorOnly.Enabled = false;
            this.checkBoxShowVectorOnly.Location = new System.Drawing.Point(77, 34);
            this.checkBoxShowVectorOnly.Name = "checkBoxShowVectorOnly";
            this.checkBoxShowVectorOnly.Size = new System.Drawing.Size(108, 16);
            this.checkBoxShowVectorOnly.TabIndex = 64;
            this.checkBoxShowVectorOnly.Text = "只显示矢量图层";
            this.checkBoxShowVectorOnly.UseVisualStyleBackColor = true;
            this.checkBoxShowVectorOnly.Visible = false;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(10, 495);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 63;
            this.buttonClear.Text = "清空";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(313, 495);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 62;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(230, 495);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 61;
            this.buttonOk.Text = "确定";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // textBoxWhereClause
            // 
            this.textBoxWhereClause.Location = new System.Drawing.Point(10, 377);
            this.textBoxWhereClause.Multiline = true;
            this.textBoxWhereClause.Name = "textBoxWhereClause";
            this.textBoxWhereClause.Size = new System.Drawing.Size(381, 112);
            this.textBoxWhereClause.TabIndex = 60;
            // 
            // labelDescription2
            // 
            this.labelDescription2.AutoSize = true;
            this.labelDescription2.Location = new System.Drawing.Point(101, 385);
            this.labelDescription2.Name = "labelDescription2";
            this.labelDescription2.Size = new System.Drawing.Size(0, 12);
            this.labelDescription2.TabIndex = 59;
            // 
            // labelDescription3
            // 
            this.labelDescription3.AutoSize = true;
            this.labelDescription3.Location = new System.Drawing.Point(211, 359);
            this.labelDescription3.Name = "labelDescription3";
            this.labelDescription3.Size = new System.Drawing.Size(35, 12);
            this.labelDescription3.TabIndex = 58;
            this.labelDescription3.Text = "Where";
            // 
            // labelDescription1
            // 
            this.labelDescription1.AutoSize = true;
            this.labelDescription1.Location = new System.Drawing.Point(15, 359);
            this.labelDescription1.Name = "labelDescription1";
            this.labelDescription1.Size = new System.Drawing.Size(89, 12);
            this.labelDescription1.TabIndex = 57;
            this.labelDescription1.Text = "Select * From ";
            // 
            // buttonChars
            // 
            this.buttonChars.Location = new System.Drawing.Point(17, 268);
            this.buttonChars.Name = "buttonChars";
            this.buttonChars.Size = new System.Drawing.Size(21, 23);
            this.buttonChars.TabIndex = 56;
            this.buttonChars.Text = "%";
            this.buttonChars.UseVisualStyleBackColor = true;
            this.buttonChars.Click += new System.EventHandler(this.buttonChars_Click);
            // 
            // buttonIs
            // 
            this.buttonIs.Location = new System.Drawing.Point(17, 297);
            this.buttonIs.Name = "buttonIs";
            this.buttonIs.Size = new System.Drawing.Size(43, 23);
            this.buttonIs.TabIndex = 55;
            this.buttonIs.Text = "Is";
            this.buttonIs.UseVisualStyleBackColor = true;
            this.buttonIs.Click += new System.EventHandler(this.buttonIs_Click);
            // 
            // buttonNot
            // 
            this.buttonNot.Location = new System.Drawing.Point(115, 268);
            this.buttonNot.Name = "buttonNot";
            this.buttonNot.Size = new System.Drawing.Size(43, 23);
            this.buttonNot.TabIndex = 54;
            this.buttonNot.Text = "Not";
            this.buttonNot.UseVisualStyleBackColor = true;
            this.buttonNot.Click += new System.EventHandler(this.buttonNot_Click);
            // 
            // buttonBrace
            // 
            this.buttonBrace.Location = new System.Drawing.Point(66, 268);
            this.buttonBrace.Name = "buttonBrace";
            this.buttonBrace.Size = new System.Drawing.Size(43, 23);
            this.buttonBrace.TabIndex = 53;
            this.buttonBrace.Text = "( )";
            this.buttonBrace.UseVisualStyleBackColor = true;
            this.buttonBrace.Click += new System.EventHandler(this.buttonBrace_Click);
            // 
            // buttonOr
            // 
            this.buttonOr.Location = new System.Drawing.Point(115, 239);
            this.buttonOr.Name = "buttonOr";
            this.buttonOr.Size = new System.Drawing.Size(43, 23);
            this.buttonOr.TabIndex = 52;
            this.buttonOr.Text = "Or";
            this.buttonOr.UseVisualStyleBackColor = true;
            this.buttonOr.Click += new System.EventHandler(this.buttonOr_Click);
            // 
            // buttonBig
            // 
            this.buttonBig.Location = new System.Drawing.Point(17, 210);
            this.buttonBig.Name = "buttonBig";
            this.buttonBig.Size = new System.Drawing.Size(43, 23);
            this.buttonBig.TabIndex = 51;
            this.buttonBig.Text = ">";
            this.buttonBig.UseVisualStyleBackColor = true;
            this.buttonBig.Click += new System.EventHandler(this.buttonBig_Click);
            // 
            // buttonBigEqual
            // 
            this.buttonBigEqual.Location = new System.Drawing.Point(66, 210);
            this.buttonBigEqual.Name = "buttonBigEqual";
            this.buttonBigEqual.Size = new System.Drawing.Size(43, 23);
            this.buttonBigEqual.TabIndex = 50;
            this.buttonBigEqual.Text = "> =";
            this.buttonBigEqual.UseVisualStyleBackColor = true;
            this.buttonBigEqual.Click += new System.EventHandler(this.buttonBigEqual_Click);
            // 
            // buttonSmallEqual
            // 
            this.buttonSmallEqual.Location = new System.Drawing.Point(66, 239);
            this.buttonSmallEqual.Name = "buttonSmallEqual";
            this.buttonSmallEqual.Size = new System.Drawing.Size(43, 23);
            this.buttonSmallEqual.TabIndex = 49;
            this.buttonSmallEqual.Text = "< =";
            this.buttonSmallEqual.UseVisualStyleBackColor = true;
            this.buttonSmallEqual.Click += new System.EventHandler(this.buttonSmallEqual_Click);
            // 
            // buttonAnd
            // 
            this.buttonAnd.Location = new System.Drawing.Point(115, 210);
            this.buttonAnd.Name = "buttonAnd";
            this.buttonAnd.Size = new System.Drawing.Size(43, 23);
            this.buttonAnd.TabIndex = 48;
            this.buttonAnd.Text = "And";
            this.buttonAnd.UseVisualStyleBackColor = true;
            this.buttonAnd.Click += new System.EventHandler(this.buttonAnd_Click);
            // 
            // buttonSmall
            // 
            this.buttonSmall.Location = new System.Drawing.Point(17, 239);
            this.buttonSmall.Name = "buttonSmall";
            this.buttonSmall.Size = new System.Drawing.Size(43, 23);
            this.buttonSmall.TabIndex = 47;
            this.buttonSmall.Text = "<";
            this.buttonSmall.UseVisualStyleBackColor = true;
            this.buttonSmall.Click += new System.EventHandler(this.buttonSmall_Click);
            // 
            // buttonNotEqual
            // 
            this.buttonNotEqual.Location = new System.Drawing.Point(66, 181);
            this.buttonNotEqual.Name = "buttonNotEqual";
            this.buttonNotEqual.Size = new System.Drawing.Size(43, 23);
            this.buttonNotEqual.TabIndex = 46;
            this.buttonNotEqual.Text = "< >";
            this.buttonNotEqual.UseVisualStyleBackColor = true;
            this.buttonNotEqual.Click += new System.EventHandler(this.buttonNotEqual_Click);
            // 
            // buttonLike
            // 
            this.buttonLike.Location = new System.Drawing.Point(115, 181);
            this.buttonLike.Name = "buttonLike";
            this.buttonLike.Size = new System.Drawing.Size(43, 23);
            this.buttonLike.TabIndex = 45;
            this.buttonLike.Text = "Like";
            this.buttonLike.UseVisualStyleBackColor = true;
            this.buttonLike.Click += new System.EventHandler(this.buttonLike_Click);
            // 
            // buttonEqual
            // 
            this.buttonEqual.Location = new System.Drawing.Point(17, 181);
            this.buttonEqual.Name = "buttonEqual";
            this.buttonEqual.Size = new System.Drawing.Size(43, 23);
            this.buttonEqual.TabIndex = 44;
            this.buttonEqual.Text = "=";
            this.buttonEqual.UseVisualStyleBackColor = true;
            this.buttonEqual.Click += new System.EventHandler(this.buttonEqual_Click);
            // 
            // listBoxValues
            // 
            this.listBoxValues.FormattingEnabled = true;
            this.listBoxValues.HorizontalScrollbar = true;
            this.listBoxValues.ItemHeight = 12;
            this.listBoxValues.Location = new System.Drawing.Point(164, 177);
            this.listBoxValues.Name = "listBoxValues";
            this.listBoxValues.ScrollAlwaysVisible = true;
            this.listBoxValues.Size = new System.Drawing.Size(231, 172);
            this.listBoxValues.TabIndex = 43;
            this.listBoxValues.DoubleClick += new System.EventHandler(this.listBoxValues_DoubleClick);
            // 
            // listBoxFields
            // 
            this.listBoxFields.FormattingEnabled = true;
            this.listBoxFields.HorizontalScrollbar = true;
            this.listBoxFields.ItemHeight = 12;
            this.listBoxFields.Location = new System.Drawing.Point(14, 83);
            this.listBoxFields.Name = "listBoxFields";
            this.listBoxFields.ScrollAlwaysVisible = true;
            this.listBoxFields.Size = new System.Drawing.Size(381, 88);
            this.listBoxFields.TabIndex = 41;
            this.listBoxFields.DoubleClick += new System.EventHandler(this.listBoxFields_DoubleClick);
            this.listBoxFields.SelectedIndexChanged += new System.EventHandler(this.listBoxFields_SelectedIndexChanged);
            // 
            // comboBoxMethod
            // 
            this.comboBoxMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMethod.FormattingEnabled = true;
            this.comboBoxMethod.Items.AddRange(new object[] {
            "生成新的选择集 (Creates a new selection)",
            "添加到当前的选择集 (Adds to the current selection)",
            "从当前的选择集中去除 (Subtracts from the current selection)",
            "在当前的选择集中选择 (Selects from the current selection)"});
            this.comboBoxMethod.Location = new System.Drawing.Point(77, 56);
            this.comboBoxMethod.Name = "comboBoxMethod";
            this.comboBoxMethod.Size = new System.Drawing.Size(318, 20);
            this.comboBoxMethod.TabIndex = 39;
            // 
            // Method
            // 
            this.Method.AutoSize = true;
            this.Method.Location = new System.Drawing.Point(12, 59);
            this.Method.Name = "Method";
            this.Method.Size = new System.Drawing.Size(59, 12);
            this.Method.TabIndex = 38;
            this.Method.Text = "查询方法:";
            // 
            // comboBoxLayers
            // 
            this.comboBoxLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLayers.FormattingEnabled = true;
            this.comboBoxLayers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.comboBoxLayers.Location = new System.Drawing.Point(77, 6);
            this.comboBoxLayers.Name = "comboBoxLayers";
            this.comboBoxLayers.Size = new System.Drawing.Size(318, 20);
            this.comboBoxLayers.TabIndex = 37;
            this.comboBoxLayers.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayers_SelectedIndexChanged);
            // 
            // LabelLayers
            // 
            this.LabelLayers.AutoSize = true;
            this.LabelLayers.Location = new System.Drawing.Point(12, 9);
            this.LabelLayers.Name = "LabelLayers";
            this.LabelLayers.Size = new System.Drawing.Size(59, 12);
            this.LabelLayers.TabIndex = 36;
            this.LabelLayers.Text = "图层名称:";
            // 
            // labelLayer
            // 
            this.labelLayer.AutoSize = true;
            this.labelLayer.Location = new System.Drawing.Point(103, 359);
            this.labelLayer.Name = "labelLayer";
            this.labelLayer.Size = new System.Drawing.Size(0, 12);
            this.labelLayer.TabIndex = 71;
            // 
            // SelectByAttrFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 527);
            this.Controls.Add(this.labelLayer);
            this.Controls.Add(this.buttonGetValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxZoomtoSelected);
            this.Controls.Add(this.buttonChar);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.checkBoxShowVectorOnly);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxWhereClause);
            this.Controls.Add(this.labelDescription2);
            this.Controls.Add(this.labelDescription3);
            this.Controls.Add(this.labelDescription1);
            this.Controls.Add(this.buttonChars);
            this.Controls.Add(this.buttonIs);
            this.Controls.Add(this.buttonNot);
            this.Controls.Add(this.buttonBrace);
            this.Controls.Add(this.buttonOr);
            this.Controls.Add(this.buttonBig);
            this.Controls.Add(this.buttonBigEqual);
            this.Controls.Add(this.buttonSmallEqual);
            this.Controls.Add(this.buttonAnd);
            this.Controls.Add(this.buttonSmall);
            this.Controls.Add(this.buttonNotEqual);
            this.Controls.Add(this.buttonLike);
            this.Controls.Add(this.buttonEqual);
            this.Controls.Add(this.listBoxValues);
            this.Controls.Add(this.listBoxFields);
            this.Controls.Add(this.comboBoxMethod);
            this.Controls.Add(this.Method);
            this.Controls.Add(this.comboBoxLayers);
            this.Controls.Add(this.LabelLayers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectByAttrFrm";
            this.ShowInTaskbar = false;
            this.Text = "多功能查询";
            this.Load += new System.EventHandler(this.SelectByAttrFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGetValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxZoomtoSelected;
        private System.Windows.Forms.Button buttonChar;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.CheckBox checkBoxShowVectorOnly;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.TextBox textBoxWhereClause;
        private System.Windows.Forms.Label labelDescription2;
        private System.Windows.Forms.Label labelDescription3;
        private System.Windows.Forms.Label labelDescription1;
        private System.Windows.Forms.Button buttonChars;
        private System.Windows.Forms.Button buttonIs;
        private System.Windows.Forms.Button buttonNot;
        private System.Windows.Forms.Button buttonBrace;
        private System.Windows.Forms.Button buttonOr;
        private System.Windows.Forms.Button buttonBig;
        private System.Windows.Forms.Button buttonBigEqual;
        private System.Windows.Forms.Button buttonSmallEqual;
        private System.Windows.Forms.Button buttonAnd;
        private System.Windows.Forms.Button buttonSmall;
        private System.Windows.Forms.Button buttonNotEqual;
        private System.Windows.Forms.Button buttonLike;
        private System.Windows.Forms.Button buttonEqual;
        private System.Windows.Forms.ListBox listBoxValues;
        private System.Windows.Forms.ListBox listBoxFields;
        private System.Windows.Forms.ComboBox comboBoxMethod;
        private System.Windows.Forms.Label Method;
        private System.Windows.Forms.ComboBox comboBoxLayers;
        private System.Windows.Forms.Label LabelLayers;
        private System.Windows.Forms.Label labelLayer;
    }
}