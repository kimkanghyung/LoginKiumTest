namespace LoginTestKium
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.axKHOpenAPI1 = new AxKHOpenAPILib.AxKHOpenAPI();
            this.ulpDataGridView = new System.Windows.Forms.DataGridView();
            this.종목명 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.현재가 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.대비 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.등락률 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.거래량 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.매수잔량 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.연속 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ulpDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // axKHOpenAPI1
            // 
            this.axKHOpenAPI1.Enabled = true;
            this.axKHOpenAPI1.Location = new System.Drawing.Point(2, 407);
            this.axKHOpenAPI1.Name = "axKHOpenAPI1";
            this.axKHOpenAPI1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKHOpenAPI1.OcxState")));
            this.axKHOpenAPI1.Size = new System.Drawing.Size(100, 50);
            this.axKHOpenAPI1.TabIndex = 0;
            // 
            // ulpDataGridView
            // 
            this.ulpDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ulpDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.종목명,
            this.현재가,
            this.대비,
            this.등락률,
            this.거래량,
            this.매수잔량,
            this.연속});
            this.ulpDataGridView.Location = new System.Drawing.Point(26, 42);
            this.ulpDataGridView.Name = "ulpDataGridView";
            this.ulpDataGridView.RowTemplate.Height = 27;
            this.ulpDataGridView.Size = new System.Drawing.Size(771, 343);
            this.ulpDataGridView.TabIndex = 1;
            // 
            // 종목명
            // 
            this.종목명.HeaderText = "종목명";
            this.종목명.Name = "종목명";
            // 
            // 현재가
            // 
            this.현재가.HeaderText = "현재가";
            this.현재가.Name = "현재가";
            // 
            // 대비
            // 
            this.대비.HeaderText = "대비";
            this.대비.Name = "대비";
            // 
            // 등락률
            // 
            this.등락률.HeaderText = "등락률";
            this.등락률.Name = "등락률";
            // 
            // 거래량
            // 
            this.거래량.HeaderText = "거래량";
            this.거래량.Name = "거래량";
            // 
            // 매수잔량
            // 
            this.매수잔량.HeaderText = "매수잔량";
            this.매수잔량.Name = "매수잔량";
            // 
            // 연속
            // 
            this.연속.HeaderText = "연속";
            this.연속.Name = "연속";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(26, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(771, 25);
            this.textBox1.TabIndex = 2;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 454);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ulpDataGridView);
            this.Controls.Add(this.axKHOpenAPI1);
            this.Name = "Form2";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ulpDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI1;
        private System.Windows.Forms.DataGridView ulpDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn 종목명;
        private System.Windows.Forms.DataGridViewTextBoxColumn 현재가;
        private System.Windows.Forms.DataGridViewTextBoxColumn 대비;
        private System.Windows.Forms.DataGridViewTextBoxColumn 등락률;
        private System.Windows.Forms.DataGridViewTextBoxColumn 거래량;
        private System.Windows.Forms.DataGridViewTextBoxColumn 매수잔량;
        private System.Windows.Forms.DataGridViewTextBoxColumn 연속;
        private System.Windows.Forms.TextBox textBox1;
    }
}