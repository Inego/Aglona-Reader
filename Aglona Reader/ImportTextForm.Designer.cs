namespace AglonaReader
{
    partial class ImportTextForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportTextForm));
            this.fileName1 = new System.Windows.Forms.TextBox();
            this.selectFile1Button = new System.Windows.Forms.Button();
            this.selectFile2Button = new System.Windows.Forms.Button();
            this.fileName2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // fileName1
            // 
            this.fileName1.Location = new System.Drawing.Point(12, 24);
            this.fileName1.Name = "fileName1";
            this.fileName1.Size = new System.Drawing.Size(443, 20);
            this.fileName1.TabIndex = 0;
            // 
            // selectFile1Button
            // 
            this.selectFile1Button.Location = new System.Drawing.Point(461, 24);
            this.selectFile1Button.Name = "selectFile1Button";
            this.selectFile1Button.Size = new System.Drawing.Size(25, 20);
            this.selectFile1Button.TabIndex = 1;
            this.selectFile1Button.Text = "...";
            this.selectFile1Button.UseVisualStyleBackColor = true;
            this.selectFile1Button.Click += new System.EventHandler(this.selectFile1Button_Click);
            // 
            // selectFile2Button
            // 
            this.selectFile2Button.Location = new System.Drawing.Point(461, 67);
            this.selectFile2Button.Name = "selectFile2Button";
            this.selectFile2Button.Size = new System.Drawing.Size(25, 20);
            this.selectFile2Button.TabIndex = 3;
            this.selectFile2Button.Text = "...";
            this.selectFile2Button.UseVisualStyleBackColor = true;
            this.selectFile2Button.Click += new System.EventHandler(this.selectFile2Button_Click);
            // 
            // fileName2
            // 
            this.fileName2.Location = new System.Drawing.Point(12, 67);
            this.fileName2.Name = "fileName2";
            this.fileName2.Size = new System.Drawing.Size(443, 20);
            this.fileName2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Translation:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Source:";
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(170, 104);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 6;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(251, 104);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ImportTextForm
            // 
            this.AcceptButton = this.importButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(497, 148);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectFile2Button);
            this.Controls.Add(this.fileName2);
            this.Controls.Add(this.selectFile1Button);
            this.Controls.Add(this.fileName1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportTextForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import text from source files";
            this.Shown += new System.EventHandler(this.ImportTextForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fileName1;
        private System.Windows.Forms.Button selectFile1Button;
        private System.Windows.Forms.Button selectFile2Button;
        private System.Windows.Forms.TextBox fileName2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button cancelButton;
    }
}