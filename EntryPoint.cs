﻿//#define FULL_INFO

using DefleMaskConvert.DAO;
using DefleMaskConvert.DAO.DefleMask;
using DefleMaskConvert.DAO.Exporters.Echo;
using DefleMaskConvert.DAO.Importers.DMF;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DefleMaskConvert
{
	public partial class EntryPoint : Form
	{
		private const string INSTRUMENT_BINARY_NAME = "instr_{0}.{1}";
		private const int INSTRUMENT_BINARY_NAME_PAD_LEFT = 2;

		private ProjectData _project;
		private string _title;

		private CheckBox[] _channels;
		private RadioButton[] _PCMRates;
		private StringBuilder _message = new StringBuilder();

		public EntryPoint()
		{
			InitializeComponent();
			RefreshExportEchoButtons();

			unsupportedEffects.Visible = false;
			projectPanel.Visible = false;
			saveToolStripMenuItem.Enabled = false;
			importToolStripMenuItem.Enabled = false;
			_title = this.Text;

			_channels = new CheckBox[]
			{
				btnExportChannelFM1,
				btnExportChannelFM2,
				btnExportChannelFM3,
				btnExportChannelFM4,
				btnExportChannelFM5,
				btnExportChannelFM6,
				btnExportChannelPSG1,
				btnExportChannelPSG2,
				btnExportChannelPSG3,
				btnExportChannelPSGNoise
			};

			_PCMRates = new RadioButton[]
			{
				btnRateNotChange,
				btnRate26632,
				btnRate17755,
				btnRate13316,
				btnRate10653,
				btnRate8877,
				btnRate7609,
				btnRate6658,
				btnRate5918,
				btnRate5326,
				btnRate4842,
				btnRate4439,
				btnRate4097,
			};
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openDefleMaskDialog.ShowDialog() != DialogResult.OK) return;

			string path = openDefleMaskDialog.FileName;
			if (_project.CheckDuplicateSongFile(path))
			{
				_message.Clear();
				_message.AppendLine("The file is already in the project.");
				_message.Append("Path: ");
				_message.Append(path);
				ShowErrorMessage(_message.ToString());
				return;
			}

			var importer = new DMFImporter(path);
			DMFData data;
			var result = importer.ParseData(out data);
			data.FilePath = path;
			data.ExportName = GetSongExportName(path);

			string fileName = Path.GetFileName(openDefleMaskDialog.FileName);
			switch(result)
			{
				case DMFImporter.Result.UnknowFileFormat:
					ShowErrorMessage(string.Format("The file '{0}' is not a DefleMask DMF.", fileName));
					break;

				case DMFImporter.Result.UnsupportedFileVersion:
					_message.Clear();
					_message.AppendFormat("File version {0}(0x{1}) is not supperted.", data.FileVersion, data.FileVersion.ToString("X"));
					_message.AppendLine();
					_message.AppendLine("List os supported versions:");

					int total = Constants.GetTotalSupportedVersions();
					for (int i = 0; i < total; i++)
					{
						byte version;
						string name;
						Constants.GetDefleMaskVersion(i, out version, out name);
						_message.AppendFormat(" - {0}(0x{1}) - DefleMask v{2}", version, version.ToString("X"), name);
						_message.AppendLine();
					}

					ShowErrorMessage(_message.ToString(), string.Format("Error: {0}", fileName));
					break;

				case DMFImporter.Result.UnsupportedSystem:
					_message.Clear();
					_message.AppendFormat("The System {0} is not supperted.", data.System.ToString());
					_message.AppendLine();
					_message.AppendLine("Only Sega Genesis / Mega Drive is supported. (Note: Ext. CH3 is not supported)");

					ShowErrorMessage(_message.ToString(), string.Format("Error: {0}", fileName));
					break;

				case DMFImporter.Result.UnknowInstrumentType:
					_message.Clear();
					_message.AppendLine("Unknow Instrument type.");
					_message.AppendFormat("File version: {0}(0x{1})", data.FileVersion, data.FileVersion.ToString("X"));

					ShowErrorMessage(_message.ToString(), string.Format("Error: {0}", fileName));
					break;

				case DMFImporter.Result.UnsupportedSampleBits:
					_message.Clear();
					_message.AppendFormat("The file had one or more samples with unsupported bits size. You can only use {0} Bits samples.", Constants.SAMPLE_BITS);

					ShowErrorMessage(_message.ToString(), string.Format("Error: {0}", fileName));
					break;

				case DMFImporter.Result.OK:
					_project.Songs.Add(data);
					RefreshDetailsView();
					break;
			}

			unsupportedEffects.Visible = false;
			RefreshExportEchoButtons();
		}

		private string GetSongExportName(string path)
		{
			string name = Path.GetFileNameWithoutExtension(path);
			IEnumerable<string> names = from item in _project.Songs select item.ExportName;
			foreach(string existingName in names)
			{
				if(name == existingName)
					return GetNextDefaultName(names, name);
			}

			return name;
		}

		private void ShowErrorMessage(string message, string title = "Error")
		{
			SystemSounds.Beep.Play();
			MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void RefreshExportEchoButtons()
		{
			bool hadProject = _project != null;
			echoInstrumentsASMToolStripMenuItem.Enabled = hadProject;
			allEchoInstrumentsBinaryToolStripMenuItem.Enabled = hadProject;
			allEchoStreamFormatASMToolStripMenuItem.Enabled = hadProject && _project.Songs.Count > 0;
			allEchoSongsBinaryToolStripMenuItem.Enabled = hadProject && _project.Songs.Count > 0;

			bool enabled = songsTreeView.SelectedNode != null;
			echoStreamFormatASMToolStripMenuItem.Enabled = enabled;
			echoStreamFormatESFToolStripMenuItem.Enabled = enabled;
			exportParams.Enabled = enabled;
		}

		private void songsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			RefreshExportEchoButtons();
			DMFData dmf = (DMFData)e.Node.Tag;
			exportParams.Enabled = dmf != null;
			if(dmf != null) FillExportParams(dmf);
		}

		private void songsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			DMFData dmf = (DMFData)e.Node.Tag;
			dmf.Export = e.Node.Checked;
		}

		#region Export
		private void echoInstrumentsASMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportAssemblyDialog.FileName = "";
			exportAssemblyDialog.Title = "Export Instruments";
			if (exportAssemblyDialog.ShowDialog() != DialogResult.OK) return;
			
			Cursor.Current = Cursors.WaitCursor;
			List<Instrument> instruments = DMF2EchoInstruments.Convert(DMFGlobalInstruments.GetActiveInstruments(_project.Songs));
			try
			{
				EchoInstruments2ASM.SaveFile(exportAssemblyDialog.FileName, instruments);
				Cursor.Current = Cursors.Default;
			}
			catch(Exception)
			{
				Cursor.Current = Cursors.Default;
				_message.Clear();
				_message.AppendLine("Unable to export file:");
				
				_message.Append(" - ");
				_message.AppendLine(exportAssemblyDialog.FileName);

				ShowErrorMessage(_message.ToString());
			}
		}

		private void echoStreamFormatASMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DMFData dmf = (DMFData)songsTreeView.SelectedNode.Tag;
			exportAssemblyDialog.FileName = dmf.ExportName;
			exportAssemblyDialog.Title = "Export Echo Stream Format";
			if (exportAssemblyDialog.ShowDialog() != DialogResult.OK) return;
			
			Cursor.Current = Cursors.WaitCursor;
			EchoESF data = DMF2EchoESF.Convert(dmf, DMFGlobalInstruments.GetActiveInstruments(_project.Songs));

			try
			{
				EchoESM2ASM.SaveFile(exportAssemblyDialog.FileName, data);
				Cursor.Current = Cursors.Default;
			}
			catch (Exception)
			{
				Cursor.Current = Cursors.Default;
				_message.Clear();
				_message.AppendLine("Unable to export file:");

				_message.Append(" - ");
				_message.AppendLine(exportAssemblyDialog.FileName);

				ShowErrorMessage(_message.ToString());
			}
		}

		private void echoStreamFormatESFToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DMFData dmf = (DMFData)songsTreeView.SelectedNode.Tag;
			exportBinaryDialog.FileName = dmf.ExportName;
			if (exportBinaryDialog.ShowDialog() != DialogResult.OK) return;

			Cursor.Current = Cursors.WaitCursor;
			EchoESF data = DMF2EchoESF.Convert(dmf, DMFGlobalInstruments.GetActiveInstruments(_project.Songs));

			try
			{
				File.WriteAllBytes(exportBinaryDialog.FileName, data.GetBinaryData());
				Cursor.Current = Cursors.Default;
			}
			catch (Exception)
			{
				Cursor.Current = Cursors.Default;
				_message.Clear();
				_message.AppendLine("Unable to export file:");

				_message.Append(" - ");
				_message.AppendLine(exportBinaryDialog.FileName);

				ShowErrorMessage(_message.ToString());
			}
		}

		private void allEchoStreamFormatASMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (exportFolderBrowserDialog.ShowDialog() != DialogResult.OK) return;

			Cursor.Current = Cursors.WaitCursor;
			var fails = new List<string>();
			var activeInstruments = DMFGlobalInstruments.GetActiveInstruments(_project.Songs);

			foreach(var song in _project.Songs)
			{
				if (!song.Export) continue;

				EchoESF data = DMF2EchoESF.Convert(song, activeInstruments);
				string path = Path.Combine(exportFolderBrowserDialog.SelectedPath, song.ExportName + ".asm");
				try
				{
					EchoESM2ASM.SaveFile(path, data);
				}
				catch (Exception)
				{
					fails.Add(path);
				}
			}

			Cursor.Current = Cursors.Default;
			if (fails.Count > 0)
			{
				_message.Clear();
				_message.AppendLine("Unable to export one or more files.");
				foreach (var path in fails)
				{
					_message.Append(" - ");
					_message.AppendLine(path);
				}

				ShowErrorMessage(_message.ToString());
			}
		}

		private void allEchoSongsBinaryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (exportFolderBrowserDialog.ShowDialog() != DialogResult.OK) return;

			Cursor.Current = Cursors.WaitCursor;
			var fails = new List<string>();
			var activeInstruments = DMFGlobalInstruments.GetActiveInstruments(_project.Songs);

			foreach (var song in _project.Songs)
			{
				if (!song.Export) continue;

				EchoESF data = DMF2EchoESF.Convert(song, activeInstruments);
				string path = Path.Combine(exportFolderBrowserDialog.SelectedPath, song.ExportName + ".esf");
				try
				{
					File.WriteAllBytes(path, data.GetBinaryData());
				}
				catch (Exception)
				{
					fails.Add(path);
				}
			}

			Cursor.Current = Cursors.Default;
			if (fails.Count > 0)
			{
				_message.Clear();
				_message.AppendLine("Unable to export one or more files.");
				foreach (var path in fails)
				{
					_message.Append(" - ");
					_message.AppendLine(path);
				}

				ShowErrorMessage(_message.ToString());
			}
		}

		private void allEchoInstrumentsBinaryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (exportFolderBrowserDialog.ShowDialog() != DialogResult.OK) return;

			Cursor.Current = Cursors.WaitCursor;
			var fails = new List<string>();

			List<Instrument> instruments = DMF2EchoInstruments.Convert(DMFGlobalInstruments.GetActiveInstruments(_project.Songs));
			int index = -1;
			foreach(var data in instruments)
			{
				index++;
				string extension = null;
				if (data is FMInstrument) extension = "eif";
				if (data is PSGInstrument) extension = "eef";
				if (data is SampleInstrument) extension = "ewf";

				if (extension == null) continue;

				string fileName = string.Format(INSTRUMENT_BINARY_NAME, index.ToString().PadLeft(INSTRUMENT_BINARY_NAME_PAD_LEFT, '0'), extension);
				string path = Path.Combine(exportFolderBrowserDialog.SelectedPath, fileName);

				try
				{
					File.WriteAllBytes(path, data.GetBinaryData());
				}
				catch (Exception)
				{
					fails.Add(path);
				}
			}

			if (fails.Count > 0)
			{
				_message.Clear();
				_message.AppendLine("Unable to export one or more files.");
				foreach (var path in fails)
				{
					_message.Append(" - ");
					_message.AppendLine(path);
				}

				ShowErrorMessage(_message.ToString());
			}

			Cursor.Current = Cursors.Default;
		}
		#endregion

		#region Export Properties
		private bool _isChanging = false;
		private void FillExportParams(DMFData data)
		{
			_isChanging = true;

			btnLockChannels.Checked = data.LockChannels;
			_PCMRates[(int)data.PCMRate].Checked = true;

			if (data.IsLoopJumpSet())
			{
				btnLoopWholeTrack.Checked = false;
				btnLoopWholeTrack.Enabled = false;
			}
			else
			{
				btnLoopWholeTrack.Enabled = true;
				btnLoopWholeTrack.Checked = data.LoopWholeTrack;
			}

			bool psg3Enbaled = !data.IsUsingPSGNoiseFrequency();
			for (int i = 0; i < _channels.Length; i++)
			{
				var checkbox = _channels[i];
				bool channelEnabled = i < data.Channels.Count;
				checkbox.Enabled = channelEnabled;
				if (channelEnabled)
					checkbox.Checked = data.Channels[i].Export;

				if (checkbox == btnExportChannelPSG3)
				{
					data.Channels[i].Export = data.Channels[i+1].Export;
					checkbox.Enabled = psg3Enbaled;
					checkbox.Checked = data.Channels[i].Export;
				}
			}

			var unsupported = data.GetUnsupportedEffects();
			unsupportedEffects.Visible = unsupported.Count > 0;
			if (unsupported.Count > 0)
			{
				unsupportedTreeView.Nodes.Clear();
				foreach(var effect in unsupported)
				{
					string name = (sbyte)effect < 0 && (sbyte)effect >= -2 ? effect.ToString() : string.Format("{0} (0x{1})", effect.ToString(), ((int)effect).ToString("X").PadLeft(2, '0'));
					unsupportedTreeView.Nodes.Add(new TreeNode(name));
				}
			}

			_isChanging = false;
		}

		private void btnExportChannelCheckedChanged(object sender, EventArgs e)
		{
			if (_isChanging) return;

			DMFData dmf = (DMFData)songsTreeView.SelectedNode.Tag;
			CheckBox clicked = (CheckBox)sender;
			int index = Array.IndexOf<CheckBox>(_channels, clicked);
			dmf.Channels[index].Export = clicked.Checked;

			if (clicked == btnExportChannelPSGNoise && dmf.IsUsingPSGNoiseFrequency())
			{
				dmf.Channels[index - 1].Export = clicked.Checked;
				btnExportChannelPSG3.Checked = clicked.Checked;
			}
		}

		private void btnLockChannels_CheckedChanged(object sender, EventArgs e)
		{
			if (_isChanging) return;

			DMFData dmf = (DMFData)songsTreeView.SelectedNode.Tag;
			dmf.LockChannels = btnLockChannels.Checked;
		}

		private void btnLoopWholeTrack_CheckedChanged(object sender, EventArgs e)
		{
			if (_isChanging) return;

			DMFData dmf = (DMFData)songsTreeView.SelectedNode.Tag;
			dmf.LoopWholeTrack = btnLoopWholeTrack.Checked;
		}

		private void btnPCMRateCheckedChanged(object sender, EventArgs e)
		{
			if (_isChanging) return;
			_isChanging = true;

			DMFData dmf = (DMFData)songsTreeView.SelectedNode.Tag;
			RadioButton radio = (RadioButton)sender;
			for (int i = 0; i < _PCMRates.Length; i++)
			{
				if (_PCMRates[i] == radio)
				{
					dmf.PCMRate = (ESF_PCMRate)i;
					break;
				}
			}

			_isChanging = false;
		}
		#endregion

		#region Details View
		private void RefreshDetailsView()
		{
			songsTreeView.SelectedNode = null;
			songsTreeView.Nodes.Clear();
			foreach (var data in _project.Songs)
			{
				songsTreeView.Nodes.Add(CreateDetailsView(data));
			}
		}

		private void songsTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (e.Label == null) return;

			var invalidChars = Path.GetInvalidFileNameChars();
			if (e.Label.IndexOfAny(invalidChars) >= 0)
			{
				e.CancelEdit = true;
				ShowErrorMessage("The name contains invalid chars.");
			}
			else if (_project.CheckDuplicateExportName(e.Label))
			{
				e.CancelEdit = true;
				ShowErrorMessage(string.Format("The name '{0}' already exist.", e.Label));
			}
			else
			{
				DMFData data = (DMFData)e.Node.Tag;
				data.ExportName = e.Label;
			}
		}

		private void songsTreeView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && songsTreeView.SelectedNode != null)
			{
				DMFData data = (DMFData)songsTreeView.SelectedNode.Tag;
				_project.Songs.Remove(data);

				RefreshDetailsView();
				unsupportedEffects.Visible = false;
				exportParams.Enabled = false;
				RefreshExportEchoButtons();
			}
		}

		private void songsTreeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			// Move the dragged node when the left mouse button is used. 
			if (e.Button == MouseButtons.Left)
				songsTreeView.DoDragDrop(e.Item, DragDropEffects.Move);
		}

		private void songsTreeView_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.None)
				return;

			TreeNode node = (TreeNode)e.Data.GetData(typeof(TreeNode));
			Point pos = new Point(e.X, e.Y);
			switch (e.Effect)
			{
				case DragDropEffects.Move:
					MoveNode(node, pos);
					break;
				//case DragDropEffects.Copy:
				//    CopyNode(node, pos);
				//    break;
			}
		}

		private void MoveNode(TreeNode dragged, Point pos)
		{
			TreeNode _overNode = songsTreeView.GetNodeAt(songsTreeView.PointToClient(pos));
			TreeNodeCollection nodes = _overNode != null && _overNode.Parent != null ? _overNode.Parent.Nodes : songsTreeView.Nodes;
			int index1, index2;

			if (_overNode != null)
			{
				if (_overNode.Parent == dragged.Parent)
				{
					index1 = nodes.IndexOf(dragged);
					index2 = nodes.IndexOf(_overNode);

					SwapItems(index1, index2);
				}
			}
			else
			{
				index1 = nodes.IndexOf(dragged);
				index2 = nodes.Count - 1;
				SwapItems(index1, index2);
			}
		}

		private void SwapItems(int index1, int index2)
		{
			var temp = _project.Songs[index1];
			_project.Songs[index1] = _project.Songs[index2];
			_project.Songs[index2] = temp;

			RefreshDetailsView();
			unsupportedEffects.Visible = false;
			exportParams.Enabled = false;
			RefreshExportEchoButtons();
		}

		private void songsTreeView_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}

		private TreeNode CreateDetailsView(DMFData data)
		{
			TreeNode root = new TreeNode(data.ExportName);
			root.Checked = data.Export;
			root.Tag = data;

#if FULL_INFO
			root.Nodes.Add(new TreeNode("Song Name: " + data.SongName));
			root.Nodes.Add(new TreeNode("Author: " + data.SongAuthor));
			root.Nodes.Add(new TreeNode("Time Base: " + data.TimeBase));
			root.Nodes.Add(new TreeNode("Tick Time Even: " + data.TickTimeEven));
			root.Nodes.Add(new TreeNode("Tick Time Odd: " + data.TickTimeOdd));

			TreeNode instruments = new TreeNode("Instruments");
			foreach (var instrument in data.Instruments)
			{
				TreeNode instrumentNode = new TreeNode(instrument.Name);
				instruments.Nodes.Add(instrumentNode);

				if (instrument is FMInstrumentData)
				{
					instrumentNode.Text = instrumentNode.Text + " (FM)";
					var fm = instrument as FMInstrumentData;

					instrumentNode.Nodes.Add(new TreeNode("Alg: " + fm.Alg));
					instrumentNode.Nodes.Add(new TreeNode("Fb: " + fm.Fb));
					instrumentNode.Nodes.Add(new TreeNode("LFO: " + fm.LFO));
					instrumentNode.Nodes.Add(new TreeNode("LFO2: " + fm.LFO2));

					TreeNode operators = new TreeNode("Operators");
					instrumentNode.Nodes.Add(operators);
					foreach (var op in fm.Operators)
					{
						TreeNode opNode = new TreeNode("Op " + fm.Operators.IndexOf(op));
						operators.Nodes.Add(opNode);

						opNode.Nodes.Add(new TreeNode("Am: " + op.Am));
						opNode.Nodes.Add(new TreeNode("Ar: " + op.Ar));
						opNode.Nodes.Add(new TreeNode("Dr: " + op.Dr));
						opNode.Nodes.Add(new TreeNode("Mult: " + op.Mult));
						opNode.Nodes.Add(new TreeNode("Rr: " + op.Rr));
						opNode.Nodes.Add(new TreeNode("Sl: " + op.Sl));
						opNode.Nodes.Add(new TreeNode("Tl: " + op.Tl));
						opNode.Nodes.Add(new TreeNode("Dt2: " + op.Dt2));
						opNode.Nodes.Add(new TreeNode("Rs: " + op.Rs));
						opNode.Nodes.Add(new TreeNode("Dt: " + op.Dt));
						opNode.Nodes.Add(new TreeNode("D2r: " + op.D2r));
						opNode.Nodes.Add(new TreeNode("SsgMode: " + op.SsgMode));
					}
				}

				if (instrument is PSGInstrumentData)
				{
					instrumentNode.Text = instrumentNode.Text + " (PSG)";
					var psg = instrument as PSGInstrumentData;

					instrumentNode.Nodes.Add(CreatePSGEnvelopeDetails(psg.Volume, "Volume"));

					TreeNode arpeggio = CreatePSGEnvelopeDetails(psg.Arpeggio, "Arpeggio");
					instrumentNode.Nodes.Add(arpeggio);
					arpeggio.Nodes.Add(new TreeNode("Mode: " + psg.ArpeggioMode.ToString()));

					instrumentNode.Nodes.Add(CreatePSGEnvelopeDetails(psg.DutyNoise, "Duty Noise"));
					instrumentNode.Nodes.Add(CreatePSGEnvelopeDetails(psg.WaveTable, "Wave Table"));
				}
			}
			root.Nodes.Add(instruments);

			TreeNode waveTables = new TreeNode("Wave Tables");
			foreach (var wave in data.WaveTables)
			{
				TreeNode table = new TreeNode(string.Format("Wave Table (Size {0})", wave.Size));
				waveTables.Nodes.Add(table);

				for (int i = 0; i < wave.Size; i++)
				{
					table.Nodes.Add(new TreeNode("0x" + wave.Data[i].ToString("X").PadLeft(8, '0')));
				}
			}
			root.Nodes.Add(waveTables);

			TreeNode samples = new TreeNode("Samples");
			foreach (var sample in data.Samples)
			{
				TreeNode sampleNode = new TreeNode(sample.Name);
				samples.Nodes.Add(sampleNode);

				sampleNode.Nodes.Add(new TreeNode("Size: " + sample.Size));
				sampleNode.Nodes.Add(new TreeNode("Sample Rate: " + sample.SampleRate));
				sampleNode.Nodes.Add(new TreeNode("Pitch: " + sample.Pitch));
				sampleNode.Nodes.Add(new TreeNode("Amp: " + sample.Amp));
				sampleNode.Nodes.Add(new TreeNode("Sample Bits: " + sample.SampleBits));

				TreeNode dataNode = new TreeNode("Data");
				sampleNode.Nodes.Add(dataNode);
				for (int i = 0; i < sample.Size; i++)
				{
					dataNode.Nodes.Add(new TreeNode(string.Format("0x{0}", sample.Data[i].ToString("X").PadLeft(4, '0'))));
				}
			}
			root.Nodes.Add(samples);

			root.Nodes.Add(CreateChannelsDetails(data));
#endif

			return root;
		}

