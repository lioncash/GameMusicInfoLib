using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from ScreamTracker 2.x modules.
	/// </summary>
	public sealed class StmReader
	{
		// TODO: Get instrument information

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an STM module</param>
		public StmReader(string path)
		{
			using (var stm = new BinaryReader(File.OpenRead(path)))
			{
				SongName    = Encoding.UTF8.GetString(stm.ReadBytes(20));
				TrackerName = Encoding.UTF8.GetString(stm.ReadBytes(8));

				// Filetype
				FileType = stm.ReadByte();

				// Tempo
				stm.BaseStream.Seek(0x20, SeekOrigin.Begin);
				Tempo = stm.ReadByte();

				// Total patterns
				TotalPatterns = stm.ReadByte();

				// Global volume
				GlobalVolume = stm.ReadByte();
			}
		}

		/// <summary>
		/// The song name of the track in the STM module
		/// </summary>
		public string SongName
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the tracker the module was created in
		/// </summary>
		public string TrackerName
		{
			get;
			private set;
		}

		/// <summary>
		/// The file type of the module file.
		/// <para/>
		/// If it returns 1, it's a song.
		/// <para/>
		/// If it returns 2, it's a module.
		/// </summary>
		public int FileType
		{
			get;
			private set;
		}

		/// <summary>
		/// The tempo of the module file
		/// </summary>
		public int Tempo
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of patterns saved in the STM module.
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The global volume of the STM module
		/// </summary>
		public int GlobalVolume
		{
			get;
			private set;
		}
	}
}
