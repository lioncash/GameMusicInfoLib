using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from MOD modules.
	/// </summary>
	public sealed class ModReader
	{
		// TODO: More info?

		// Filestream representing a MOD module.
		private readonly FileStream mod;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the MOD module.</param>
		public ModReader(string path)
		{
			mod = File.OpenRead(path);
		}

		/// <summary>
		/// The song title of the MOD file
		/// </summary>
		public string SongTitle
		{
			get
			{
				byte[] songTitle = new byte[20];

				// Ensure we're at the beginning of the module.
				mod.Seek(0, SeekOrigin.Begin);
				// Read first 20 bytes
				mod.Read(songTitle, 0, 20);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songTitle);
			}
		}

		/// <summary>
		/// The song length in patterns for the MOD file
		/// </summary>
		public int SongLength
		{
			get
			{
				// Seek 950 bytes (0x3B6) in
				mod.Seek(0x3B6, SeekOrigin.Begin);

				// Read one byte
				return mod.ReadByte();
			}
		}

		/// <summary>
		/// The 4 character module ID for the MOD file
		/// 
		/// If module has ID's M.K., 8CHN, 4CHN, 6CHN, FLT4 or FLT8, then
		/// the module has 31 instruments
		/// </summary>
		public string ModuleID
		{
			get
			{
				byte[] moduleid = new byte[4];

				// Seek 1080 bytes (0x438) in
				mod.Seek(0x438, SeekOrigin.Begin);

				// Read 4 bytes
				mod.Read(moduleid, 0, 4);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(moduleid);
			}
		}
	}
}
