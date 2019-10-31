using DefleMaskConvert.DAO.DefleMask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DefleMaskConvert.DAO
{
	[XmlRootAttribute("Project")]
	public class ProjectData
	{
		[XmlIgnore]
		public string FilePath = null;

		[XmlIgnore]
		public string Name
		{
			get
			{
				if (FilePath != null)
					return System.IO.Path.GetFileNameWithoutExtension(FilePath);

				return "*";
			}
		}

		[XmlArray("Music")]
		public List<SongData> SaveSongs = new List<SongData>();
		[XmlArray("SFXs")]
		public List<SFXData> SFXs = new List<SFXData>();

		[XmlIgnore]
		public List<DMFData> Songs = new List<DMFData>();

		public void PrepareToSave()
		{
			SaveSongs.Clear();
			string rootPath = GetProjectFolderPath();

			foreach(var song in Songs)
			{
				var save = new SongData();
				SaveSongs.Add(save);

				string path = song.FilePath;
				if(path.IndexOf(rootPath) == 0)
					path = path.Substring(rootPath.Length);

				save.Path = path;
				save.Export = song.Export;
				save.ExportName = song.ExportName;
				save.LockChannels = song.LockChannels;
				save.LoopWholeTrack = song.LoopWholeTrack;
				save.PCMRate = song.PCMRate;

				foreach(var channel in song.Channels)
				{
					save.ExportChannels.Add(channel.Export);
				}
			}

			foreach(var sfx in SFXs)
			{
				if (sfx.Path.IndexOf(rootPath) == 0)
					sfx.Path = sfx.Path.Substring(rootPath.Length);

				sfx.PrepareToSave(rootPath);
			}
		}

		public string GetProjectFolderPath()
		{
			string fileName = Path.GetFileName(FilePath);
			return FilePath.Substring(0, FilePath.IndexOf(fileName));
		}

		public bool CheckDuplicateSongExportName(string name)
		{
			foreach(var song in Songs)
			{
				if (name == song.ExportName) return true;
			}

			return false;
		}

		public bool CheckDuplicateSFXExportName(string name)
		{
			foreach (var sfx in SFXs)
			{
				if (name == sfx.ExportName) return true;
			}

			return false;
		}

		public SFXData FindContainer(DMFData data)
		{
			foreach (var sfx in SFXs)
			{
				if (sfx.FXs.Contains(data))
					return sfx;
			}

			throw new ArgumentException();
		}

		public bool CheckDuplicateSongFile(string path)
		{
			foreach (var song in Songs)
			{
				if (path == song.FilePath) return true;
			}

			return false;
		}
	}

	public class AudioData
	{
		[XmlAttribute]
		public string Path;

		[XmlAttribute]
		public string ExportName { get; set; }

		[XmlAttribute]
		public bool Export;
	}

	[XmlType(TypeName = "Song")]
	public class SongData : AudioData
	{
		[XmlAttribute]
		public bool LockChannels;

		[XmlAttribute]
		public bool LoopWholeTrack;

		[XmlAttribute]
		public ESF_PCMRate PCMRate;

		[XmlAttribute]
		public byte Priority = 1;

		[XmlIgnore]
		public List<bool> ExportChannels = new List<bool>();

		[XmlAttribute("Channels")]
		public string ExportChannelsSerialized
		{
			get
			{
				StringBuilder text = new StringBuilder();
				for (int i = 0; i < ExportChannels.Count; i++)
				{
					if (i > 0) text.Append("|");
					if (ExportChannels[i]) text.Append(".");
				}

				return text.ToString();
			}
			set
			{
				var split = value.Split('|');
				for (int i = 0; i < split.Length; i++)
				{
					ExportChannels.Add(!string.IsNullOrWhiteSpace(split[i]));
				}
			}
		}
	}

	[XmlType(TypeName = "SFXFile")]
	public class SFXData : AudioData
	{
		[XmlIgnore]
		public DMFData Source;
		[XmlIgnore]
		public List<DMFData> FXs = new List<DMFData>();

		[XmlElement("SFX")]
		public List<SongData> SaveSFX = new List<SongData>();

		public bool CheckDuplicateExportName(string name)
		{
			foreach (var sfx in FXs)
			{
				if (name == sfx.ExportName) return true;
			}

			return false;
		}

		public void PrepareToSave(string rootPath)
		{
			SaveSFX.Clear();

			foreach (var song in FXs)
			{
				var save = new SongData();
				SaveSFX.Add(save);

				string path = song.FilePath;
				if (path.IndexOf(rootPath) == 0)
					path = path.Substring(rootPath.Length);

				save.Path = path;
				save.Export = song.Export;
				save.ExportName = song.ExportName;
				save.LockChannels = song.LockChannels;
				save.LoopWholeTrack = song.LoopWholeTrack;
				save.PCMRate = song.PCMRate;
				save.Priority = song.Priority;

				foreach (var channel in song.Channels)
				{
					save.ExportChannels.Add(channel.Export);
				}
			}
		}
	}

}
