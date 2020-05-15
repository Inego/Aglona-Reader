namespace AglonaReader
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.programVersionLabel = new System.Windows.Forms.Label();
            this.releaseDateLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ListOfContributorsLabel = new System.Windows.Forms.Label();
            this.bugFixedAndImprovementsLabel = new System.Windows.Forms.Label();
            this.otherContributorsList = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // programVersionLabel
            // 
            resources.ApplyResources(this.programVersionLabel, "programVersionLabel");
            this.programVersionLabel.Name = "programVersionLabel";
            // 
            // releaseDateLabel
            // 
            resources.ApplyResources(this.releaseDateLabel, "releaseDateLabel");
            this.releaseDateLabel.Name = "releaseDateLabel";
            // 
            // colorLabel
            // 
            resources.ApplyResources(this.colorLabel, "colorLabel");
            this.colorLabel.Name = "colorLabel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // ListOfContributorsLabel
            // 
            resources.ApplyResources(this.ListOfContributorsLabel, "ListOfContributorsLabel");
            this.ListOfContributorsLabel.Name = "ListOfContributorsLabel";
            // 
            // bugFixedAndImprovementsLabel
            // 
            resources.ApplyResources(this.bugFixedAndImprovementsLabel, "bugFixedAndImprovementsLabel");
            this.bugFixedAndImprovementsLabel.Name = "bugFixedAndImprovementsLabel";
            // 
            // otherContributorsList
            // 
            resources.ApplyResources(this.otherContributorsList, "otherContributorsList");
            this.otherContributorsList.Name = "otherContributorsList";
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.otherContributorsList);
            this.Controls.Add(this.bugFixedAndImprovementsLabel);
            this.Controls.Add(this.ListOfContributorsLabel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.colorLabel);
            this.Controls.Add(this.releaseDateLabel);
            this.Controls.Add(this.programVersionLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AboutForm_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AboutForm_MouseMove);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label bugFixedAndImprovementsLabel;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label ListOfContributorsLabel;
        private System.Windows.Forms.Label otherContributorsList;
        private System.Windows.Forms.Label programVersionLabel;
        private System.Windows.Forms.Label releaseDateLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        #endregion
    }
}