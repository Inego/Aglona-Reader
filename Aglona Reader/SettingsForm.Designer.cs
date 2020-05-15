using System.Windows.Forms;

namespace AglonaReader
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.fontNameLabel = new System.Windows.Forms.Label();
            this.fontsComboBox = new System.Windows.Forms.ComboBox();
            this.fontSizeTrackBar = new System.Windows.Forms.TrackBar();
            this.fontGroupBox = new System.Windows.Forms.GroupBox();
            this.highlightFirstWordsCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.brightnessBar = new System.Windows.Forms.TrackBar();
            this.highlightFragmentsCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.readingModeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.alternatingColorSchemeComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize) (this.fontSizeTrackBar)).BeginInit();
            this.fontGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.brightnessBar)).BeginInit();
            this.SuspendLayout();
            // 
            // fontNameLabel
            // 
            resources.ApplyResources(this.fontNameLabel, "fontNameLabel");
            this.fontNameLabel.Name = "fontNameLabel";
            // 
            // fontsComboBox
            // 
            this.fontsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontsComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.fontsComboBox, "fontsComboBox");
            this.fontsComboBox.Name = "fontsComboBox";
            this.fontsComboBox.SelectedIndexChanged += new System.EventHandler(this.fontsComboBox_SelectedIndexChanged);
            // 
            // fontSizeTrackBar
            // 
            this.fontSizeTrackBar.LargeChange = 100;
            resources.ApplyResources(this.fontSizeTrackBar, "fontSizeTrackBar");
            this.fontSizeTrackBar.Maximum = 1000;
            this.fontSizeTrackBar.Name = "fontSizeTrackBar";
            this.fontSizeTrackBar.TickFrequency = 10;
            this.fontSizeTrackBar.Scroll += new System.EventHandler(this.fontSizeTrackBar_Scroll);
            // 
            // fontGroupBox
            // 
            this.fontGroupBox.Controls.Add(this.fontsComboBox);
            this.fontGroupBox.Controls.Add(this.fontSizeTrackBar);
            this.fontGroupBox.Controls.Add(this.fontNameLabel);
            resources.ApplyResources(this.fontGroupBox, "fontGroupBox");
            this.fontGroupBox.Name = "fontGroupBox";
            this.fontGroupBox.TabStop = false;
            // 
            // highlightFirstWordsCheckBox
            // 
            resources.ApplyResources(this.highlightFirstWordsCheckBox, "highlightFirstWordsCheckBox");
            this.highlightFirstWordsCheckBox.Name = "highlightFirstWordsCheckBox";
            this.highlightFirstWordsCheckBox.UseVisualStyleBackColor = true;
            this.highlightFirstWordsCheckBox.CheckedChanged += new System.EventHandler(this.highlightFirstWordsCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.brightnessBar);
            this.groupBox1.Controls.Add(this.highlightFragmentsCheckBox);
            this.groupBox1.Controls.Add(this.highlightFirstWordsCheckBox);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // brightnessBar
            // 
            resources.ApplyResources(this.brightnessBar, "brightnessBar");
            this.brightnessBar.Maximum = 1000;
            this.brightnessBar.Minimum = 600;
            this.brightnessBar.Name = "brightnessBar";
            this.brightnessBar.TickFrequency = 3;
            this.brightnessBar.Value = 950;
            this.brightnessBar.Scroll += new System.EventHandler(this.brightnessBar_Scroll);
            // 
            // highlightFragmentsCheckBox
            // 
            resources.ApplyResources(this.highlightFragmentsCheckBox, "highlightFragmentsCheckBox");
            this.highlightFragmentsCheckBox.Name = "highlightFragmentsCheckBox";
            this.highlightFragmentsCheckBox.UseVisualStyleBackColor = true;
            this.highlightFragmentsCheckBox.CheckedChanged += new System.EventHandler(this.highlightFragmentsCheckBox_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // readingModeComboBox
            // 
            this.readingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.readingModeComboBox.FormattingEnabled = true;
            this.readingModeComboBox.Items.AddRange(new object[] {resources.GetString("readingModeComboBox.Items"), resources.GetString("readingModeComboBox.Items1"), resources.GetString("readingModeComboBox.Items2")});
            resources.ApplyResources(this.readingModeComboBox, "readingModeComboBox");
            this.readingModeComboBox.Name = "readingModeComboBox";
            this.readingModeComboBox.SelectedIndexChanged += new System.EventHandler(this.readingModeComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // alternatingColorSchemeComboBox
            // 
            this.alternatingColorSchemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.alternatingColorSchemeComboBox.FormattingEnabled = true;
            this.alternatingColorSchemeComboBox.Items.AddRange(new object[] {resources.GetString("alternatingColorSchemeComboBox.Items"), resources.GetString("alternatingColorSchemeComboBox.Items1")});
            resources.ApplyResources(this.alternatingColorSchemeComboBox, "alternatingColorSchemeComboBox");
            this.alternatingColorSchemeComboBox.Name = "alternatingColorSchemeComboBox";
            this.alternatingColorSchemeComboBox.SelectedIndexChanged += new System.EventHandler(this.alternatingColorSchemeComboBox_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.alternatingColorSchemeComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.readingModeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fontGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SettingsForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize) (this.fontSizeTrackBar)).EndInit();
            this.fontGroupBox.ResumeLayout(false);
            this.fontGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.brightnessBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ComboBox alternatingColorSchemeComboBox;
        private System.Windows.Forms.TrackBar brightnessBar;
        private System.Windows.Forms.GroupBox fontGroupBox;
        private System.Windows.Forms.Label fontNameLabel;
        private System.Windows.Forms.ComboBox fontsComboBox;
        private System.Windows.Forms.TrackBar fontSizeTrackBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox highlightFirstWordsCheckBox;
        private System.Windows.Forms.CheckBox highlightFragmentsCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox readingModeComboBox;

        #endregion
    }
}