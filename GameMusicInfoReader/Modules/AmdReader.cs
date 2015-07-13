using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for getting info from AMusic AMD files.
	/// </summary>
	public sealed class AmdReader
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the AMD module.</param>
		public AmdReader(string path)
		{
			using (var amd = new BinaryReader(File.OpenRead(path)))
			{
				SongName = Encoding.UTF8.GetString(amd.ReadBytes(24));
				Artist   = Encoding.UTF8.GetString(amd.ReadBytes(24));

				// Song length & total patterns
				amd.BaseStream.Seek(0x3A4, SeekOrigin.Begin);
				SongLength = amd.ReadByte();
				TotalPatterns = amd.ReadByte();

				// Version
				amd.BaseStream.Seek(0x42F, SeekOrigin.Begin);
				Version = amd.ReadByte();
			}
		}

		#endregion

		#region Properties

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

		#endregion
	}
}
