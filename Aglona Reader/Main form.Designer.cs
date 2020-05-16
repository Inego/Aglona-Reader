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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Drawing.StringFormat stringFormat1 = new System.Drawing.StringFormat();
            AglonaReader.ParallelText parallelText1 = new AglonaReader.ParallelText();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLeftTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRightTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hTMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.reverseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reverseContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.structureLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.structureRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.editModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.insertBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertPairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startpauseStopwatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetStopwatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alternatingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.showGoogleTranslatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitScreenVerticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aglonaReaderSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paraBooksMakerSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.ssPosition = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssPositionPercent = new System.Windows.Forms.ToolStripStatusLabel();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pTC = new AglonaReader.ParallelTextControl();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.mainMenu.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.fileToolStripMenuItem, this.bookToolStripMenuItem, this.pairToolStripMenuItem, this.statsToolStripMenuItem, this.settingsToolStripMenuItem, this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.mainMenu.Size = new System.Drawing.Size(688, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.newToolStripMenuItem, this.toolStripSeparator4, this.openToolStripMenuItem, this.openRecentToolStripMenuItem, this.toolStripSeparator6, this.saveToolStripMenuItem, this.saveAsToolStripMenuItem, this.toolStripMenuItem2, this.exportToolStripMenuItem, this.toolStripSeparator5, this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(220, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openRecentToolStripMenuItem
            // 
            this.openRecentToolStripMenuItem.Name = "openRecentToolStripMenuItem";
            this.openRecentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.O)));
            this.openRecentToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.openRecentToolStripMenuItem.Text = "Open recent";
            this.openRecentToolStripMenuItem.Click += new System.EventHandler(this.OpenRecentToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(220, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(220, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.exportLeftTextToolStripMenuItem, this.exportRightTextToolStripMenuItem, this.hTMLToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // exportLeftTextToolStripMenuItem
            // 
            this.exportLeftTextToolStripMenuItem.Name = "exportLeftTextToolStripMenuItem";
            this.exportLeftTextToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.exportLeftTextToolStripMenuItem.Text = "Left text to TXT";
            this.exportLeftTextToolStripMenuItem.Click += new System.EventHandler(this.ExportLeftTextToolStripMenuItem_Click);
            // 
            // exportRightTextToolStripMenuItem
            // 
            this.exportRightTextToolStripMenuItem.Name = "exportRightTextToolStripMenuItem";
            this.exportRightTextToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.exportRightTextToolStripMenuItem.Text = "Right text to TXT";
            this.exportRightTextToolStripMenuItem.Click += new System.EventHandler(this.ExportRightTextToolStripMenuItem_Click);
            // 
            // hTMLToolStripMenuItem
            // 
            this.hTMLToolStripMenuItem.Name = "hTMLToolStripMenuItem";
            this.hTMLToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.hTMLToolStripMenuItem.Text = "Book to HTML";
            this.hTMLToolStripMenuItem.Click += new System.EventHandler(this.hTMLToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(220, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // bookToolStripMenuItem
            // 
            this.bookToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.informationToolStripMenuItem, this.toolStripSeparator1, this.reverseToolStripMenuItem, this.reverseContentsToolStripMenuItem, this.toolStripSeparator2, this.structureLeftToolStripMenuItem, this.structureRightToolStripMenuItem, this.toolStripSeparator3, this.findToolStripMenuItem, this.toolStripMenuItem1, this.editModeToolStripMenuItem});
            this.bookToolStripMenuItem.Name = "bookToolStripMenuItem";
            this.bookToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F2)));
            this.bookToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.bookToolStripMenuItem.Text = "Book";
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.informationToolStripMenuItem.Text = "Information";
            this.informationToolStripMenuItem.Click += new System.EventHandler(this.informationToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(207, 6);
            // 
            // reverseToolStripMenuItem
            // 
            this.reverseToolStripMenuItem.CheckOnClick = true;
            this.reverseToolStripMenuItem.Name = "reverseToolStripMenuItem";
            this.reverseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.reverseToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.reverseToolStripMenuItem.Text = "Reverse";
            this.reverseToolStripMenuItem.Click += new System.EventHandler(this.reverseToolStripMenuItem_Click);
            // 
            // reverseContentsToolStripMenuItem
            // 
            this.reverseContentsToolStripMenuItem.Name = "reverseContentsToolStripMenuItem";
            this.reverseContentsToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.reverseContentsToolStripMenuItem.Text = "Reverse contents";
            this.reverseContentsToolStripMenuItem.Click += new System.EventHandler(this.ReverseContentsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(207, 6);
            // 
            // structureLeftToolStripMenuItem
            // 
            this.structureLeftToolStripMenuItem.Name = "structureLeftToolStripMenuItem";
            this.structureLeftToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F1)));
            this.structureLeftToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.structureLeftToolStripMenuItem.Text = "Structure (left)";
            this.structureLeftToolStripMenuItem.Click += new System.EventHandler(this.structureLeftToolStripMenuItem_Click);
            // 
            // structureRightToolStripMenuItem
            // 
            this.structureRightToolStripMenuItem.Name = "structureRightToolStripMenuItem";
            this.structureRightToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F2)));
            this.structureRightToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.structureRightToolStripMenuItem.Text = "Structure (right)";
            this.structureRightToolStripMenuItem.Click += new System.EventHandler(this.structureRightToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(207, 6);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.findToolStripMenuItem.Text = "Find...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.FindToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(207, 6);
            // 
            // editModeToolStripMenuItem
            // 
            this.editModeToolStripMenuItem.CheckOnClick = true;
            this.editModeToolStripMenuItem.Name = "editModeToolStripMenuItem";
            this.editModeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.editModeToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.editModeToolStripMenuItem.Text = "Edit mode";
            this.editModeToolStripMenuItem.Click += new System.EventHandler(this.editModeToolStripMenuItem_Click);
            // 
            // pairToolStripMenuItem
            // 
            this.pairToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.importToolStripMenuItem, this.editToolStripMenuItem, this.toolStripMenuItem4, this.insertBeforeToolStripMenuItem, this.insertPairToolStripMenuItem, this.deletePairToolStripMenuItem});
            this.pairToolStripMenuItem.Name = "pairToolStripMenuItem";
            this.pairToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.pairToolStripMenuItem.Text = "Pair";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.importToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.importToolStripMenuItem.Text = "Import...";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.editToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(203, 6);
            // 
            // insertBeforeToolStripMenuItem
            // 
            this.insertBeforeToolStripMenuItem.Name = "insertBeforeToolStripMenuItem";
            this.insertBeforeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Insert)));
            this.insertBeforeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.insertBeforeToolStripMenuItem.Text = "Insert before";
            this.insertBeforeToolStripMenuItem.Click += new System.EventHandler(this.InsertBeforeToolStripMenuItem_Click);
            // 
            // insertPairToolStripMenuItem
            // 
            this.insertPairToolStripMenuItem.Name = "insertPairToolStripMenuItem";
            this.insertPairToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.insertPairToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.insertPairToolStripMenuItem.Text = "Insert after";
            this.insertPairToolStripMenuItem.Click += new System.EventHandler(this.insertPairToolStripMenuItem_Click);
            // 
            // deletePairToolStripMenuItem
            // 
            this.deletePairToolStripMenuItem.Name = "deletePairToolStripMenuItem";
            this.deletePairToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deletePairToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.deletePairToolStripMenuItem.Text = "Delete";
            this.deletePairToolStripMenuItem.Click += new System.EventHandler(this.DeletePairToolStripMenuItem_Click);
            // 
            // statsToolStripMenuItem
            // 
            this.statsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.startpauseStopwatchToolStripMenuItem, this.resetStopwatchToolStripMenuItem});
            this.statsToolStripMenuItem.Name = "statsToolStripMenuItem";
            this.statsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.statsToolStripMenuItem.Text = "Stats";
            // 
            // startpauseStopwatchToolStripMenuItem
            // 
            this.startpauseStopwatchToolStripMenuItem.Name = "startpauseStopwatchToolStripMenuItem";
            this.startpauseStopwatchToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startpauseStopwatchToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.startpauseStopwatchToolStripMenuItem.Text = "Start/pause stopwatch";
            this.startpauseStopwatchToolStripMenuItem.Click += new System.EventHandler(this.startpauseStopwatchToolStripMenuItem_Click);
            // 
            // resetStopwatchToolStripMenuItem
            // 
            this.resetStopwatchToolStripMenuItem.Name = "resetStopwatchToolStripMenuItem";
            this.resetStopwatchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.resetStopwatchToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.resetStopwatchToolStripMenuItem.Text = "Reset stopwatch";
            this.resetStopwatchToolStripMenuItem.Click += new System.EventHandler(this.ResetStopwatchToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.normalModeToolStripMenuItem, this.alternatingModeToolStripMenuItem, this.advancedModeToolStripMenuItem, this.toolStripMenuItem5, this.showGoogleTranslatorToolStripMenuItem, this.splitScreenVerticallyToolStripMenuItem, this.toolStripMenuItem6, this.optionsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
            this.settingsToolStripMenuItem.ShowShortcutKeys = false;
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // normalModeToolStripMenuItem
            // 
            this.normalModeToolStripMenuItem.Name = "normalModeToolStripMenuItem";
            this.normalModeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.normalModeToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.normalModeToolStripMenuItem.Text = "Normal mode";
            this.normalModeToolStripMenuItem.Click += new System.EventHandler(this.NormalModeToolStripMenuItem_Click);
            // 
            // alternatingModeToolStripMenuItem
            // 
            this.alternatingModeToolStripMenuItem.Name = "alternatingModeToolStripMenuItem";
            this.alternatingModeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.alternatingModeToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.alternatingModeToolStripMenuItem.Text = "Alternating mode";
            this.alternatingModeToolStripMenuItem.Click += new System.EventHandler(this.AlternatingModeToolStripMenuItem_Click);
            // 
            // advancedModeToolStripMenuItem
            // 
            this.advancedModeToolStripMenuItem.Name = "advancedModeToolStripMenuItem";
            this.advancedModeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.advancedModeToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.advancedModeToolStripMenuItem.Text = "Advanced mode";
            this.advancedModeToolStripMenuItem.Click += new System.EventHandler(this.advancedModeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(247, 6);
            // 
            // showGoogleTranslatorToolStripMenuItem
            // 
            this.showGoogleTranslatorToolStripMenuItem.CheckOnClick = true;
            this.showGoogleTranslatorToolStripMenuItem.Name = "showGoogleTranslatorToolStripMenuItem";
            this.showGoogleTranslatorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.showGoogleTranslatorToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.showGoogleTranslatorToolStripMenuItem.Text = "Show Google Translator";
            this.showGoogleTranslatorToolStripMenuItem.Click += new System.EventHandler(this.showGoogleTranslatorToolStripMenuItem_Click);
            // 
            // splitScreenVerticallyToolStripMenuItem
            // 
            this.splitScreenVerticallyToolStripMenuItem.CheckOnClick = true;
            this.splitScreenVerticallyToolStripMenuItem.Name = "splitScreenVerticallyToolStripMenuItem";
            this.splitScreenVerticallyToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.splitScreenVerticallyToolStripMenuItem.Text = "Split screen vertically";
            this.splitScreenVerticallyToolStripMenuItem.Click += new System.EventHandler(this.splitScreenVerticallyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(247, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.aglonaReaderSiteToolStripMenuItem, this.paraBooksMakerSiteToolStripMenuItem, this.toolStripMenuItem3, this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aglonaReaderSiteToolStripMenuItem
            // 
            this.aglonaReaderSiteToolStripMenuItem.Name = "aglonaReaderSiteToolStripMenuItem";
            this.aglonaReaderSiteToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.aglonaReaderSiteToolStripMenuItem.Text = "Aglona Reader site";
            this.aglonaReaderSiteToolStripMenuItem.Click += new System.EventHandler(this.aglonaReaderSiteToolStripMenuItem_Click);
            // 
            // paraBooksMakerSiteToolStripMenuItem
            // 
            this.paraBooksMakerSiteToolStripMenuItem.Name = "paraBooksMakerSiteToolStripMenuItem";
            this.paraBooksMakerSiteToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.paraBooksMakerSiteToolStripMenuItem.Text = "ParaBooksMaker site";
            this.paraBooksMakerSiteToolStripMenuItem.Click += new System.EventHandler(this.ParaBooksMakerSiteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(188, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.ssPosition, this.ssPositionPercent});
            this.statusStrip.Location = new System.Drawing.Point(0, 242);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(688, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // ssPosition
            // 
            this.ssPosition.Name = "ssPosition";
            this.ssPosition.Size = new System.Drawing.Size(32, 17);
            this.ssPosition.Text = "0 / 0";
            // 
            // ssPositionPercent
            // 
            this.ssPositionPercent.Name = "ssPositionPercent";
            this.ssPositionPercent.Size = new System.Drawing.Size(44, 17);
            this.ssPositionPercent.Text = "0,00 %";
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.LargeChange = 1;
            this.vScrollBar.Location = new System.Drawing.Point(670, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(18, 182);
            this.vScrollBar.TabIndex = 5;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VScrollBar_Scroll);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 25);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.pTC);
            this.splitContainer.Panel1.Controls.Add(this.vScrollBar);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.webBrowser);
            this.splitContainer.Size = new System.Drawing.Size(688, 216);
            this.splitContainer.SplitterDistance = 182;
            this.splitContainer.SplitterWidth = 9;
            this.splitContainer.TabIndex = 6;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // pTC
            // 
            this.pTC.AlternatingColorScheme = 0;
            this.pTC.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.pTC.Brightness = 0.97D;
            this.pTC.CurrentPair = 0;
            this.pTC.DebugString = null;
            this.pTC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pTC.EditMode = false;
            this.pTC.EditWhenNipped = false;
            this.pTC.FirstRenderedPair = 0;
            stringFormat1.Alignment = System.Drawing.StringAlignment.Near;
            stringFormat1.FormatFlags = ((System.Drawing.StringFormatFlags) (((System.Drawing.StringFormatFlags.FitBlackBox | System.Drawing.StringFormatFlags.LineLimit) | System.Drawing.StringFormatFlags.NoClip)));
            stringFormat1.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
            stringFormat1.LineAlignment = System.Drawing.StringAlignment.Near;
            stringFormat1.Trimming = System.Drawing.StringTrimming.None;
            this.pTC.HighlightedPair = 0;
            this.pTC.HighlightFirstWords = true;
            this.pTC.HighlightFragments = true;
            this.pTC.LastFullScreenLine = 0;
            this.pTC.LastMouseX = -1;
            this.pTC.LastMouseY = -1;
            this.pTC.LastRenderedPair = 0;
            this.pTC.Location = new System.Drawing.Point(0, 0);
            this.pTC.Margin = new System.Windows.Forms.Padding(4);
            this.pTC.MinimumSize = new System.Drawing.Size(10, 10);
            this.pTC.Modified = false;
            this.pTC.MouseCurrentWord = null;
            this.pTC.MousePressed = false;
            this.pTC.Name = "pTC";
            this.pTC.NaturalDividerPosition1 = 0;
            this.pTC.NaturalDividerPosition1W = null;
            this.pTC.NaturalDividerPosition2 = 0;
            this.pTC.NaturalDividerPosition2W = null;
            this.pTC.NumberOfScreenLines = 0;
            this.pTC.PanelMargin = 10;
            this.pTC.PrimaryBg = null;
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
            parallelText1.WithAudio = false;
            this.pTC.PText = parallelText1;
            this.pTC.ReadingMode = 0;
            this.pTC.Reversed = false;
            this.pTC.SecondaryBg = null;
            this.pTC.Selection1Pair = 0;
            this.pTC.Selection1Position = 0;
            this.pTC.Selection2Pair = 0;
            this.pTC.Selection2Position = 0;
            this.pTC.SelectionFinished = true;
            this.pTC.SelectionSide = ((byte) (0));
            this.pTC.Side1Set = false;
            this.pTC.Side2Set = false;
            this.pTC.Size = new System.Drawing.Size(670, 182);
            this.pTC.SpaceLength = 6;
            this.pTC.SplitterMoveOffset = 0;
            this.pTC.SplitterPosition = 445;
            this.pTC.SplitterRatio = 0F;
            this.pTC.SplitterWidth = 4;
            this.pTC.TabIndex = 3;
            this.pTC.VMargin = 3;
            this.pTC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pTC_KeyDown);
            this.pTC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pTC_KeyPress);
            this.pTC.KeyUp += new System.Windows.Forms.KeyEventHandler(this.pTC_KeyUp);
            this.pTC.MouseDown += new System.Windows.Forms.MouseEventHandler(this.parallelTextControl_MouseDown);
            this.pTC.MouseLeave += new System.EventHandler(this.parallelTextControl_MouseLeave);
            this.pTC.MouseMove += new System.Windows.Forms.MouseEventHandler(this.parallelTextControl_MouseMove);
            this.pTC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.parallelTextControl_MouseUp);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Margin = new System.Windows.Forms.Padding(2);
            this.webBrowser.MinimumSize = new System.Drawing.Size(15, 16);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Size = new System.Drawing.Size(688, 25);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 264);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Aglona Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aglonaReaderSiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alternatingModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportLeftTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportRightTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hTMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertBeforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertPairToolStripMenuItem;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRecentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paraBooksMakerSiteToolStripMenuItem;
        private AglonaReader.ParallelTextControl pTC;
        private System.Windows.Forms.ToolStripMenuItem resetStopwatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reverseContentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reverseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGoogleTranslatorToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolStripMenuItem splitScreenVerticallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel ssPosition;
        private System.Windows.Forms.ToolStripStatusLabel ssPositionPercent;
        private System.Windows.Forms.ToolStripMenuItem startpauseStopwatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem structureLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem structureRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.WebBrowser webBrowser;

        #endregion
    }
}

