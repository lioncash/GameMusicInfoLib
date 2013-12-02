using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for getting info from AMusic AMD files.
	/// </summary>
	public sealed class AmdReader
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the AMD module.</param>
		public AmdReader(string path)
		{
			using (FileStream amd = File.OpenRead(path))
			{
				// Song name
				byte[] info = new byte[24];
				amd.Read(info, 0, 24);
				SongName = Encoding.UTF8.GetString(info);

				// Artist name
				amd.Read(info, 0, 24);
				Artist = Encoding.UTF8.GetString(info);

				// Song length & total patterns
				amd.Seek(0x3A4, SeekOrigin.Begin);
				SongLength = amd.ReadByte();
				TotalPatterns = amd.ReadByte();

				// Version
				amd.Seek(0x42F, SeekOrigin.Begin);
				Version = amd.ReadByte();
			}
		}

		/// <summary>
		/// The name of the song within the module.
		/// </summary>
		public string SongName
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the artist who created the module file
		/// </summary>
		public string Artist
		{
			get;
			private set;
		}

		/// <summary>
		/// Length of the track
		/// </summary>
		public int SongLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Total patterns - 1
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// If 0x10 (16) is returned, then the module is normal.
		/// <para/>
		/// If 0x11 (17) is returned, then the module is packed.
		/// </summary>
		public int Version
		{
			get;
			private set;
		}
	}
}
