using System.IO;
using System.Text;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A reader for getting info out of Gameboy GBS files.
	/// </summary>
	public sealed class GbsReader
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to a GBS file</param>
		public GbsReader(string path)
		{
			BinaryReader gbs = new BinaryReader(File.OpenRead(path));

			// Header magic
			byte[] magic = new byte[3];
			gbs.Read(magic, 0, 3);
			HeaderID = Encoding.UTF8.GetString(magic);

			Version = gbs.ReadByte();
			TotalSongs = gbs.ReadByte();
			StartingSong = gbs.ReadByte();
			LoadAddress = gbs.ReadInt16();
			InitAddress = gbs.ReadInt16();
			PlayAddress = gbs.ReadInt16();
			StackPointer = gbs.ReadInt16();
			TimerModulo = gbs.ReadByte();
			TimerControl = gbs.ReadByte();

			byte[] infoBytes = new byte[32];
			gbs.Read(infoBytes, 0, 32);
			SongTitle = Encoding.UTF8.GetString(infoBytes);

			gbs.Read(infoBytes, 0, 32);
			Artist = Encoding.UTF8.GetString(infoBytes);

			gbs.Read(infoBytes, 0, 32);
			Copyright = Encoding.UTF8.GetString(infoBytes);

			// Done
			gbs.Close();
		}

		/// <summary>
		/// The header magic of a GBS file.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// The version number of the GBS standard being used in the file
		/// </summary>
		public int Version
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of songs in the GBS file
		/// </summary>
		public int TotalSongs
		{
			get;
			private set;
		}

		/// <summary>
		/// The starting index for the songs
		/// </summary>
		public int StartingSong
		{
			get;
			private set;
		}

		/// <summary>
		/// Load Address (can be 0x400 to 0x7FFF)
		/// </summary>
		public int LoadAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// Init Address (can be from 0x400 to 0x7FFF)
		/// </summary>
		public int InitAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// Play Address (can be from 0x400 to 0x7FFF)
		/// </summary>
		public int PlayAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// Stack pointer
		/// </summary>
		public int StackPointer
		{
			get;
			private set;
		}

		/// <summary>
		/// Timer modulo
		/// </summary>
		public int TimerModulo
		{
			get;
			private set;
		}

		/// <summary>
		/// Timer control
		/// </summary>
		public int TimerControl
		{
			get;
			private set;
		}

		/// <summary>
		/// The title of the song.
		/// </summary>
		public string SongTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// The artist of the current song
		/// </summary>
		public string Artist
		{
			get;
			private set;
		}

		/// <summary>
		/// The Copyright string for the loaded GBS file
		/// </summary>
		public string Copyright
		{
			get;
			private set;
		}
	}
}
