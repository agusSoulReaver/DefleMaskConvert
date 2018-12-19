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

		[XmlElement("Song")]
		public List<SongData> SaveSongs = new List<SongData>();

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
		}

		public string GetProjectFolderPath()
		{
			string fileName = Path.GetFileName(FilePath);
			return FilePath.Substring(0, FilePath.IndexOf(fileName));
		}

		public bool CheckDuplicateExportName(string name)
		{
			foreach(var song in Songs)
			{
				if (name == song.ExportName) return true;
			}

			return false;
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

	public class SongData
	{
		[XmlAttribute]
		public string Path;

		[XmlAttribute]
		public string ExportName { get; set; }

		[XmlAttribute]
		public bool Export;

		[XmlAttribute]
		public bool LockChannels;

		[XmlAttribute]
		public bool LoopWholeTrack;

		[XmlAttribute]
		public ESF_PCMRate PCMRate;

		[XmlElement("Channel")]
		public List<bool> ExportChannels = new List<bool>();
	}
}
