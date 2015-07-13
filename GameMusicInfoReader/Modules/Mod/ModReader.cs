using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules.Mod
{
	/// <summary>
	/// Reader for getting info from Protracker MOD modules.
	/// </summary>
	public sealed class ModReader
	{
		// TODO: More info?

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the MOD module.</param>
		public ModReader(string path)
		{
			using (var mod = new BinaryReader(File.OpenRead(path)))
			{
				// Song title
				byte[] songTitle = mod.ReadBytes(20);
				SongTitle = Encoding.UTF8.GetString(songTitle);

				// Song length
				mod.BaseStream.Seek(0x3B6, SeekOrigin.Begin);
				SongLength = mod.ReadByte();

				// Module ID
				mod.BaseStream.Seek(0x438, SeekOrigin.Begin);
				ModuleID = Encoding.UTF8.GetString(mod.ReadBytes(4));
			}
		}

		#endregion

		#region Properties

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

		#endregion
	}
}