#if FULL_INFO
		private TreeNode CreatePSGEnvelopeDetails(PSGEnvelopeData data, string name)
		{
			TreeNode node = new TreeNode(name);
			if (data != null)
			{
				node.Nodes.Add(new TreeNode("Size: " + data.Size));

				if (data.Size > 0)
				{
					TreeNode dataNode = new TreeNode("Data");
					node.Nodes.Add(dataNode);
					for (int i = 0; i < data.Size; i++)
					{
						dataNode.Nodes.Add(new TreeNode("0x" + data.Data[i].ToString("X").PadLeft(8, '0')));
					}

					node.Nodes.Add(new TreeNode("Loop Position: " + data.LoopPosition));
				}
			}

			return node;
		}

		private TreeNode CreateChannelsDetails(DMFData data)
		{
			TreeNode root = new TreeNode("Channels");
			var textBuilder = new StringBuilder();

			foreach (var channel in data.Channels)
			{
				int index = data.Channels.IndexOf(channel) + 1;
				TreeNode channelNode = new TreeNode("Ch " + index);
				root.Nodes.Add(channelNode);

				for (int i = 0; i < channel.Pages.Length; i++)
				{
					PatternPage page = channel.Pages[i];
					TreeNode pageNode = new TreeNode("Page " + i);
					channelNode.Nodes.Add(pageNode);

					for (int j = 0; j < page.Notes.Length; j++)
					{
						NoteData note = page.Notes[j];
						textBuilder.Clear();

						ushort noteValue = note.Note;
						ushort octave = note.Octave;
						if (noteValue >= Constants.LAST_NOTE)
						{
							noteValue = 0;
							octave++;
						}

						textBuilder.Append(note.IsEmpty ? "---" : note.IsOff ? "OFF" : Constants.GetNoteName(noteValue) + octave);
						textBuilder.AppendFormat(" ({0})", note.Volume > -1 ? note.Volume.ToString("X").PadLeft(2, '0') : "-");
						textBuilder.AppendFormat(" {0}", note.Instrument > -1 ? note.Instrument.ToString() : "-");

						string effectInfo = " [-|-]";
						if (note.Effects.Length > 0)
						{
							NoteEffect effect = note.Effects[0];
							if (effect.Type > -1)
								effectInfo = string.Format(" [{0}|{1}]", effect.Type.ToString("X").PadLeft(2, '0'), effect.Value.ToString("X").PadLeft(2, '0'));
						}
						textBuilder.Append(effectInfo);

						TreeNode noteNode = new TreeNode(textBuilder.ToString());
						pageNode.Nodes.Add(noteNode);
					}
				}
			}

			return root;
		}
