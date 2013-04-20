namespace AglonaReader
{
    partial class FindForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindForm));
            this.label1 = new System.Windows.Forms.Label();
            this.textToFindBox = new System.Windows.Forms.TextBox();
            this.leftTextRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rightTextRadioButton = new System.Windows.Forms.RadioButton();
            this.bothTextsRadioButton = new System.Windows.Forms.RadioButton();
            this.findNextButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text to find:";
            // 
            // textToFindBox
            // 
            this.textToFindBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textToFindBox.Location = new System.Drawing.Point(79, 6);
            this.textToFindBox.Name = "textToFindBox";
            this.textToFindBox.Size = new System.Drawing.Size(309, 20);
            this.textToFindBox.TabIndex = 1;
            // 
            // leftTextRadioButton
            // 
            this.leftTextRadioButton.AutoSize = true;
            this.leftTextRadioButton.Location = new System.Drawing.Point(6, 19);
            this.leftTextRadioButton.Name = "leftTextRadioButton";
            this.leftTextRadioButton.Size = new System.Drawing.Size(43, 17);
            this.leftTextRadioButton.TabIndex = 3;
            this.leftTextRadioButton.TabStop = true;
            this.leftTextRadioButton.Text = "Left";
            this.leftTextRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.rightTextRadioButton);
            this.groupBox1.Controls.Add(this.bothTextsRadioButton);
            this.groupBox1.Controls.Add(this.leftTextRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(162, 44);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Texts";
            // 
            // rightTextRadioButton
            // 
            this.rightTextRadioButton.AutoSize = true;
            this.rightTextRadioButton.Location = new System.Drawing.Point(110, 20);
            this.rightTextRadioButton.Name = "rightTextRadioButton";
            this.rightTextRadioButton.Size = new System.Drawing.Size(50, 17);
            this.rightTextRadioButton.TabIndex = 5;
            this.rightTextRadioButton.TabStop = true;
            this.rightTextRadioButton.Text = "Right";
            this.rightTextRadioButton.UseVisualStyleBackColor = true;
            // 
            // bothTextsRadioButton
            // 
            this.bothTextsRadioButton.AutoSize = true;
            this.bothTextsRadioButton.Location = new System.Drawing.Point(56, 20);
            this.bothTextsRadioButton.Name = "bothTextsRadioButton";
            this.bothTextsRadioButton.Size = new System.Drawing.Size(47, 17);
            this.bothTextsRadioButton.TabIndex = 4;
            this.bothTextsRadioButton.TabStop = true;
            this.bothTextsRadioButton.Text = "Both";
            this.bothTextsRadioButton.UseVisualStyleBackColor = true;
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.findNextButton.Location = new System.Drawing.Point(313, 56);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(75, 23);
            this.findNextButton.TabIndex = 2;
            this.findNextButton.Text = "Find next";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
            // 
            // FindForm
            // 
            this.AcceptButton = this.findNextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 91);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textToFindBox);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(416, 129);
            this.Name = "FindForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find text";
            this.Activated += new System.EventHandler(this.FindForm_Activated);
            this.Load += new System.EventHandler(this.FindForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textToFindBox;
        private System.Windows.Forms.RadioButton leftTextRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rightTextRadioButton;
        private System.Windows.Forms.RadioButton bothTextsRadioButton;
        private System.Windows.Forms.Button findNextButton;
    }
}