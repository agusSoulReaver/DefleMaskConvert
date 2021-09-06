//#define FULL_INFO

using DefleMaskConvert.DAO;
using DefleMaskConvert.DAO.DefleMask;
using DefleMaskConvert.DAO.Exporters.Echo;
using DefleMaskConvert.DAO.Exporters;
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
		private const string SFX_NAME_MULTI = "{0}_{1}";
		private const string SFX_NAME_SIMPLE = "{0}";
		private const int INSTRUMENT_BINARY_NAME_PAD_LEFT = 2;

		private ProjectData _project;
		private string _title;

		private CheckBox[] _channels;
		//private RadioButton[] _PCMRates;
		private StringBuilder _message = new StringBuilder();

		public EntryPoint()
		{
			InitializeComponent();
			RefreshExportEchoButtons();

			unsupportedEffects.Visible = false;
			projectPanel.Visible = false;
			saveToolStripMenuItem.Enabled = false;
			importToolStripMenuItem.Enabled = false;
			settingsToolStripMenuItem.Enabled = false;
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

			//_PCMRates = new RadioButton[]
			//{
			//	btnRateNotChange,
			//	btnRate26632,
			//	btnRate17755,
			//	btnRate13316,
			//	btnRate10653,
			//	btnRate8877,
			//	btnRate7609,
			//	btnRate6658,
			//	btnRate5918,
			//	btnRate5326,
			//	btnRate4842,
			//	btnRate4439,
			//	btnRate4097,
			//};
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
					if (IsSFXMode())
					{
						data.ExportName = GetSongExportName(path, from item in _project.SFXs select item.ExportName);
						var sfx = new SFXData();
						sfx.Path = data.FilePath;
						sfx.Export = data.Export;
						sfx.Source = data;
						sfx.ExportName = data.ExportName;
						for (int i = 0; i < data.PatternPages; i++)
						{
							var fx = new DMFData(data, i);
							sfx.FXs.Add(fx);
						}
						FilterSFXsNames(sfx);
						_project.SFXs.Add(sfx);
						RefreshSFXsView();
					}
					else
					{
						data.ExportName = GetSongExportName(path, from item in _project.Songs select item.ExportName);
						_project.Songs.Add(data);
						RefreshDetailsView();
					}
					break;
			}

			unsupportedEffects.Visible = false;
			RefreshExportEchoButtons();
		}

		private bool IsSFXMode()
		{
			return audioSection.SelectedTab == sfxsMode;
		}

		private void audioSection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (IsSFXMode())
				exportParams.Enabled = false;
			else
				RefreshExportEchoButtons();
		}

		private string GetSongExportName(string path, IEnumerable<string> names)
		{
			string name = Path.GetFileNameWithoutExtension(path);
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

            toASMProjectToolStripMenuItem.Enabled = hadProject;

			bool enabled = songsTreeView.SelectedNode != null;
			echoStreamFormatASMToolStripMenuItem.Enabled = enabled;
			echoStreamFormatESFToolStripMenuItem.Enabled = enabled;
			exportParams.Enabled = enabled;

			enabled = IsSFXMode();
			echoSFXsPrioritiesASMToolStripMenuItem.Enabled = enabled;
			allEchoSFXsASMToolStripMenuItem.Enabled = enabled;
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

		private void sfxsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			SFXData containerData = e.Node.Tag as SFXData;
			if (containerData == null)
			{
				DMFData sfx = (DMFData)e.Node.Tag;
				sfx.Export = e.Node.Checked;
			}
			else
				containerData.Export = e.Node.Checked;
		}

		#region Export
		private List<InstrumentData> TryGetInstrumentsData()
		{
			var data = DMFGlobalInstruments.GetActiveInstruments(_project.Songs, _project.SFXs);
			var errors = DMFGlobalInstruments.GetSampleErrors();
			if (errors.Count > 0)
			{
				_message.Clear();
				_message.AppendLine("Unable to find sample:");

				foreach (var e in errors)
				{
					_message.Append(" - ");
					_message.AppendLine(e);
				}

				ShowErrorMessage(_message.ToString());
				return null;
			}

			return data;
		}

		private void echoInstrumentsASMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportAssemblyDialog.FileName = "";
			exportAssemblyDialog.Title = "Export Instruments";
			if (exportAssemblyDialog.ShowDialog() != DialogResult.OK) return;
			
			Cursor.Current = Cursors.WaitCursor;
			var instrumentData = TryGetInstrumentsData();
			if (instrumentData == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}

			List<Instrument> instruments = DMF2EchoInstruments.Convert(instrumentData);
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
			var instrumentData = TryGetInstrumentsData();
			if (instrumentData == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}
			EchoESF data = DMF2EchoESF.Convert(dmf, instrumentData);

			try
			{
				EchoESM2ASM.SaveFile(exportAssemblyDialog.FileName, data, dmf.SongName, dmf.SongAuthor);
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
			var instrumentData = TryGetInstrumentsData();
			if (instrumentData == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}
			EchoESF data = DMF2EchoESF.Convert(dmf, instrumentData);

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
			var instrumentData = TryGetInstrumentsData();
			if (instrumentData == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}

			var fails = new List<string>();
			foreach(var song in _project.Songs)
			{
				if (!song.Export) continue;

				EchoESF data = DMF2EchoESF.Convert(song, instrumentData);
				string path = Path.Combine(exportFolderBrowserDialog.SelectedPath, song.ExportName + ".asm");
				try
				{
					EchoESM2ASM.SaveFile(path, data, song.SongName, song.SongAuthor);
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
			var instrumentData = TryGetInstrumentsData();
			if (instrumentData == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}

			var fails = new List<string>();
			foreach (var song in _project.Songs)
			{
				if (!song.Export) continue;

				EchoESF data = DMF2EchoESF.Convert(song, instrumentData);
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
			var instrumentData = TryGetInstrumentsData();
			if (instrumentData == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}

			var fails = new List<string>();
			List<Instrument> instruments = DMF2EchoInstruments.Convert(instrumentData);
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

		static private readonly List<EchoESF> _sfxs = new List<EchoESF>();
		private void allEchoSFXsASMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportAssemblyDialog.FileName = "";
			exportAssemblyDialog.Title = "Export SFXs";
			if (exportAssemblyDialog.ShowDialog() != DialogResult.OK) return;

			Cursor.Current = Cursors.WaitCursor;
			var activeInstruments = TryGetInstrumentsData();
			if (activeInstruments == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}

			_sfxs.Clear();
			var fails = new List<string>();

			foreach (var container in _project.SFXs)
			{
				if (!container.Export) continue;

				string nameFormat = container.FXs.Count > 1? SFX_NAME_MULTI: SFX_NAME_SIMPLE;
				foreach (var fx in container.FXs)
				{
					if (!fx.Export) continue;

					EchoESF data = DMF2EchoESF.Convert(fx, activeInstruments);
					data.ExportName = string.Format(nameFormat, container.ExportName, fx.ExportName);
					_sfxs.Add(data);
				}
			}

			try
			{
				EchoESM2ASM.SaveFile(exportAssemblyDialog.FileName, _sfxs, _project.ExportChangeBitRateAction);
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

		private void echoSFXsPrioritiesASMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportAssemblyDialog.FileName = "";
			exportAssemblyDialog.Title = "Export SFXs Priorities";
			if (exportAssemblyDialog.ShowDialog() != DialogResult.OK) return;

			var data = GetSFXsPrioritiesData();
			try
			{
				SFXPrioritiesExporter.SaveFile(exportAssemblyDialog.FileName, data);
			}
			catch (Exception)
			{
				_message.Clear();
				_message.AppendLine("Unable to export file:");

				_message.Append(" - ");
				_message.AppendLine(exportAssemblyDialog.FileName);

				ShowErrorMessage(_message.ToString());
			}
		}

		private Dictionary<string, byte> GetSFXsPrioritiesData()
		{
			var data = new Dictionary<string, byte>();
			foreach (var container in _project.SFXs)
			{
				if (!container.Export) continue;

				string nameFormat = container.FXs.Count > 1 ? SFX_NAME_MULTI : SFX_NAME_SIMPLE;
				nameFormat = nameFormat + "Priority";
				foreach (var fx in container.FXs)
				{
					if (!fx.Export) continue;

					string key = string.Format(nameFormat, container.ExportName, fx.ExportName);
					data.Add(key, fx.Priority);
				}
			}

			return data;
		}

		private void toASMProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(exportFolderBrowserDialog.SelectedPath))
			{
				exportFolderBrowserDialog.SelectedPath = Path.GetDirectoryName(_project.FilePath);
			}

			if (exportFolderBrowserDialog.ShowDialog() != DialogResult.OK) return;

			Cursor.Current = Cursors.WaitCursor;
			_sfxs.Clear();
			var fails = new List<string>();
			var includes = new List<string>();
			var includesWLabel = new List<KeyValuePair<string, string>>();
			var songReferences = new Dictionary<string,string>();
			string pathBase = exportFolderBrowserDialog.SelectedPath;

			// exporting instruments
			Cursor.Current = Cursors.WaitCursor;
			var activeInstruments = TryGetInstrumentsData();
			if (activeInstruments == null)
			{
				Cursor.Current = Cursors.Default;
				return;
			}

			List<Instrument> instruments = DMF2EchoInstruments.Convert(activeInstruments);
			string path = Path.Combine(pathBase, "instruments.asm");
			try
			{
				EchoInstruments2ASM.SaveFile(path, instruments);
				includes.Add(path);
			}
			catch (Exception)
			{
				fails.Add(path);
			}

			// exporting songs
			Directory.CreateDirectory(Path.Combine(pathBase, "music"));
			foreach (var song in _project.Songs)
			{
				if (!song.Export) continue;

				EchoESF data = DMF2EchoESF.Convert(song, activeInstruments);
				path = Path.Combine(pathBase, "music", song.ExportName + ".asm");
				string label = string.Format("BGM_{0}", song.ExportName);

				try
				{
					EchoESM2ASM.SaveFile(path, data, song.SongName, song.SongAuthor);
					includesWLabel.Add(new KeyValuePair<string,string>(label, path));
					songReferences.Add(label, song.ExportName);
				}
				catch (Exception)
				{
					fails.Add(path);
				}
			}

			// exporting SFX
			foreach (var container in _project.SFXs)
			{
				if (!container.Export) continue;

				string nameFormat = container.FXs.Count > 1 ? SFX_NAME_MULTI : SFX_NAME_SIMPLE;
				foreach (var fx in container.FXs)
				{
					if (!fx.Export) continue;

					EchoESF data = DMF2EchoESF.Convert(fx, activeInstruments);
					data.ExportName = string.Format(nameFormat, container.ExportName, fx.ExportName);
					_sfxs.Add(data);
				}
			}

			path = Path.Combine(pathBase, "sfxs.asm");
			try
			{
				EchoESM2ASM.SaveFile(path, _sfxs, _project.ExportChangeBitRateAction);
				includes.Add(path);
			}
			catch (Exception)
			{
				fails.Add(path);
			}

			// exporting priorities
			var priorities = GetSFXsPrioritiesData();
			path = Path.Combine(pathBase, "priorities.asm");
			try
			{
				SFXPrioritiesExporter.SaveFile(path, priorities);
				includes.Add(path);
			}
			catch (Exception)
			{
				fails.Add(path);
			}

			// exporting link file
			for (int i = 0; i < pathBase.Length && i < _project.FilePath.Length; i++)
			{
				if (pathBase[i] != _project.FilePath[i])
				{
					path = pathBase.Substring(0, i);
					Console.WriteLine(path);
					for (i = 0; i < includes.Count; i++)
					{
						includes[i] = includes[i].Replace(path, "");
					}
					for (i = 0; i < includesWLabel.Count; i++)
					{
						var pair = includesWLabel[i];
						pair = new KeyValuePair<string, string>(pair.Key, pair.Value.Replace(path, ""));
						includesWLabel[i] = pair;
					}
					break;
				}
			}

			path = Path.Combine(pathBase, "_link.asm");
			try
			{
				ProjectExporter.SaveIncludes(path, includes, includesWLabel);
			}
			catch (Exception)
			{
				fails.Add(path);
			}

			// exportinf juckbox
			path = Path.Combine(pathBase, "_jukebox.asm");
			try
			{
				ProjectExporter.SaveJukebox(path, songReferences, _sfxs);
			}
			catch (Exception)
			{
				fails.Add(path);
			}

			// try to show errors
			Cursor.Current = Cursors.Default;
			if (fails.Count > 0)
			{
				_message.Clear();
				_message.AppendLine("Unable to export one or more files.");
				foreach (var failPath in fails)
				{
					_message.Append(" - ");
					_message.AppendLine(failPath);
				}

				ShowErrorMessage(_message.ToString());
			}
		}
		#endregion

		#region Export Properties
		private bool _isChanging = false;
		private void FillExportParams(DMFData data)
		{
			_isChanging = true;

			btnLockChannels.Checked = data.LockChannels;
			//_PCMRates[(int)data.PCMRate].Checked = true;

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

		//private void btnPCMRateCheckedChanged(object sender, EventArgs e)
		//{
		//	if (_isChanging) return;
		//	_isChanging = true;

		//	DMFData dmf = (DMFData)songsTreeView.SelectedNode.Tag;
		//	RadioButton radio = (RadioButton)sender;
		//	for (int i = 0; i < _PCMRates.Length; i++)
		//	{
		//		if (_PCMRates[i] == radio)
		//		{
		//			dmf.PCMRate = (ESF_PCMRate)i;
		//			break;
		//		}
		//	}

		//	_isChanging = false;
		//}
		#endregion

		#region SFXs
		private void RefreshSFXsView()
		{
			sfxsTreeView.SelectedNode = null;
			sfxsTreeView.Nodes.Clear();
			foreach (var data in _project.SFXs)
			{
				TreeNode container = new TreeNode(data.ExportName);
				container.Tag = data;
				container.Checked = data.Export;

				foreach(var sfx in data.FXs)
				{
					TreeNode node = new TreeNode(sfx.ExportName);
					node.Tag = sfx;
					node.Checked = sfx.Export;
					container.Nodes.Add(node);
				}

				sfxsTreeView.Nodes.Add(container);
			}

			RefreshSFXPriority(null);
		}

		private void sfxsTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (e.Label == null) return;

			var invalidChars = Path.GetInvalidFileNameChars();
			if (e.Label.IndexOfAny(invalidChars) >= 0)
			{
				e.CancelEdit = true;
				ShowErrorMessage("The name contains invalid chars.");
			}

			SFXData containerData = e.Node.Tag as SFXData;
			if (containerData == null)
			{
				DMFData sfx = (DMFData)e.Node.Tag;
				containerData = _project.FindContainer(sfx);

				if (containerData.CheckDuplicateExportName(e.Label))
				{
					e.CancelEdit = true;
					ShowErrorMessage(string.Format("The name '{0}' already exist.", e.Label));
				}
				else
					sfx.ExportName = e.Label;
			}
			else if (_project.CheckDuplicateSFXExportName(e.Label))
			{
				e.CancelEdit = true;
				ShowErrorMessage(string.Format("The name '{0}' already exist.", e.Label));
			}
			else
				containerData.ExportName = e.Label;
		}

		private void sfxsTreeView_KeyUp(object sender, KeyEventArgs e)
		{
			if (sfxsTreeView.SelectedNode == null) return;

			switch (e.KeyCode)
			{
				case Keys.Delete:
					var data = sfxsTreeView.SelectedNode.Tag as SFXData;
					if (data == null) return;

					_project.SFXs.Remove(data);

					RefreshSFXsView();
					unsupportedEffects.Visible = false;
					exportParams.Enabled = false;
					RefreshExportEchoButtons();
					break;

				case Keys.F2:
					sfxsTreeView.SelectedNode.BeginEdit();
					break;
			}
		}

		private void sfxsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			RefreshSFXPriority(e.Node);
		}

		private void RefreshSFXPriority(TreeNode node)
		{
			bool selected = node != null && node.Tag is DMFData;
			btnSFXPriority.Enabled = selected;
			if (selected)
			{
				var data = node.Tag as DMFData;
				btnSFXPriority.Tag = data;
				btnSFXPriority.Value = data.Priority;
			}
			else
				btnSFXPriority.Tag = null;
		}

		private void btnSFXPriority_ValueChanged(object sender, EventArgs e)
		{
			var data = btnSFXPriority.Tag as DMFData;
			if (data != null)
				data.Priority = (byte)btnSFXPriority.Value;
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
			else if (_project.CheckDuplicateSongExportName(e.Label))
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
			if (songsTreeView.SelectedNode == null) return;
			switch (e.KeyCode)
			{
				case Keys.Delete:
					DMFData data = (DMFData)songsTreeView.SelectedNode.Tag;
					_project.Songs.Remove(data);

					RefreshDetailsView();
					unsupportedEffects.Visible = false;
					exportParams.Enabled = false;
					RefreshExportEchoButtons();
					break;

				case Keys.F2:
					songsTreeView.SelectedNode.BeginEdit();
					break;
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
			settingsToolStripMenuItem.Enabled = true;

			if (_project.SaveSongs.Count > 0 || _project.SFXs.Count > 0)
			{
				string rootPath = _project.GetProjectFolderPath();
				var fails = new Dictionary<DMFImporter.Result, List<string>>();

				foreach (var song in _project.SaveSongs)
				{
					var data = LoadDMF(song, rootPath, fails);
					if (data != null)
					{
						SetSongSaveData(data, song);
						_project.Songs.Add(data);
					}
				}

				foreach (var sfx in _project.SFXs)
				{
					var data = LoadDMF(sfx, rootPath, fails);
					if (data != null)
					{
						sfx.Source = data;
						int i = 0;
						for (; i < data.PatternPages; i++)
						{
							var fx = new DMFData(data, i);
							sfx.FXs.Add(fx);

							if (i < sfx.SaveSFX.Count)
							{
								var savedData = sfx.SaveSFX[i];
								fx.Export = savedData.Export;
								fx.ExportName = savedData.ExportName;
								fx.Priority = savedData.Priority;
								SetSongSaveData(fx, savedData);
							}
						}
						FilterSFXsNames(sfx);
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
			RefreshSFXsView();
			unsupportedEffects.Visible = false;
			RefreshExportEchoButtons();
			RefreshExportChangeBitRate();
		}

		static private readonly List<string> _usedNames = new List<string>();
		private void FilterSFXsNames(SFXData fxs)
		{
			_usedNames.Clear();
			foreach(var sfx in fxs.FXs)
			{
				string name = sfx.ExportName;
				if (string.IsNullOrWhiteSpace(name))
					name = "Sound";

				name = GetNextDefaultName(_usedNames, name);
				sfx.ExportName = name;
				_usedNames.Add(name);
			}
		}

		private void SetSongSaveData(DMFData data, SongData savedData)
		{
			data.LockChannels = savedData.LockChannels;
			data.LoopWholeTrack = savedData.LoopWholeTrack;
			data.PCMRate = savedData.PCMRate;

			for (int i = 0; i < savedData.ExportChannels.Count && i < data.Channels.Count; i++)
			{
				data.Channels[i].Export = savedData.ExportChannels[i];
			}
		}

		private DMFData LoadDMF(AudioData audio, string rootPath, Dictionary<DMFImporter.Result, List<string>> fails)
		{
			List<string> errors;
			string path = audio.Path;
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
				return null;
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
					data.ExportName = audio.ExportName;
					data.Export = audio.Export;
					return data;
			}

			return null;
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
			bool nameUsed = false;
			foreach (var item in names)
			{
				nameUsed = item == prefix;
				if (nameUsed) break;
			}

			if (!nameUsed) return prefix;

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

		private void exportChangeBitRateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_project.ExportChangeBitRateAction = !_project.ExportChangeBitRateAction;
			RefreshExportChangeBitRate();
		}

		private void RefreshExportChangeBitRate()
		{
			exportChangeBitRateToolStripMenuItem.Checked = _project.ExportChangeBitRateAction;
		}

	}
}
