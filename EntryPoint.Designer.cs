namespace DefleMaskConvert
{
	partial class EntryPoint
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
			this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.echoInstrumentsASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allEchoStreamFormatASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.echoStreamFormatASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openDefleMaskDialog = new System.Windows.Forms.OpenFileDialog();
			this.projectPanel = new System.Windows.Forms.GroupBox();
			this.unsupportedEffects = new System.Windows.Forms.GroupBox();
			this.unsupportedTreeView = new System.Windows.Forms.TreeView();
			this.exportParams = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnRateNotChange = new System.Windows.Forms.RadioButton();
			this.btnRate4097 = new System.Windows.Forms.RadioButton();
			this.btnRate4439 = new System.Windows.Forms.RadioButton();
			this.btnRate4842 = new System.Windows.Forms.RadioButton();
			this.btnRate5326 = new System.Windows.Forms.RadioButton();
			this.btnRate5918 = new System.Windows.Forms.RadioButton();
			this.btnRate6658 = new System.Windows.Forms.RadioButton();
			this.btnRate7609 = new System.Windows.Forms.RadioButton();
			this.btnRate8877 = new System.Windows.Forms.RadioButton();
			this.btnRate10653 = new System.Windows.Forms.RadioButton();
			this.btnRate13316 = new System.Windows.Forms.RadioButton();
			this.btnRate17755 = new System.Windows.Forms.RadioButton();
			this.btnRate26632 = new System.Windows.Forms.RadioButton();
			this.btnLoopWholeTrack = new System.Windows.Forms.CheckBox();
			this.btnLockChannels = new System.Windows.Forms.CheckBox();
			this.btnExportChannelPSGNoise = new System.Windows.Forms.CheckBox();
			this.btnExportChannelPSG3 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelPSG2 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelPSG1 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelFM6 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelFM5 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelFM4 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelFM3 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelFM2 = new System.Windows.Forms.CheckBox();
			this.btnExportChannelFM1 = new System.Windows.Forms.CheckBox();
			this.songsTreeView = new System.Windows.Forms.TreeView();
			this.exportAssemblyDialog = new System.Windows.Forms.SaveFileDialog();
			this.saveProjectDialog = new System.Windows.Forms.SaveFileDialog();
			this.openProjectDialog = new System.Windows.Forms.OpenFileDialog();
			this.exportFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.allEchoInstrumentsBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.echoStreamFormatESFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportBinaryDialog = new System.Windows.Forms.SaveFileDialog();
			this.allEchoSongsBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mainMenuStrip.SuspendLayout();
			this.projectPanel.SuspendLayout();
			this.unsupportedEffects.SuspendLayout();
			this.exportParams.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenuStrip
			// 
			this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.mainMenuStrip.Name = "mainMenuStrip";
			this.mainMenuStrip.Size = new System.Drawing.Size(784, 24);
			this.mainMenuStrip.TabIndex = 0;
			this.mainMenuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newToolStripMenuItem.Text = "New...";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.openToolStripMenuItem.Text = "Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.importToolStripMenuItem.Text = "Import...";
			this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.echoInstrumentsASMToolStripMenuItem,
            this.allEchoInstrumentsBinaryToolStripMenuItem,
            this.allEchoStreamFormatASMToolStripMenuItem,
            this.allEchoSongsBinaryToolStripMenuItem,
            this.toolStripSeparator2,
            this.echoStreamFormatASMToolStripMenuItem,
            this.echoStreamFormatESFToolStripMenuItem});
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exportToolStripMenuItem.Text = "Export";
			// 
			// echoInstrumentsASMToolStripMenuItem
			// 
			this.echoInstrumentsASMToolStripMenuItem.Name = "echoInstrumentsASMToolStripMenuItem";
			this.echoInstrumentsASMToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			this.echoInstrumentsASMToolStripMenuItem.Text = "All Echo Instruments (ASM)";
			this.echoInstrumentsASMToolStripMenuItem.Click += new System.EventHandler(this.echoInstrumentsASMToolStripMenuItem_Click);
			// 
			// allEchoStreamFormatASMToolStripMenuItem
			// 
			this.allEchoStreamFormatASMToolStripMenuItem.Name = "allEchoStreamFormatASMToolStripMenuItem";
			this.allEchoStreamFormatASMToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.allEchoStreamFormatASMToolStripMenuItem.Text = "All Echo Songs (ASM)";
			this.allEchoStreamFormatASMToolStripMenuItem.Click += new System.EventHandler(this.allEchoStreamFormatASMToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(231, 6);
			// 
			// echoStreamFormatASMToolStripMenuItem
			// 
			this.echoStreamFormatASMToolStripMenuItem.Name = "echoStreamFormatASMToolStripMenuItem";
			this.echoStreamFormatASMToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.echoStreamFormatASMToolStripMenuItem.Text = "Echo Song (ASM)";
			this.echoStreamFormatASMToolStripMenuItem.Click += new System.EventHandler(this.echoStreamFormatASMToolStripMenuItem_Click);
			// 
			// openDefleMaskDialog
			// 
			this.openDefleMaskDialog.Filter = "DefleMask Module Format (DMF)|*.dmf";
			this.openDefleMaskDialog.Title = "Import DefleMask";
			// 
			// projectPanel
			// 
			this.projectPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.projectPanel.Controls.Add(this.unsupportedEffects);
			this.projectPanel.Controls.Add(this.exportParams);
			this.projectPanel.Controls.Add(this.songsTreeView);
			this.projectPanel.Location = new System.Drawing.Point(12, 27);
			this.projectPanel.Name = "projectPanel";
			this.projectPanel.Size = new System.Drawing.Size(760, 562);
			this.projectPanel.TabIndex = 19;
			this.projectPanel.TabStop = false;
			this.projectPanel.Text = "DMF";
			// 
			// unsupportedEffects
			// 
			this.unsupportedEffects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.unsupportedEffects.Controls.Add(this.unsupportedTreeView);
			this.unsupportedEffects.Location = new System.Drawing.Point(509, 402);
			this.unsupportedEffects.Name = "unsupportedEffects";
			this.unsupportedEffects.Size = new System.Drawing.Size(245, 154);
			this.unsupportedEffects.TabIndex = 24;
			this.unsupportedEffects.TabStop = false;
			this.unsupportedEffects.Text = "Unsupported Effects In File";
			// 
			// unsupportedTreeView
			// 
			this.unsupportedTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.unsupportedTreeView.Location = new System.Drawing.Point(6, 19);
			this.unsupportedTreeView.Name = "unsupportedTreeView";
			this.unsupportedTreeView.Size = new System.Drawing.Size(233, 129);
			this.unsupportedTreeView.TabIndex = 0;
			// 
			// exportParams
			// 
			this.exportParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.exportParams.Controls.Add(this.groupBox1);
			this.exportParams.Controls.Add(this.btnLoopWholeTrack);
			this.exportParams.Controls.Add(this.btnLockChannels);
			this.exportParams.Controls.Add(this.btnExportChannelPSGNoise);
			this.exportParams.Controls.Add(this.btnExportChannelPSG3);
			this.exportParams.Controls.Add(this.btnExportChannelPSG2);
			this.exportParams.Controls.Add(this.btnExportChannelPSG1);
			this.exportParams.Controls.Add(this.btnExportChannelFM6);
			this.exportParams.Controls.Add(this.btnExportChannelFM5);
			this.exportParams.Controls.Add(this.btnExportChannelFM4);
			this.exportParams.Controls.Add(this.btnExportChannelFM3);
			this.exportParams.Controls.Add(this.btnExportChannelFM2);
			this.exportParams.Controls.Add(this.btnExportChannelFM1);
			this.exportParams.Location = new System.Drawing.Point(509, 19);
			this.exportParams.Name = "exportParams";
			this.exportParams.Size = new System.Drawing.Size(245, 377);
			this.exportParams.TabIndex = 23;
			this.exportParams.TabStop = false;
			this.exportParams.Text = "Export Params";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnRateNotChange);
			this.groupBox1.Controls.Add(this.btnRate4097);
			this.groupBox1.Controls.Add(this.btnRate4439);
			this.groupBox1.Controls.Add(this.btnRate4842);
			this.groupBox1.Controls.Add(this.btnRate5326);
			this.groupBox1.Controls.Add(this.btnRate5918);
			this.groupBox1.Controls.Add(this.btnRate6658);
			this.groupBox1.Controls.Add(this.btnRate7609);
			this.groupBox1.Controls.Add(this.btnRate8877);
			this.groupBox1.Controls.Add(this.btnRate10653);
			this.groupBox1.Controls.Add(this.btnRate13316);
			this.groupBox1.Controls.Add(this.btnRate17755);
			this.groupBox1.Controls.Add(this.btnRate26632);
			this.groupBox1.Location = new System.Drawing.Point(6, 182);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(233, 189);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Playback PCM Rate";
			// 
			// btnRateNotChange
			// 
			this.btnRateNotChange.AutoSize = true;
			this.btnRateNotChange.Location = new System.Drawing.Point(106, 135);
			this.btnRateNotChange.Name = "btnRateNotChange";
			this.btnRateNotChange.Size = new System.Drawing.Size(90, 17);
			this.btnRateNotChange.TabIndex = 12;
			this.btnRateNotChange.TabStop = true;
			this.btnRateNotChange.Text = "Don\'t Change";
			this.btnRateNotChange.UseVisualStyleBackColor = true;
			this.btnRateNotChange.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate4097
			// 
			this.btnRate4097.AutoSize = true;
			this.btnRate4097.Location = new System.Drawing.Point(106, 112);
			this.btnRate4097.Name = "btnRate4097";
			this.btnRate4097.Size = new System.Drawing.Size(65, 17);
			this.btnRate4097.TabIndex = 11;
			this.btnRate4097.TabStop = true;
			this.btnRate4097.Text = "4097 Hz";
			this.btnRate4097.UseVisualStyleBackColor = true;
			this.btnRate4097.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate4439
			// 
			this.btnRate4439.AutoSize = true;
			this.btnRate4439.Location = new System.Drawing.Point(106, 89);
			this.btnRate4439.Name = "btnRate4439";
			this.btnRate4439.Size = new System.Drawing.Size(65, 17);
			this.btnRate4439.TabIndex = 10;
			this.btnRate4439.TabStop = true;
			this.btnRate4439.Text = "4439 Hz";
			this.btnRate4439.UseVisualStyleBackColor = true;
			this.btnRate4439.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate4842
			// 
			this.btnRate4842.AutoSize = true;
			this.btnRate4842.Location = new System.Drawing.Point(106, 66);
			this.btnRate4842.Name = "btnRate4842";
			this.btnRate4842.Size = new System.Drawing.Size(65, 17);
			this.btnRate4842.TabIndex = 9;
			this.btnRate4842.TabStop = true;
			this.btnRate4842.Text = "4842 Hz";
			this.btnRate4842.UseVisualStyleBackColor = true;
			this.btnRate4842.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate5326
			// 
			this.btnRate5326.AutoSize = true;
			this.btnRate5326.Location = new System.Drawing.Point(106, 43);
			this.btnRate5326.Name = "btnRate5326";
			this.btnRate5326.Size = new System.Drawing.Size(65, 17);
			this.btnRate5326.TabIndex = 8;
			this.btnRate5326.TabStop = true;
			this.btnRate5326.Text = "5326 Hz";
			this.btnRate5326.UseVisualStyleBackColor = true;
			this.btnRate5326.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate5918
			// 
			this.btnRate5918.AutoSize = true;
			this.btnRate5918.Location = new System.Drawing.Point(106, 20);
			this.btnRate5918.Name = "btnRate5918";
			this.btnRate5918.Size = new System.Drawing.Size(65, 17);
			this.btnRate5918.TabIndex = 7;
			this.btnRate5918.TabStop = true;
			this.btnRate5918.Text = "5918 Hz";
			this.btnRate5918.UseVisualStyleBackColor = true;
			this.btnRate5918.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate6658
			// 
			this.btnRate6658.AutoSize = true;
			this.btnRate6658.Location = new System.Drawing.Point(8, 158);
			this.btnRate6658.Name = "btnRate6658";
			this.btnRate6658.Size = new System.Drawing.Size(65, 17);
			this.btnRate6658.TabIndex = 6;
			this.btnRate6658.TabStop = true;
			this.btnRate6658.Text = "6658 Hz";
			this.btnRate6658.UseVisualStyleBackColor = true;
			this.btnRate6658.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate7609
			// 
			this.btnRate7609.AutoSize = true;
			this.btnRate7609.Location = new System.Drawing.Point(8, 135);
			this.btnRate7609.Name = "btnRate7609";
			this.btnRate7609.Size = new System.Drawing.Size(65, 17);
			this.btnRate7609.TabIndex = 5;
			this.btnRate7609.TabStop = true;
			this.btnRate7609.Text = "7609 Hz";
			this.btnRate7609.UseVisualStyleBackColor = true;
			this.btnRate7609.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate8877
			// 
			this.btnRate8877.AutoSize = true;
			this.btnRate8877.Location = new System.Drawing.Point(7, 112);
			this.btnRate8877.Name = "btnRate8877";
			this.btnRate8877.Size = new System.Drawing.Size(65, 17);
			this.btnRate8877.TabIndex = 4;
			this.btnRate8877.TabStop = true;
			this.btnRate8877.Text = "8877 Hz";
			this.btnRate8877.UseVisualStyleBackColor = true;
			this.btnRate8877.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate10653
			// 
			this.btnRate10653.AutoSize = true;
			this.btnRate10653.Location = new System.Drawing.Point(7, 89);
			this.btnRate10653.Name = "btnRate10653";
			this.btnRate10653.Size = new System.Drawing.Size(71, 17);
			this.btnRate10653.TabIndex = 3;
			this.btnRate10653.TabStop = true;
			this.btnRate10653.Text = "10653 Hz";
			this.btnRate10653.UseVisualStyleBackColor = true;
			this.btnRate10653.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate13316
			// 
			this.btnRate13316.AutoSize = true;
			this.btnRate13316.Location = new System.Drawing.Point(7, 66);
			this.btnRate13316.Name = "btnRate13316";
			this.btnRate13316.Size = new System.Drawing.Size(71, 17);
			this.btnRate13316.TabIndex = 2;
			this.btnRate13316.TabStop = true;
			this.btnRate13316.Text = "13316 Hz";
			this.btnRate13316.UseVisualStyleBackColor = true;
			this.btnRate13316.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate17755
			// 
			this.btnRate17755.AutoSize = true;
			this.btnRate17755.Location = new System.Drawing.Point(7, 43);
			this.btnRate17755.Name = "btnRate17755";
			this.btnRate17755.Size = new System.Drawing.Size(71, 17);
			this.btnRate17755.TabIndex = 1;
			this.btnRate17755.TabStop = true;
			this.btnRate17755.Text = "17755 Hz";
			this.btnRate17755.UseVisualStyleBackColor = true;
			this.btnRate17755.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnRate26632
			// 
			this.btnRate26632.AutoSize = true;
			this.btnRate26632.Location = new System.Drawing.Point(7, 20);
			this.btnRate26632.Name = "btnRate26632";
			this.btnRate26632.Size = new System.Drawing.Size(71, 17);
			this.btnRate26632.TabIndex = 0;
			this.btnRate26632.TabStop = true;
			this.btnRate26632.Text = "26632 Hz";
			this.btnRate26632.UseVisualStyleBackColor = true;
			this.btnRate26632.CheckedChanged += new System.EventHandler(this.btnPCMRateCheckedChanged);
			// 
			// btnLoopWholeTrack
			// 
			this.btnLoopWholeTrack.AutoSize = true;
			this.btnLoopWholeTrack.Location = new System.Drawing.Point(112, 159);
			this.btnLoopWholeTrack.Name = "btnLoopWholeTrack";
			this.btnLoopWholeTrack.Size = new System.Drawing.Size(115, 17);
			this.btnLoopWholeTrack.TabIndex = 11;
			this.btnLoopWholeTrack.Text = "Loop Whole Track";
			this.btnLoopWholeTrack.UseVisualStyleBackColor = true;
			this.btnLoopWholeTrack.CheckedChanged += new System.EventHandler(this.btnLoopWholeTrack_CheckedChanged);
			// 
			// btnLockChannels
			// 
			this.btnLockChannels.AutoSize = true;
			this.btnLockChannels.Location = new System.Drawing.Point(7, 159);
			this.btnLockChannels.Name = "btnLockChannels";
			this.btnLockChannels.Size = new System.Drawing.Size(97, 17);
			this.btnLockChannels.TabIndex = 10;
			this.btnLockChannels.Text = "Lock Channels";
			this.btnLockChannels.UseVisualStyleBackColor = true;
			this.btnLockChannels.CheckedChanged += new System.EventHandler(this.btnLockChannels_CheckedChanged);
			// 
			// btnExportChannelPSGNoise
			// 
			this.btnExportChannelPSGNoise.AutoSize = true;
			this.btnExportChannelPSGNoise.Location = new System.Drawing.Point(112, 123);
			this.btnExportChannelPSGNoise.Name = "btnExportChannelPSGNoise";
			this.btnExportChannelPSGNoise.Size = new System.Drawing.Size(120, 17);
			this.btnExportChannelPSGNoise.TabIndex = 9;
			this.btnExportChannelPSGNoise.Text = "Channel PSG Noise";
			this.btnExportChannelPSGNoise.UseVisualStyleBackColor = true;
			this.btnExportChannelPSGNoise.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelPSG3
			// 
			this.btnExportChannelPSG3.AutoSize = true;
			this.btnExportChannelPSG3.Location = new System.Drawing.Point(112, 100);
			this.btnExportChannelPSG3.Name = "btnExportChannelPSG3";
			this.btnExportChannelPSG3.Size = new System.Drawing.Size(99, 17);
			this.btnExportChannelPSG3.TabIndex = 8;
			this.btnExportChannelPSG3.Text = "Channel PSG 3";
			this.btnExportChannelPSG3.UseVisualStyleBackColor = true;
			this.btnExportChannelPSG3.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelPSG2
			// 
			this.btnExportChannelPSG2.AutoSize = true;
			this.btnExportChannelPSG2.Location = new System.Drawing.Point(7, 123);
			this.btnExportChannelPSG2.Name = "btnExportChannelPSG2";
			this.btnExportChannelPSG2.Size = new System.Drawing.Size(99, 17);
			this.btnExportChannelPSG2.TabIndex = 7;
			this.btnExportChannelPSG2.Text = "Channel PSG 2";
			this.btnExportChannelPSG2.UseVisualStyleBackColor = true;
			this.btnExportChannelPSG2.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelPSG1
			// 
			this.btnExportChannelPSG1.AutoSize = true;
			this.btnExportChannelPSG1.Location = new System.Drawing.Point(7, 100);
			this.btnExportChannelPSG1.Name = "btnExportChannelPSG1";
			this.btnExportChannelPSG1.Size = new System.Drawing.Size(99, 17);
			this.btnExportChannelPSG1.TabIndex = 6;
			this.btnExportChannelPSG1.Text = "Channel PSG 1";
			this.btnExportChannelPSG1.UseVisualStyleBackColor = true;
			this.btnExportChannelPSG1.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelFM6
			// 
			this.btnExportChannelFM6.AutoSize = true;
			this.btnExportChannelFM6.Location = new System.Drawing.Point(112, 66);
			this.btnExportChannelFM6.Name = "btnExportChannelFM6";
			this.btnExportChannelFM6.Size = new System.Drawing.Size(123, 17);
			this.btnExportChannelFM6.TabIndex = 5;
			this.btnExportChannelFM6.Text = "Channel FM 6 (DAC)";
			this.btnExportChannelFM6.UseVisualStyleBackColor = true;
			this.btnExportChannelFM6.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelFM5
			// 
			this.btnExportChannelFM5.AutoSize = true;
			this.btnExportChannelFM5.Location = new System.Drawing.Point(112, 43);
			this.btnExportChannelFM5.Name = "btnExportChannelFM5";
			this.btnExportChannelFM5.Size = new System.Drawing.Size(92, 17);
			this.btnExportChannelFM5.TabIndex = 4;
			this.btnExportChannelFM5.Text = "Channel FM 5";
			this.btnExportChannelFM5.UseVisualStyleBackColor = true;
			this.btnExportChannelFM5.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelFM4
			// 
			this.btnExportChannelFM4.AutoSize = true;
			this.btnExportChannelFM4.Location = new System.Drawing.Point(112, 20);
			this.btnExportChannelFM4.Name = "btnExportChannelFM4";
			this.btnExportChannelFM4.Size = new System.Drawing.Size(92, 17);
			this.btnExportChannelFM4.TabIndex = 3;
			this.btnExportChannelFM4.Text = "Channel FM 4";
			this.btnExportChannelFM4.UseVisualStyleBackColor = true;
			this.btnExportChannelFM4.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelFM3
			// 
			this.btnExportChannelFM3.AutoSize = true;
			this.btnExportChannelFM3.Location = new System.Drawing.Point(7, 66);
			this.btnExportChannelFM3.Name = "btnExportChannelFM3";
			this.btnExportChannelFM3.Size = new System.Drawing.Size(92, 17);
			this.btnExportChannelFM3.TabIndex = 2;
			this.btnExportChannelFM3.Text = "Channel FM 3";
			this.btnExportChannelFM3.UseVisualStyleBackColor = true;
			this.btnExportChannelFM3.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelFM2
			// 
			this.btnExportChannelFM2.AutoSize = true;
			this.btnExportChannelFM2.Location = new System.Drawing.Point(7, 43);
			this.btnExportChannelFM2.Name = "btnExportChannelFM2";
			this.btnExportChannelFM2.Size = new System.Drawing.Size(92, 17);
			this.btnExportChannelFM2.TabIndex = 1;
			this.btnExportChannelFM2.Text = "Channel FM 2";
			this.btnExportChannelFM2.UseVisualStyleBackColor = true;
			this.btnExportChannelFM2.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// btnExportChannelFM1
			// 
			this.btnExportChannelFM1.AutoSize = true;
			this.btnExportChannelFM1.Location = new System.Drawing.Point(7, 20);
			this.btnExportChannelFM1.Name = "btnExportChannelFM1";
			this.btnExportChannelFM1.Size = new System.Drawing.Size(92, 17);
			this.btnExportChannelFM1.TabIndex = 0;
			this.btnExportChannelFM1.Text = "Channel FM 1";
			this.btnExportChannelFM1.UseVisualStyleBackColor = true;
			this.btnExportChannelFM1.CheckedChanged += new System.EventHandler(this.btnExportChannelCheckedChanged);
			// 
			// songsTreeView
			// 
			this.songsTreeView.AllowDrop = true;
			this.songsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.songsTreeView.CheckBoxes = true;
			this.songsTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.songsTreeView.HideSelection = false;
			this.songsTreeView.LabelEdit = true;
			this.songsTreeView.Location = new System.Drawing.Point(6, 19);
			this.songsTreeView.Name = "songsTreeView";
			this.songsTreeView.Size = new System.Drawing.Size(497, 537);
			this.songsTreeView.TabIndex = 22;
			this.songsTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.songsTreeView_AfterLabelEdit);
			this.songsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.songsTreeView_AfterCheck);
			this.songsTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.songsTreeView_ItemDrag);
			this.songsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.songsTreeView_AfterSelect);
			this.songsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.songsTreeView_DragDrop);
			this.songsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.songsTreeView_DragEnter);
			this.songsTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.songsTreeView_KeyUp);
			// 
			// exportAssemblyDialog
			// 
			this.exportAssemblyDialog.Filter = "Assembly|*.asm";
			// 
			// saveProjectDialog
			// 
			this.saveProjectDialog.Filter = "DefleMask Convert Project|*.dmconvert";
			this.saveProjectDialog.Title = "Save Project";
			// 
			// openProjectDialog
			// 
			this.openProjectDialog.Filter = "DefleMask Convert Project|*.dmconvert";
			this.openProjectDialog.Title = "Open DefleMask Convert Project";
			// 
			// exportFolderBrowserDialog
			// 
			this.exportFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			// 
			// allEchoInstrumentsBinaryToolStripMenuItem
			// 
			this.allEchoInstrumentsBinaryToolStripMenuItem.Name = "allEchoInstrumentsBinaryToolStripMenuItem";
			this.allEchoInstrumentsBinaryToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			this.allEchoInstrumentsBinaryToolStripMenuItem.Text = "All Echo Instruments (Binary)";
			this.allEchoInstrumentsBinaryToolStripMenuItem.Click += new System.EventHandler(this.allEchoInstrumentsBinaryToolStripMenuItem_Click);
			// 
			// echoStreamFormatESFToolStripMenuItem
			// 
			this.echoStreamFormatESFToolStripMenuItem.Name = "echoStreamFormatESFToolStripMenuItem";
			this.echoStreamFormatESFToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.echoStreamFormatESFToolStripMenuItem.Text = "Echo Song (ESF)";
			this.echoStreamFormatESFToolStripMenuItem.Click += new System.EventHandler(this.echoStreamFormatESFToolStripMenuItem_Click);
			// 
			// exportBinaryDialog
			// 
			this.exportBinaryDialog.Filter = "Echo Stream Format|*.esf";
			this.exportBinaryDialog.Title = "Export Echo Stream Format";
			// 
			// allEchoSongsBinaryToolStripMenuItem
			// 
			this.allEchoSongsBinaryToolStripMenuItem.Name = "allEchoSongsBinaryToolStripMenuItem";
			this.allEchoSongsBinaryToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.allEchoSongsBinaryToolStripMenuItem.Text = "All Echo Songs (Binary)";
			this.allEchoSongsBinaryToolStripMenuItem.Click += new System.EventHandler(this.allEchoSongsBinaryToolStripMenuItem_Click);
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
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// EntryPoint
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 601);
			this.Controls.Add(this.projectPanel);
			this.Controls.Add(this.mainMenuStrip);
			this.MainMenuStrip = this.mainMenuStrip;
			this.MinimumSize = new System.Drawing.Size(800, 640);
			this.Name = "EntryPoint";
			this.Text = "DefleMask Convert";
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.projectPanel.ResumeLayout(false);
			this.unsupportedEffects.ResumeLayout(false);
			this.exportParams.ResumeLayout(false);
			this.exportParams.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip mainMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openDefleMaskDialog;
		private System.Windows.Forms.GroupBox projectPanel;
		private System.Windows.Forms.TreeView songsTreeView;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem echoInstrumentsASMToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog exportAssemblyDialog;
		private System.Windows.Forms.ToolStripMenuItem echoStreamFormatASMToolStripMenuItem;
		private System.Windows.Forms.GroupBox exportParams;
		private System.Windows.Forms.CheckBox btnExportChannelPSGNoise;
		private System.Windows.Forms.CheckBox btnExportChannelPSG3;
		private System.Windows.Forms.CheckBox btnExportChannelPSG2;
		private System.Windows.Forms.CheckBox btnExportChannelPSG1;
		private System.Windows.Forms.CheckBox btnExportChannelFM6;
		private System.Windows.Forms.CheckBox btnExportChannelFM5;
		private System.Windows.Forms.CheckBox btnExportChannelFM4;
		private System.Windows.Forms.CheckBox btnExportChannelFM3;
		private System.Windows.Forms.CheckBox btnExportChannelFM2;
		private System.Windows.Forms.CheckBox btnExportChannelFM1;
		private System.Windows.Forms.CheckBox btnLockChannels;
		private System.Windows.Forms.CheckBox btnLoopWholeTrack;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton btnRate26632;
		private System.Windows.Forms.RadioButton btnRateNotChange;
		private System.Windows.Forms.RadioButton btnRate4097;
		private System.Windows.Forms.RadioButton btnRate4439;
		private System.Windows.Forms.RadioButton btnRate4842;
		private System.Windows.Forms.RadioButton btnRate5326;
		private System.Windows.Forms.RadioButton btnRate5918;
		private System.Windows.Forms.RadioButton btnRate6658;
		private System.Windows.Forms.RadioButton btnRate7609;
		private System.Windows.Forms.RadioButton btnRate8877;
		private System.Windows.Forms.RadioButton btnRate10653;
		private System.Windows.Forms.RadioButton btnRate13316;
		private System.Windows.Forms.RadioButton btnRate17755;
		private System.Windows.Forms.GroupBox unsupportedEffects;
		private System.Windows.Forms.TreeView unsupportedTreeView;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.SaveFileDialog saveProjectDialog;
		private System.Windows.Forms.OpenFileDialog openProjectDialog;
		private System.Windows.Forms.ToolStripMenuItem allEchoStreamFormatASMToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.FolderBrowserDialog exportFolderBrowserDialog;
		private System.Windows.Forms.ToolStripMenuItem allEchoInstrumentsBinaryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem echoStreamFormatESFToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog exportBinaryDialog;
		private System.Windows.Forms.ToolStripMenuItem allEchoSongsBinaryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
	}
}

