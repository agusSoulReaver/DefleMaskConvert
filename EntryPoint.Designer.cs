﻿namespace DefleMaskConvert
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
			this.allEchoInstrumentsBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allEchoStreamFormatASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allEchoSongsBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allEchoSFXsASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.echoSFXsPrioritiesASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.echoStreamFormatASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.echoStreamFormatESFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toASMProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportChangeBitRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openDefleMaskDialog = new System.Windows.Forms.OpenFileDialog();
			this.projectPanel = new System.Windows.Forms.GroupBox();
			this.audioSection = new System.Windows.Forms.TabControl();
			this.songsMode = new System.Windows.Forms.TabPage();
			this.songsTreeView = new System.Windows.Forms.TreeView();
			this.sfxsMode = new System.Windows.Forms.TabPage();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSFXPriority = new System.Windows.Forms.NumericUpDown();
			this.sfxsTreeView = new System.Windows.Forms.TreeView();
			this.unsupportedEffects = new System.Windows.Forms.GroupBox();
			this.unsupportedTreeView = new System.Windows.Forms.TreeView();
			this.exportParams = new System.Windows.Forms.GroupBox();
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
			this.exportAssemblyDialog = new System.Windows.Forms.SaveFileDialog();
			this.saveProjectDialog = new System.Windows.Forms.SaveFileDialog();
			this.openProjectDialog = new System.Windows.Forms.OpenFileDialog();
			this.exportFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.exportBinaryDialog = new System.Windows.Forms.SaveFileDialog();
			this.mainMenuStrip.SuspendLayout();
			this.projectPanel.SuspendLayout();
			this.audioSection.SuspendLayout();
			this.songsMode.SuspendLayout();
			this.sfxsMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnSFXPriority)).BeginInit();
			this.unsupportedEffects.SuspendLayout();
			this.exportParams.SuspendLayout();
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
            this.exportToolStripMenuItem,
            this.toolStripSeparator4,
            this.settingsToolStripMenuItem});
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
            this.allEchoSFXsASMToolStripMenuItem,
            this.echoSFXsPrioritiesASMToolStripMenuItem,
            this.toolStripSeparator2,
            this.echoStreamFormatASMToolStripMenuItem,
            this.echoStreamFormatESFToolStripMenuItem,
            this.toolStripSeparator3,
            this.toASMProjectToolStripMenuItem});
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exportToolStripMenuItem.Text = "Export";
			// 
			// echoInstrumentsASMToolStripMenuItem
			// 
			this.echoInstrumentsASMToolStripMenuItem.Name = "echoInstrumentsASMToolStripMenuItem";
			this.echoInstrumentsASMToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.echoInstrumentsASMToolStripMenuItem.Text = "All Echo Instruments (ASM)";
			this.echoInstrumentsASMToolStripMenuItem.Click += new System.EventHandler(this.echoInstrumentsASMToolStripMenuItem_Click);
			// 
			// allEchoInstrumentsBinaryToolStripMenuItem
			// 
			this.allEchoInstrumentsBinaryToolStripMenuItem.Name = "allEchoInstrumentsBinaryToolStripMenuItem";
			this.allEchoInstrumentsBinaryToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.allEchoInstrumentsBinaryToolStripMenuItem.Text = "All Echo Instruments (Binary)";
			this.allEchoInstrumentsBinaryToolStripMenuItem.Click += new System.EventHandler(this.allEchoInstrumentsBinaryToolStripMenuItem_Click);
			// 
			// allEchoStreamFormatASMToolStripMenuItem
			// 
			this.allEchoStreamFormatASMToolStripMenuItem.Name = "allEchoStreamFormatASMToolStripMenuItem";
			this.allEchoStreamFormatASMToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.allEchoStreamFormatASMToolStripMenuItem.Text = "All Echo Songs (ASM)";
			this.allEchoStreamFormatASMToolStripMenuItem.Click += new System.EventHandler(this.allEchoStreamFormatASMToolStripMenuItem_Click);
			// 
			// allEchoSongsBinaryToolStripMenuItem
			// 
			this.allEchoSongsBinaryToolStripMenuItem.Name = "allEchoSongsBinaryToolStripMenuItem";
			this.allEchoSongsBinaryToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.allEchoSongsBinaryToolStripMenuItem.Text = "All Echo Songs (Binary)";
			this.allEchoSongsBinaryToolStripMenuItem.Click += new System.EventHandler(this.allEchoSongsBinaryToolStripMenuItem_Click);
			// 
			// allEchoSFXsASMToolStripMenuItem
			// 
			this.allEchoSFXsASMToolStripMenuItem.Name = "allEchoSFXsASMToolStripMenuItem";
			this.allEchoSFXsASMToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.allEchoSFXsASMToolStripMenuItem.Text = "All Echo SFXs (ASM)";
			this.allEchoSFXsASMToolStripMenuItem.Click += new System.EventHandler(this.allEchoSFXsASMToolStripMenuItem_Click);
			// 
			// echoSFXsPrioritiesASMToolStripMenuItem
			// 
			this.echoSFXsPrioritiesASMToolStripMenuItem.Name = "echoSFXsPrioritiesASMToolStripMenuItem";
			this.echoSFXsPrioritiesASMToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.echoSFXsPrioritiesASMToolStripMenuItem.Text = "Echo SFXs Priorities (ASM)";
			this.echoSFXsPrioritiesASMToolStripMenuItem.Click += new System.EventHandler(this.echoSFXsPrioritiesASMToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(224, 6);
			// 
			// echoStreamFormatASMToolStripMenuItem
			// 
			this.echoStreamFormatASMToolStripMenuItem.Name = "echoStreamFormatASMToolStripMenuItem";
			this.echoStreamFormatASMToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.echoStreamFormatASMToolStripMenuItem.Text = "Echo Song (ASM)";
			this.echoStreamFormatASMToolStripMenuItem.Click += new System.EventHandler(this.echoStreamFormatASMToolStripMenuItem_Click);
			// 
			// echoStreamFormatESFToolStripMenuItem
			// 
			this.echoStreamFormatESFToolStripMenuItem.Name = "echoStreamFormatESFToolStripMenuItem";
			this.echoStreamFormatESFToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.echoStreamFormatESFToolStripMenuItem.Text = "Echo Song (ESF)";
			this.echoStreamFormatESFToolStripMenuItem.Click += new System.EventHandler(this.echoStreamFormatESFToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(224, 6);
			// 
			// toASMProjectToolStripMenuItem
			// 
			this.toASMProjectToolStripMenuItem.Name = "toASMProjectToolStripMenuItem";
			this.toASMProjectToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			this.toASMProjectToolStripMenuItem.Text = "To ASM Project";
			this.toASMProjectToolStripMenuItem.Click += new System.EventHandler(this.toASMProjectToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(149, 6);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportChangeBitRateToolStripMenuItem});
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.settingsToolStripMenuItem.Text = "Settings";
			// 
			// exportChangeBitRateToolStripMenuItem
			// 
			this.exportChangeBitRateToolStripMenuItem.Name = "exportChangeBitRateToolStripMenuItem";
			this.exportChangeBitRateToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.exportChangeBitRateToolStripMenuItem.Text = "Export Change Bit Rate";
			this.exportChangeBitRateToolStripMenuItem.Click += new System.EventHandler(this.exportChangeBitRateToolStripMenuItem_Click);
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
			this.projectPanel.Controls.Add(this.audioSection);
			this.projectPanel.Controls.Add(this.unsupportedEffects);
			this.projectPanel.Controls.Add(this.exportParams);
			this.projectPanel.Location = new System.Drawing.Point(12, 27);
			this.projectPanel.Name = "projectPanel";
			this.projectPanel.Size = new System.Drawing.Size(760, 562);
			this.projectPanel.TabIndex = 19;
			this.projectPanel.TabStop = false;
			this.projectPanel.Text = "DMF";
			// 
			// audioSection
			// 
			this.audioSection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.audioSection.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.audioSection.Controls.Add(this.songsMode);
			this.audioSection.Controls.Add(this.sfxsMode);
			this.audioSection.Location = new System.Drawing.Point(6, 19);
			this.audioSection.Name = "audioSection";
			this.audioSection.SelectedIndex = 0;
			this.audioSection.Size = new System.Drawing.Size(497, 537);
			this.audioSection.TabIndex = 25;
			this.audioSection.SelectedIndexChanged += new System.EventHandler(this.audioSection_SelectedIndexChanged);
			// 
			// songsMode
			// 
			this.songsMode.Controls.Add(this.songsTreeView);
			this.songsMode.Location = new System.Drawing.Point(4, 25);
			this.songsMode.Name = "songsMode";
			this.songsMode.Padding = new System.Windows.Forms.Padding(3);
			this.songsMode.Size = new System.Drawing.Size(489, 508);
			this.songsMode.TabIndex = 0;
			this.songsMode.Text = "Songs";
			this.songsMode.UseVisualStyleBackColor = true;
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
			this.songsTreeView.Location = new System.Drawing.Point(6, 6);
			this.songsTreeView.Name = "songsTreeView";
			this.songsTreeView.Size = new System.Drawing.Size(477, 496);
			this.songsTreeView.TabIndex = 22;
			this.songsTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.songsTreeView_AfterLabelEdit);
			this.songsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.songsTreeView_AfterCheck);
			this.songsTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.songsTreeView_ItemDrag);
			this.songsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.songsTreeView_AfterSelect);
			this.songsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.songsTreeView_DragDrop);
			this.songsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.songsTreeView_DragEnter);
			this.songsTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.songsTreeView_KeyUp);
			// 
			// sfxsMode
			// 
			this.sfxsMode.Controls.Add(this.label1);
			this.sfxsMode.Controls.Add(this.btnSFXPriority);
			this.sfxsMode.Controls.Add(this.sfxsTreeView);
			this.sfxsMode.Location = new System.Drawing.Point(4, 25);
			this.sfxsMode.Name = "sfxsMode";
			this.sfxsMode.Padding = new System.Windows.Forms.Padding(3);
			this.sfxsMode.Size = new System.Drawing.Size(489, 508);
			this.sfxsMode.TabIndex = 1;
			this.sfxsMode.Text = "SFXs";
			this.sfxsMode.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 484);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "SFX Priority";
			// 
			// btnSFXPriority
			// 
			this.btnSFXPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSFXPriority.Location = new System.Drawing.Point(85, 482);
			this.btnSFXPriority.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.btnSFXPriority.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.btnSFXPriority.Name = "btnSFXPriority";
			this.btnSFXPriority.Size = new System.Drawing.Size(71, 20);
			this.btnSFXPriority.TabIndex = 1;
			this.btnSFXPriority.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.btnSFXPriority.ValueChanged += new System.EventHandler(this.btnSFXPriority_ValueChanged);
			// 
			// sfxsTreeView
			// 
			this.sfxsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.sfxsTreeView.CheckBoxes = true;
			this.sfxsTreeView.HideSelection = false;
			this.sfxsTreeView.LabelEdit = true;
			this.sfxsTreeView.Location = new System.Drawing.Point(6, 6);
			this.sfxsTreeView.Name = "sfxsTreeView";
			this.sfxsTreeView.Size = new System.Drawing.Size(477, 470);
			this.sfxsTreeView.TabIndex = 0;
			this.sfxsTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.sfxsTreeView_AfterLabelEdit);
			this.sfxsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.sfxsTreeView_AfterCheck);
			this.sfxsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sfxsTreeView_AfterSelect);
			this.sfxsTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.sfxsTreeView_KeyUp);
			// 
			// unsupportedEffects
			// 
			this.unsupportedEffects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.unsupportedEffects.Controls.Add(this.unsupportedTreeView);
			this.unsupportedEffects.Location = new System.Drawing.Point(509, 214);
			this.unsupportedEffects.Name = "unsupportedEffects";
			this.unsupportedEffects.Size = new System.Drawing.Size(245, 342);
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
			this.unsupportedTreeView.Size = new System.Drawing.Size(233, 317);
			this.unsupportedTreeView.TabIndex = 0;
			// 
			// exportParams
			// 
			this.exportParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
			this.exportParams.Size = new System.Drawing.Size(245, 189);
			this.exportParams.TabIndex = 23;
			this.exportParams.TabStop = false;
			this.exportParams.Text = "Export Params";
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
			// exportBinaryDialog
			// 
			this.exportBinaryDialog.Filter = "Echo Stream Format|*.esf";
			this.exportBinaryDialog.Title = "Export Echo Stream Format";
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
			this.audioSection.ResumeLayout(false);
			this.songsMode.ResumeLayout(false);
			this.sfxsMode.ResumeLayout(false);
			this.sfxsMode.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnSFXPriority)).EndInit();
			this.unsupportedEffects.ResumeLayout(false);
			this.exportParams.ResumeLayout(false);
			this.exportParams.PerformLayout();
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
		private System.Windows.Forms.TabControl audioSection;
		private System.Windows.Forms.TabPage songsMode;
		private System.Windows.Forms.TabPage sfxsMode;
		private System.Windows.Forms.TreeView sfxsTreeView;
		private System.Windows.Forms.ToolStripMenuItem allEchoSFXsASMToolStripMenuItem;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown btnSFXPriority;
		private System.Windows.Forms.ToolStripMenuItem echoSFXsPrioritiesASMToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem toASMProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportChangeBitRateToolStripMenuItem;
	}
}

