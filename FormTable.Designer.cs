namespace EngineApplication
{
    partial class FormTable
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
            this.dtGridView = new System.Windows.Forms.DataGridView();
            this.tbarTotalRecords = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.dtGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dtGridView
            // 
            this.dtGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGridView.Location = new System.Drawing.Point(0, -1);
            this.dtGridView.Name = "dtGridView";
            this.dtGridView.RowTemplate.Height = 23;
            this.dtGridView.Size = new System.Drawing.Size(525, 340);
            this.dtGridView.TabIndex = 0;
            // 
            // tbarTotalRecords
            // 
            this.tbarTotalRecords.Location = new System.Drawing.Point(0, 353);
            this.tbarTotalRecords.Name = "tbarTotalRecords";
            this.tbarTotalRecords.Size = new System.Drawing.Size(537, 22);
            this.tbarTotalRecords.TabIndex = 1;
            // 
            // FormTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 375);
            this.Controls.Add(this.tbarTotalRecords);
            this.Controls.Add(this.dtGridView);
            this.Name = "FormTable";
            this.Text = "FormTable";
            this.Load += new System.EventHandler(this.FormTable_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dtGridView;
        private System.Windows.Forms.StatusStrip tbarTotalRecords;
    }
}