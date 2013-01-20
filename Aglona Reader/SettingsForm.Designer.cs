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
            ((System.ComponentModel.ISupportInitialize)(this.fontSizeTrackBar)).BeginInit();
            this.fontGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessBar)).BeginInit();
            this.SuspendLayout();
            // 
            // fontNameLabel
            // 
            this.fontNameLabel.AutoSize = true;
            this.fontNameLabel.Location = new System.Drawing.Point(236, 20);
            this.fontNameLabel.Name = "fontNameLabel";
            this.fontNameLabel.Size = new System.Drawing.Size(35, 13);
            this.fontNameLabel.TabIndex = 2;
            this.fontNameLabel.Text = "Name";
            // 
            // fontsComboBox
            // 
            this.fontsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontsComboBox.FormattingEnabled = true;
            this.fontsComboBox.Location = new System.Drawing.Point(15, 17);
            this.fontsComboBox.Name = "fontsComboBox";
            this.fontsComboBox.Size = new System.Drawing.Size(215, 21);
            this.fontsComboBox.TabIndex = 5;
            this.fontsComboBox.SelectedIndexChanged += new System.EventHandler(this.fontsComboBox_SelectedIndexChanged);
            // 
            // fontSizeTrackBar
            // 
            this.fontSizeTrackBar.LargeChange = 100;
            this.fontSizeTrackBar.Location = new System.Drawing.Point(6, 44);
            this.fontSizeTrackBar.Maximum = 1000;
            this.fontSizeTrackBar.Name = "fontSizeTrackBar";
            this.fontSizeTrackBar.Size = new System.Drawing.Size(678, 45);
            this.fontSizeTrackBar.TabIndex = 6;
            this.fontSizeTrackBar.TickFrequency = 10;
            this.fontSizeTrackBar.Scroll += new System.EventHandler(this.fontSizeTrackBar_Scroll);
            // 
            // fontGroupBox
            // 
            this.fontGroupBox.Controls.Add(this.fontsComboBox);
            this.fontGroupBox.Controls.Add(this.fontSizeTrackBar);
            this.fontGroupBox.Controls.Add(this.fontNameLabel);
            this.fontGroupBox.Location = new System.Drawing.Point(12, 92);
            this.fontGroupBox.Name = "fontGroupBox";
            this.fontGroupBox.Size = new System.Drawing.Size(693, 100);
            this.fontGroupBox.TabIndex = 7;
            this.fontGroupBox.TabStop = false;
            this.fontGroupBox.Text = "Font";
            // 
            // highlightFirstWordsCheckBox
            // 
            this.highlightFirstWordsCheckBox.AutoSize = true;
            this.highlightFirstWordsCheckBox.Location = new System.Drawing.Point(9, 19);
            this.highlightFirstWordsCheckBox.Name = "highlightFirstWordsCheckBox";
            this.highlightFirstWordsCheckBox.Size = new System.Drawing.Size(79, 17);
            this.highlightFirstWordsCheckBox.TabIndex = 8;
            this.highlightFirstWordsCheckBox.Text = "First Words";
            this.highlightFirstWordsCheckBox.UseVisualStyleBackColor = true;
            this.highlightFirstWordsCheckBox.CheckedChanged += new System.EventHandler(this.highlightFirstWordsCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.brightnessBar);
            this.groupBox1.Controls.Add(this.highlightFragmentsCheckBox);
            this.groupBox1.Controls.Add(this.highlightFirstWordsCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(693, 74);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color Highlighting";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Brightness";
            // 
            // brightnessBar
            // 
            this.brightnessBar.Location = new System.Drawing.Point(105, 26);
            this.brightnessBar.Maximum = 1000;
            this.brightnessBar.Minimum = 600;
            this.brightnessBar.Name = "brightnessBar";
            this.brightnessBar.Size = new System.Drawing.Size(576, 45);
            this.brightnessBar.TabIndex = 3;
            this.brightnessBar.TickFrequency = 3;
            this.brightnessBar.Value = 950;
            this.brightnessBar.Scroll += new System.EventHandler(this.brightnessBar_Scroll);
            // 
            // highlightFragmentsCheckBox
            // 
            this.highlightFragmentsCheckBox.AutoSize = true;
            this.highlightFragmentsCheckBox.Location = new System.Drawing.Point(9, 42);
            this.highlightFragmentsCheckBox.Name = "highlightFragmentsCheckBox";
            this.highlightFragmentsCheckBox.Size = new System.Drawing.Size(75, 17);
            this.highlightFragmentsCheckBox.TabIndex = 9;
            this.highlightFragmentsCheckBox.Text = "Fragments";
            this.highlightFragmentsCheckBox.UseVisualStyleBackColor = true;
            this.highlightFragmentsCheckBox.CheckedChanged += new System.EventHandler(this.highlightFragmentsCheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 204);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Reading Mode";
            // 
            // readingModeComboBox
            // 
            this.readingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.readingModeComboBox.FormattingEnabled = true;
            this.readingModeComboBox.Items.AddRange(new object[] {
            "Normal",
            "Alternating"});
            this.readingModeComboBox.Location = new System.Drawing.Point(98, 201);
            this.readingModeComboBox.Name = "readingModeComboBox";
            this.readingModeComboBox.Size = new System.Drawing.Size(140, 21);
            this.readingModeComboBox.TabIndex = 11;
            this.readingModeComboBox.SelectedIndexChanged += new System.EventHandler(this.readingModeComboBox_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 278);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SettingsForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.fontSizeTrackBar)).EndInit();
            this.fontGroupBox.ResumeLayout(false);
            this.fontGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label fontNameLabel;
        private System.Windows.Forms.ComboBox fontsComboBox;
        private System.Windows.Forms.TrackBar fontSizeTrackBar;
        private System.Windows.Forms.GroupBox fontGroupBox;
        private System.Windows.Forms.CheckBox highlightFirstWordsCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar brightnessBar;
        private System.Windows.Forms.CheckBox highlightFragmentsCheckBox;
        private Label label2;
        private ComboBox readingModeComboBox;

    }
}