namespace PC_Demo
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.comboBoxBaudrate = new System.Windows.Forms.ComboBox();
            this.buttonPortOpen = new System.Windows.Forms.Button();
            this.comboBoxPortName = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonImage = new System.Windows.Forms.Button();
            this.buttonEnforcement = new System.Windows.Forms.Button();
            this.btn_dust_prn = new System.Windows.Forms.Button();
            this.btn_preview = new System.Windows.Forms.Button();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxBaudrate
            // 
            this.comboBoxBaudrate.FormattingEnabled = true;
            this.comboBoxBaudrate.Location = new System.Drawing.Point(253, 454);
            this.comboBoxBaudrate.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxBaudrate.Name = "comboBoxBaudrate";
            this.comboBoxBaudrate.Size = new System.Drawing.Size(160, 23);
            this.comboBoxBaudrate.TabIndex = 3;
            this.comboBoxBaudrate.Visible = false;
            // 
            // buttonPortOpen
            // 
            this.buttonPortOpen.Location = new System.Drawing.Point(261, 21);
            this.buttonPortOpen.Margin = new System.Windows.Forms.Padding(4);
            this.buttonPortOpen.Name = "buttonPortOpen";
            this.buttonPortOpen.Size = new System.Drawing.Size(106, 39);
            this.buttonPortOpen.TabIndex = 4;
            this.buttonPortOpen.Text = "打开";
            this.buttonPortOpen.UseVisualStyleBackColor = true;
            this.buttonPortOpen.Click += new System.EventHandler(this.buttonPortOpen_Click);
            // 
            // comboBoxPortName
            // 
            this.comboBoxPortName.Location = new System.Drawing.Point(85, 24);
            this.comboBoxPortName.Name = "comboBoxPortName";
            this.comboBoxPortName.Size = new System.Drawing.Size(140, 23);
            this.comboBoxPortName.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxPortName);
            this.groupBox1.Location = new System.Drawing.Point(9, 5);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(366, 63);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "打印机接口";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "串  口：";
            // 
            // buttonImage
            // 
            this.buttonImage.Location = new System.Drawing.Point(313, 454);
            this.buttonImage.Margin = new System.Windows.Forms.Padding(4);
            this.buttonImage.Name = "buttonImage";
            this.buttonImage.Size = new System.Drawing.Size(100, 29);
            this.buttonImage.TabIndex = 13;
            this.buttonImage.Text = "图像打印测试";
            this.buttonImage.UseVisualStyleBackColor = true;
            this.buttonImage.Visible = false;
            this.buttonImage.Click += new System.EventHandler(this.buttonImage_Click);
            // 
            // buttonEnforcement
            // 
            this.buttonEnforcement.Location = new System.Drawing.Point(313, 491);
            this.buttonEnforcement.Margin = new System.Windows.Forms.Padding(4);
            this.buttonEnforcement.Name = "buttonEnforcement";
            this.buttonEnforcement.Size = new System.Drawing.Size(100, 29);
            this.buttonEnforcement.TabIndex = 12;
            this.buttonEnforcement.Text = "单据打印测试";
            this.buttonEnforcement.UseVisualStyleBackColor = true;
            this.buttonEnforcement.Visible = false;
            this.buttonEnforcement.Click += new System.EventHandler(this.buttonEnforcement_Click);
            // 
            // btn_dust_prn
            // 
            this.btn_dust_prn.Location = new System.Drawing.Point(212, 73);
            this.btn_dust_prn.Margin = new System.Windows.Forms.Padding(4);
            this.btn_dust_prn.Name = "btn_dust_prn";
            this.btn_dust_prn.Size = new System.Drawing.Size(155, 36);
            this.btn_dust_prn.TabIndex = 12;
            this.btn_dust_prn.Text = "打印报告";
            this.btn_dust_prn.UseVisualStyleBackColor = true;
            this.btn_dust_prn.Click += new System.EventHandler(this.btnDustPrn_Click);
            // 
            // btn_preview
            // 
            this.btn_preview.Location = new System.Drawing.Point(9, 73);
            this.btn_preview.Name = "btn_preview";
            this.btn_preview.Size = new System.Drawing.Size(155, 36);
            this.btn_preview.TabIndex = 14;
            this.btn_preview.Text = "内容预览";
            this.btn_preview.UseVisualStyleBackColor = true;
            this.btn_preview.Click += new System.EventHandler(this.btn_preview_Click);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(13, 138);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(362, 269);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 416);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btn_preview);
            this.Controls.Add(this.buttonImage);
            this.Controls.Add(this.comboBoxBaudrate);
            this.Controls.Add(this.btn_dust_prn);
            this.Controls.Add(this.buttonEnforcement);
            this.Controls.Add(this.buttonPortOpen);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "报告打印";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxBaudrate;
        private System.Windows.Forms.Button buttonPortOpen;
        private System.Windows.Forms.ComboBox comboBoxPortName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonImage;
        private System.Windows.Forms.Button buttonEnforcement;
        private System.Windows.Forms.Button btn_dust_prn;
        private System.Windows.Forms.Button btn_preview;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

