using System.Windows.Forms;

namespace AglonaReader
{
    partial class EditPairForm
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

        protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
        {
            if (keyData == Keys.Enter)
                return false;

            if (keyData == (Keys.Enter | Keys.Control))
            {
                PressOK();
                return false;
            }

            return base.ProcessDialogKey(keyData);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditPairForm));
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.level0 = new System.Windows.Forms.RadioButton();
            this.level3 = new System.Windows.Forms.RadioButton();
            this.level2 = new System.Windows.Forms.RadioButton();
            this.level1 = new System.Windows.Forms.RadioButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Controls.Add(this.level0);
            this.bottomPanel.Controls.Add(this.level3);
            this.bottomPanel.Controls.Add(this.level2);
            this.bottomPanel.Controls.Add(this.level1);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 130);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(734, 47);
            this.bottomPanel.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(370, 6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(104, 26);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(260, 6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(104, 26);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // level0
            // 
            this.level0.AutoSize = true;
            this.level0.Location = new System.Drawing.Point(123, 6);
            this.level0.Name = "level0";
            this.level0.Size = new System.Drawing.Size(28, 17);
            this.level0.TabIndex = 5;
            this.level0.TabStop = true;
            this.level0.Text = "-";
            this.level0.UseVisualStyleBackColor = true;
            // 
            // level3
            // 
            this.level3.AutoSize = true;
            this.level3.Location = new System.Drawing.Point(86, 6);
            this.level3.Name = "level3";
            this.level3.Size = new System.Drawing.Size(31, 17);
            this.level3.TabIndex = 4;
            this.level3.Text = "3";
            this.level3.UseVisualStyleBackColor = true;
            // 
            // level2
            // 
            this.level2.AutoSize = true;
            this.level2.Location = new System.Drawing.Point(49, 6);
            this.level2.Name = "level2";
            this.level2.Size = new System.Drawing.Size(31, 17);
            this.level2.TabIndex = 3;
            this.level2.Text = "2";
            this.level2.UseVisualStyleBackColor = true;
            // 
            // level1
            // 
            this.level1.AutoSize = true;
            this.level1.Location = new System.Drawing.Point(12, 6);
            this.level1.Name = "level1";
            this.level1.Size = new System.Drawing.Size(31, 17);
            this.level1.TabIndex = 2;
            this.level1.Text = "1";
            this.level1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBox2);
            this.splitContainer1.Size = new System.Drawing.Size(734, 130);
            this.splitContainer1.SplitterDistance = 363;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.MaxLength = 10000000;
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(363, 130);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.MaxLength = 10000000;
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(367, 130);
            this.textBox2.TabIndex = 2;
            // 
            // EditPairForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(734, 177);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.bottomPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditPairForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit pair";
            this.Shown += new System.EventHandler(this.EditPairForm_Shown);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.RadioButton level0;
        private System.Windows.Forms.RadioButton level3;
        private System.Windows.Forms.RadioButton level2;
        private System.Windows.Forms.RadioButton level1;
        private System.Windows.Forms.Button cancelButton;
    }
}