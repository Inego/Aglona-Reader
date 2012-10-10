namespace AglonaReader
{
    partial class OpenRecentForm
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
            this.components = new System.ComponentModel.Container();
            this.formCancelButton = new System.Windows.Forms.Button();
            this.formOKButton = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.removeAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // formCancelButton
            // 
            this.formCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.formCancelButton.Location = new System.Drawing.Point(402, 405);
            this.formCancelButton.Name = "formCancelButton";
            this.formCancelButton.Size = new System.Drawing.Size(50, 23);
            this.formCancelButton.TabIndex = 2;
            this.formCancelButton.Text = "Cancel";
            this.formCancelButton.UseVisualStyleBackColor = true;
            // 
            // formOKButton
            // 
            this.formOKButton.Location = new System.Drawing.Point(350, 405);
            this.formOKButton.Name = "formOKButton";
            this.formOKButton.Size = new System.Drawing.Size(46, 23);
            this.formOKButton.TabIndex = 1;
            this.formOKButton.Text = "OK";
            this.formOKButton.UseVisualStyleBackColor = true;
            this.formOKButton.Click += new System.EventHandler(this.formOKButton_Click);
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(12, 14);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(440, 381);
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 402);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Remove";
            this.toolTip1.SetToolTip(this.button1, "Remove the selected file from the list");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // removeAllButton
            // 
            this.removeAllButton.Location = new System.Drawing.Point(76, 402);
            this.removeAllButton.Name = "removeAllButton";
            this.removeAllButton.Size = new System.Drawing.Size(75, 23);
            this.removeAllButton.TabIndex = 4;
            this.removeAllButton.Text = "Remove All";
            this.toolTip1.SetToolTip(this.removeAllButton, "Remove all files from the list");
            this.removeAllButton.UseVisualStyleBackColor = true;
            this.removeAllButton.Click += new System.EventHandler(this.removeAllButton_Click);
            // 
            // OpenRecentForm
            // 
            this.AcceptButton = this.formOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.formCancelButton;
            this.ClientSize = new System.Drawing.Size(464, 440);
            this.Controls.Add(this.removeAllButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.formOKButton);
            this.Controls.Add(this.formCancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpenRecentForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open recent files";
            this.Shown += new System.EventHandler(this.OpenRecentForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button formCancelButton;
        private System.Windows.Forms.Button formOKButton;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button removeAllButton;
    }
}