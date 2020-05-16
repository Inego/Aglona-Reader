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
            this.exportButton.Location = new System.Drawing.Point(143, 62);
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
            this.cancelButton.Location = new System.Drawing.Point(224, 62);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ExportHTMLForm
            // 
            this.AcceptButton = this.exportButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(443, 104);
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
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.TextBox exportFileName;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Button selectExportFileButton;

        #endregion
    }
}