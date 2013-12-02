using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from Protracker MOD modules.
	/// </summary>
	public sealed class ModReader
	{
		// TODO: More info?

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the MOD module.</param>
		public ModReader(string path)
		{
			using (FileStream mod = File.OpenRead(path))
			{
				// Song title
				byte[] songTitle = new byte[20];
				mod.Read(songTitle, 0, songTitle.Length);
				SongTitle = Encoding.UTF8.GetString(songTitle);

				// Song length
				mod.Seek(0x3B6, SeekOrigin.Begin);
				SongLength = mod.ReadByte();

				// Module ID
				byte[] modId = new byte[4];
				mod.Seek(0x438, SeekOrigin.Begin);
				mod.Read(modId, 0, modId.Length);
				ModuleID = Encoding.UTF8.GetString(modId);
			}
		}

		/// <summary>
		/// The song title of the MOD file
		/// </summary>
		public string SongTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// The song length in patterns for the MOD file
		/// </summary>
		public int SongLength
		{
			get;
			private set;
		}

		/// <summary>
		/// The 4 character module ID for the MOD file
		/// </summary>
		/// <remarks>
		/// If module has ID's M.K., 8CHN, 4CHN, 6CHN, FLT4 or FLT8, then
		/// the module has 31 instruments
		/// </remarks>
		public string ModuleID
		{
			get;
			private set;
		}
	}
}
