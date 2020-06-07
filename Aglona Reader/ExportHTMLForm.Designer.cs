namespace AglonaReader
{
    partial class ExportHtmlForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportHtmlForm));
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.exportFileName = new System.Windows.Forms.TextBox();
            this.selectExportFileButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.StyleGroupBox = new System.Windows.Forms.GroupBox();
            this.TableColorsCb = new System.Windows.Forms.CheckBox();
            this.TableNumbersCb = new System.Windows.Forms.CheckBox();
            this.TableBordersCb = new System.Windows.Forms.CheckBox();
            this.AlternatingStyleRb = new System.Windows.Forms.RadioButton();
            this.TwoColumnStyleRb = new System.Windows.Forms.RadioButton();
            this.StyleGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(12, 9);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(52, 13);
            this.fileNameLabel.TabIndex = 0;
            this.fileNameLabel.Text = "File name";
            // 
            // exportFileName
            // 
            this.exportFileName.Location = new System.Drawing.Point(15, 25);
            this.exportFileName.Name = "exportFileName";
            this.exportFileName.Size = new System.Drawing.Size(383, 20);
            this.exportFileName.TabIndex = 1;
            // 
            // selectExportFileButton
            // 
            this.selectExportFileButton.Location = new System.Drawing.Point(404, 25);
            this.selectExportFileButton.Name = "selectExportFileButton";
            this.selectExportFileButton.Size = new System.Drawing.Size(27, 20);
            this.selectExportFileButton.TabIndex = 2;
            this.selectExportFileButton.Text = "...";
            this.selectExportFileButton.UseVisualStyleBackColor = true;
            this.selectExportFileButton.Click += new System.EventHandler(this.selectExportFileButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(275, 127);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 3;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(356, 127);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // StyleGroupBox
            // 
            this.StyleGroupBox.Controls.Add(this.TableColorsCb);
            this.StyleGroupBox.Controls.Add(this.TableNumbersCb);
            this.StyleGroupBox.Controls.Add(this.TableBordersCb);
            this.StyleGroupBox.Controls.Add(this.AlternatingStyleRb);
            this.StyleGroupBox.Controls.Add(this.TwoColumnStyleRb);
            this.StyleGroupBox.Location = new System.Drawing.Point(15, 51);
            this.StyleGroupBox.Name = "StyleGroupBox";
            this.StyleGroupBox.Size = new System.Drawing.Size(416, 70);
            this.StyleGroupBox.TabIndex = 5;
            this.StyleGroupBox.TabStop = false;
            this.StyleGroupBox.Text = "Style";
            // 
            // TableColorsCb
            // 
            this.TableColorsCb.AutoSize = true;
            this.TableColorsCb.Location = new System.Drawing.Point(293, 19);
            this.TableColorsCb.Name = "TableColorsCb";
            this.TableColorsCb.Size = new System.Drawing.Size(55, 17);
            this.TableColorsCb.TabIndex = 4;
            this.TableColorsCb.Text = "Colors";
            this.TableColorsCb.UseVisualStyleBackColor = true;
            // 
            // TableNumbersCb
            // 
            this.TableNumbersCb.AutoSize = true;
            this.TableNumbersCb.Location = new System.Drawing.Point(219, 19);
            this.TableNumbersCb.Name = "TableNumbersCb";
            this.TableNumbersCb.Size = new System.Drawing.Size(68, 17);
            this.TableNumbersCb.TabIndex = 3;
            this.TableNumbersCb.Text = "Numbers";
            this.TableNumbersCb.UseVisualStyleBackColor = true;
            // 
            // TableBordersCb
            // 
            this.TableBordersCb.AutoSize = true;
            this.TableBordersCb.Location = new System.Drawing.Point(151, 19);
            this.TableBordersCb.Name = "TableBordersCb";
            this.TableBordersCb.Size = new System.Drawing.Size(62, 17);
            this.TableBordersCb.TabIndex = 2;
            this.TableBordersCb.Text = "Borders";
            this.TableBordersCb.UseVisualStyleBackColor = true;
            // 
            // AlternatingStyleRb
            // 
            this.AlternatingStyleRb.AutoSize = true;
            this.AlternatingStyleRb.Location = new System.Drawing.Point(6, 43);
            this.AlternatingStyleRb.Name = "AlternatingStyleRb";
            this.AlternatingStyleRb.Size = new System.Drawing.Size(75, 17);
            this.AlternatingStyleRb.TabIndex = 1;
            this.AlternatingStyleRb.TabStop = true;
            this.AlternatingStyleRb.Text = "Alternating";
            this.AlternatingStyleRb.UseVisualStyleBackColor = true;
            // 
            // TwoColumnStyleRb
            // 
            this.TwoColumnStyleRb.AutoSize = true;
            this.TwoColumnStyleRb.Location = new System.Drawing.Point(6, 19);
            this.TwoColumnStyleRb.Name = "TwoColumnStyleRb";
            this.TwoColumnStyleRb.Size = new System.Drawing.Size(99, 17);
            this.TwoColumnStyleRb.TabIndex = 0;
            this.TwoColumnStyleRb.TabStop = true;
            this.TwoColumnStyleRb.Text = "2 Column Table";
            this.TwoColumnStyleRb.UseVisualStyleBackColor = true;
            // 
            // ExportHtmlForm
            // 
            this.AcceptButton = this.exportButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(443, 159);
            this.Controls.Add(this.StyleGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.selectExportFileButton);
            this.Controls.Add(this.exportFileName);
            this.Controls.Add(this.fileNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportHtmlForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export to HTML";
            this.StyleGroupBox.ResumeLayout(false);
            this.StyleGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.RadioButton AlternatingStyleRb;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.TextBox exportFileName;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Button selectExportFileButton;
        private System.Windows.Forms.GroupBox StyleGroupBox;
        private System.Windows.Forms.CheckBox TableBordersCb;
        private System.Windows.Forms.CheckBox TableColorsCb;
        private System.Windows.Forms.CheckBox TableNumbersCb;
        private System.Windows.Forms.RadioButton TwoColumnStyleRb;

        #endregion
    }
}