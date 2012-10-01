namespace AglonaReader
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Drawing.StringFormat stringFormat1 = new System.Drawing.StringFormat();
            AglonaReader.ParallelText parallelText1 = new AglonaReader.ParallelText();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reverseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.structureleftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.structurerightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highlightFramgentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highlightFirstWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pTC = new AglonaReader.ParallelTextControl();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.bookToolStripMenuItem,
            this.pairToolStripMenuItem,
            this.visualsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(688, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // bookToolStripMenuItem
            // 
            this.bookToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationToolStripMenuItem,
            this.toolStripSeparator1,
            this.reverseToolStripMenuItem,
            this.toolStripSeparator2,
            this.structureleftToolStripMenuItem,
            this.structurerightToolStripMenuItem,
            this.toolStripSeparator3,
            this.editModeToolStripMenuItem});
            this.bookToolStripMenuItem.Name = "bookToolStripMenuItem";
            this.bookToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F2)));
            this.bookToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.bookToolStripMenuItem.Text = "Book";
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.informationToolStripMenuItem.Text = "Information";
            this.informationToolStripMenuItem.Click += new System.EventHandler(this.informationToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // reverseToolStripMenuItem
            // 
            this.reverseToolStripMenuItem.CheckOnClick = true;
            this.reverseToolStripMenuItem.Name = "reverseToolStripMenuItem";
            this.reverseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.reverseToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.reverseToolStripMenuItem.Text = "Reverse";
            this.reverseToolStripMenuItem.Click += new System.EventHandler(this.reverseToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(197, 6);
            // 
            // structureleftToolStripMenuItem
            // 
            this.structureleftToolStripMenuItem.Name = "structureleftToolStripMenuItem";
            this.structureleftToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F1)));
            this.structureleftToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.structureleftToolStripMenuItem.Text = "Structure (left)";
            this.structureleftToolStripMenuItem.Click += new System.EventHandler(this.structureleftToolStripMenuItem_Click);
            // 
            // structurerightToolStripMenuItem
            // 
            this.structurerightToolStripMenuItem.Name = "structurerightToolStripMenuItem";
            this.structurerightToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F2)));
            this.structurerightToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.structurerightToolStripMenuItem.Text = "Structure (right)";
            this.structurerightToolStripMenuItem.Click += new System.EventHandler(this.structurerightToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(197, 6);
            // 
            // editModeToolStripMenuItem
            // 
            this.editModeToolStripMenuItem.CheckOnClick = true;
            this.editModeToolStripMenuItem.Name = "editModeToolStripMenuItem";
            this.editModeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.editModeToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.editModeToolStripMenuItem.Text = "Edit mode";
            this.editModeToolStripMenuItem.Click += new System.EventHandler(this.editModeToolStripMenuItem_Click);
            // 
            // pairToolStripMenuItem
            // 
            this.pairToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.editToolStripMenuItem});
            this.pairToolStripMenuItem.Name = "pairToolStripMenuItem";
            this.pairToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.pairToolStripMenuItem.Text = "Pair";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.importToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.importToolStripMenuItem.Text = "Import...";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.editToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // visualsToolStripMenuItem
            // 
            this.visualsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highlightFramgentsToolStripMenuItem,
            this.highlightFirstWordsToolStripMenuItem});
            this.visualsToolStripMenuItem.Name = "visualsToolStripMenuItem";
            this.visualsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemOpenBrackets)));
            this.visualsToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.visualsToolStripMenuItem.Text = "Visuals";
            // 
            // highlightFramgentsToolStripMenuItem
            // 
            this.highlightFramgentsToolStripMenuItem.CheckOnClick = true;
            this.highlightFramgentsToolStripMenuItem.Name = "highlightFramgentsToolStripMenuItem";
            this.highlightFramgentsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.highlightFramgentsToolStripMenuItem.Text = "Highlight fragments";
            this.highlightFramgentsToolStripMenuItem.Click += new System.EventHandler(this.highlightFramgentsToolStripMenuItem_Click);
            // 
            // highlightFirstWordsToolStripMenuItem
            // 
            this.highlightFirstWordsToolStripMenuItem.CheckOnClick = true;
            this.highlightFirstWordsToolStripMenuItem.Name = "highlightFirstWordsToolStripMenuItem";
            this.highlightFirstWordsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.highlightFirstWordsToolStripMenuItem.Text = "Highlight first words";
            this.highlightFirstWordsToolStripMenuItem.Click += new System.EventHandler(this.highlightFirstWordsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // pTC
            // 
            this.pTC.BackColor = System.Drawing.SystemColors.Info;
            this.pTC.Brightness = 0.97D;
            this.pTC.CurrentPair = 0;
            this.pTC.DebugString = null;
            this.pTC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTC.EditMode = false;
            this.pTC.EditWhenNipped = false;
            this.pTC.FirstRenderedPair = 0;
            stringFormat1.Alignment = System.Drawing.StringAlignment.Near;
            stringFormat1.FormatFlags = ((System.Drawing.StringFormatFlags)(((System.Drawing.StringFormatFlags.FitBlackBox | System.Drawing.StringFormatFlags.LineLimit)
                        | System.Drawing.StringFormatFlags.NoClip)));
            stringFormat1.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
            stringFormat1.LineAlignment = System.Drawing.StringAlignment.Near;
            stringFormat1.Trimming = System.Drawing.StringTrimming.None;
            this.pTC.GT = stringFormat1;
            this.pTC.HighlightedPair = 0;
            this.pTC.HighlightFirstWords = true;
            this.pTC.HighlightFragments = true;
            this.pTC.LastFullScreenLine = 0;
            this.pTC.LastMouseX = -1;
            this.pTC.LastMouseY = -1;
            this.pTC.LastRenderedPair = 0;
            this.pTC.LineHeight = 28;
            this.pTC.Location = new System.Drawing.Point(0, 24);
            this.pTC.Modified = false;
            this.pTC.MouseCurrentWord = null;
            this.pTC.MousePressed = false;
            this.pTC.MouseStatus = ((byte)(0));
            this.pTC.Name = "pTC";
            this.pTC.NaturalDividerPosition1 = 0;
            this.pTC.NaturalDividerPosition1W = null;
            this.pTC.NaturalDividerPosition2 = 0;
            this.pTC.NaturalDividerPosition2W = null;
            this.pTC.NumberOfScreenLines = 0;
            this.pTC.PanelMargin = 10;
            this.pTC.PrimaryBG = null;
            parallelText1.Author1 = null;
            parallelText1.Author2 = null;
            parallelText1.FileName = null;
            parallelText1.Info = null;
            parallelText1.Info1 = null;
            parallelText1.Info2 = null;
            parallelText1.Lang1 = null;
            parallelText1.Lang2 = null;
            parallelText1.Title1 = null;
            parallelText1.Title2 = null;
            this.pTC.PText = parallelText1;
            this.pTC.Reversed = false;
            this.pTC.RightPosition = 445;
            this.pTC.SecondaryBG = null;
            this.pTC.Side1Set = false;
            this.pTC.Side2Set = false;
            this.pTC.Size = new System.Drawing.Size(688, 240);
            this.pTC.SpaceLength = 6;
            this.pTC.SplitterMoveOffset = 0;
            this.pTC.SplitterPosition = 445;
            this.pTC.SplitterRatio = 0F;
            this.pTC.SplitterWidth = 4;
            this.pTC.TabIndex = 3;
            this.pTC.TextFont = new System.Drawing.Font("Times New Roman", 18F);
            this.pTC.VMargin = 3;
            this.pTC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pTC_KeyDown);
            this.pTC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pTC_KeyPress);
            this.pTC.MouseDown += new System.Windows.Forms.MouseEventHandler(this.parallelTextControl_MouseDown);
            this.pTC.MouseLeave += new System.EventHandler(this.parallelTextControl_MouseLeave);
            this.pTC.MouseMove += new System.Windows.Forms.MouseEventHandler(this.parallelTextControl_MouseMove);
            this.pTC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.parallelTextControl_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 264);
            this.Controls.Add(this.pTC);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Aglona Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private ParallelTextControl pTC;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reverseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visualsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highlightFramgentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highlightFirstWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem structureleftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem structurerightToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem editModeToolStripMenuItem;

    }
}