#endif
		#endregion

		#region Project
		private void CreateProject()
		{
			_project = new ProjectData();
			InitProject();
		}

		private void InitProject()
		{
			RefreshTitle();
			saveToolStripMenuItem.Enabled = true;
			importToolStripMenuItem.Enabled = true;
			exportToolStripMenuItem.Enabled = true;
			projectPanel.Visible = true;

			if (_project.SaveSongs.Count > 0)
			{
				string rootPath = _project.GetProjectFolderPath();
				var fails = new Dictionary<DMFImporter.Result, List<string>>();
				List<string> errors;

				foreach (var song in _project.SaveSongs)
				{
					string path = song.Path;
					string root = Path.GetPathRoot(path);
					if (string.IsNullOrWhiteSpace(root))
						path = Path.Combine(rootPath, path);

					DMFImporter importer;
					try
					{
						importer = new DMFImporter(path);
					}
					catch (Exception)
					{
						if (!fails.TryGetValue(DMFImporter.Result.UnableToOpenFile, out errors))
						{
							errors = new List<string>();
							fails.Add(DMFImporter.Result.UnableToOpenFile, errors);
						}
						errors.Add(path);
						continue;
					}

					DMFData data;
					var result = importer.ParseData(out data);
					data.FilePath = path;

					string fileName = Path.GetFileName(openDefleMaskDialog.FileName);
					switch (result)
					{
						case DMFImporter.Result.UnknowFileFormat:
							if (!fails.TryGetValue(DMFImporter.Result.UnknowFileFormat, out errors))
							{
								errors = new List<string>();
								fails.Add(DMFImporter.Result.UnknowFileFormat, errors);
							}
							errors.Add(path);
							break;

						case DMFImporter.Result.UnsupportedFileVersion:
							if (!fails.TryGetValue(DMFImporter.Result.UnsupportedFileVersion, out errors))
							{
								errors = new List<string>();
								fails.Add(DMFImporter.Result.UnsupportedFileVersion, errors);
							}
							errors.Add(path);
							break;

						case DMFImporter.Result.UnsupportedSystem:
							if (!fails.TryGetValue(DMFImporter.Result.UnsupportedSystem, out errors))
							{
								errors = new List<string>();
								fails.Add(DMFImporter.Result.UnsupportedSystem, errors);
							}
							errors.Add(path);
							break;

						case DMFImporter.Result.UnknowInstrumentType:
							if (!fails.TryGetValue(DMFImporter.Result.UnknowInstrumentType, out errors))
							{
								errors = new List<string>();
								fails.Add(DMFImporter.Result.UnknowInstrumentType, errors);
							}
							errors.Add(path);
							break;

						case DMFImporter.Result.UnsupportedSampleBits:
							if (!fails.TryGetValue(DMFImporter.Result.UnsupportedSampleBits, out errors))
							{
								errors = new List<string>();
								fails.Add(DMFImporter.Result.UnsupportedSampleBits, errors);
							}
							errors.Add(path);
							break;

						case DMFImporter.Result.OK:
							data.ExportName = song.ExportName;
							data.Export = song.Export;
							data.LockChannels = song.LockChannels;
							data.LoopWholeTrack = song.LoopWholeTrack;
							data.PCMRate = song.PCMRate;

							for (int i = 0; i < song.ExportChannels.Count && i < data.Channels.Count; i++)
							{
								data.Channels[i].Export = song.ExportChannels[i];
							}

							_project.Songs.Add(data);
							break;
					}
				}

				if (fails.Count > 0)
				{
					_message.Clear();
					_message.AppendLine("Unable to import one or more files. Fails list:");
					foreach(var pair in fails)
					{
						_message.AppendFormat(" * {0}:", pair.Key.ToString());
						_message.AppendLine();
						foreach(string path in pair.Value)
						{
							_message.AppendLine("   - " + path);
						}
					}

					ShowErrorMessage(_message.ToString());
				}
			}

			RefreshDetailsView();
			unsupportedEffects.Visible = false;
			RefreshExportEchoButtons();
		}

		private void RefreshTitle()
		{
			this.Text = _title + ": " + _project.Name;
		}

		private bool TrySaveProject()
		{
			if (_project != null && //_undoManager.CanUndo &&
				MessageBox.Show("Save current project?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
				return SaveProject();

			return true;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			TrySaveProject();
			base.OnClosing(e);
		}

		private bool SaveProject()
		{
			if (_project.FilePath == null && saveProjectDialog.ShowDialog() == DialogResult.OK)
				_project.FilePath = saveProjectDialog.FileName;

			if (_project.FilePath == null)
			{
				MessageBox.Show("Project not saved.", "Fail", MessageBoxButtons.OK);
				return false;
			}

			Console.WriteLine("Tengo que guardarlo en: " + _project.FilePath);
			File.WriteAllLines(_project.FilePath, new string[] { string.Empty });

			_project.PrepareToSave();
			using (FileStream stream = File.OpenWrite(_project.FilePath))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(ProjectData));
				serializer.Serialize(stream, _project);
			}

			RefreshTitle();
			return true;
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (TrySaveProject())
				CreateProject();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveProject();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openProjectDialog.ShowDialog() == DialogResult.OK)
			{
				if (TrySaveProject())
				{
					using (FileStream fs = new FileStream(openProjectDialog.FileName, FileMode.Open))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(ProjectData));
						_project = (ProjectData)serializer.Deserialize(fs);
					}

					_project.FilePath = openProjectDialog.FileName;
					InitProject();
				}
			}
		}
		#endregion

		private const string REGEX_CONFIG = "^({0}(\\d+))$";
		private const string DEFAULT_NAME = "{0}{1}";
		static public string GetNextDefaultName(IEnumerable<string> names, string prefix)
		{
			List<int> indexes = new List<int>();
			foreach (var item in names)
			{
				Match result = Regex.Match(item, string.Format(REGEX_CONFIG, prefix));

				if (result.Groups.Count >= 2)
				{
					string value = result.Groups[2].Value;
					if (value.Length <= 1 || value[0] != '0')
						indexes.Add(int.Parse(value));
				}
			}

			for (int i = 0; i < indexes.Count; i++)
			{
				for (int j = i + 1; j < indexes.Count; j++)
				{
					if (indexes[j] < indexes[i])
					{
						int aux = indexes[i];
						indexes[i] = indexes[j];
						indexes[j] = aux;
					}
				}
			}

			int index = 0;
			while (index < indexes.Count && index == indexes[index])
				index++;

			return string.Format(DEFAULT_NAME, prefix, index);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutPopUp().ShowDialog();
		}

	}
}
