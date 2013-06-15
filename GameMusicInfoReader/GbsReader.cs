using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A reader for getting info out of Gameboy GBS files.
	/// </summary>
	public sealed class GbsReader : IDisposable
	{
		// Filestream representing a GBS file
		private readonly FileStream gbs;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to a GBS file</param>
		public GbsReader(string path)
		{
			gbs = File.OpenRead(path);
		}

		/// <summary>
		/// The header magic of a GBS file.
		/// </summary>
		public string HeaderID
		{
			get
			{
				byte[] magic = new byte[3];

				// Start at beginning of file
				gbs.Seek(0, SeekOrigin.Begin);
				// Read first 3 bytes
				gbs.Read(magic, 0, 3);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(magic);
			}
		}

		/// <summary>
		/// The version number of the GBS standard being used in the file
		/// </summary>
		public int Version
		{
			get
			{
				// Seek 3 bytes.
				gbs.Seek(0x3, SeekOrigin.Begin);
				// Read 1 byte to get the version number
				return gbs.ReadByte();
			}
		}

		/// <summary>
		/// The total number of songs in the GBS file
		/// </summary>
		public int TotalSongs
		{
			get
			{
				// Seek 4 bytes.
				gbs.Seek(4, SeekOrigin.Begin);
				// Read 1 byte
				return gbs.ReadByte();
			}
		}

		/// <summary>
		/// The starting index for the songs
		/// </summary>
		public int StartingSong
		{
			get
			{
				// Seek 5 bytes
				gbs.Seek(5, SeekOrigin.Begin);
				// Read 1 byte
				return gbs.ReadByte();
			}
		}

		/// <summary>
		/// The title of the song.
		/// </summary>
		public string SongTitle
		{
			get
			{
				byte[] songTitle = new byte[32];

				// Seek 16 bytes in (offset 0x10)
				gbs.Seek(0x10, SeekOrigin.Begin);
				// Read 32 bytes
				gbs.Read(songTitle, 0, 32);

				// Convert bytes to string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songTitle);
			}
		}

		/// <summary>
		/// The artist of the current song
		/// </summary>
		public string Artist
		{
			get
			{
				byte[] artistTitle = new byte[32];

				// Seek 48 bytes in (offset 0x30)
				gbs.Seek(0x30, SeekOrigin.Begin);
				// Read 32 bytes
				gbs.Read(artistTitle, 0, 32);

				// Convert bytes to string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(artistTitle);
			}
		}

		/// <summary>
		/// The Copyright string for the loaded GBS file
		/// </summary>
		public string Copyright
		{
			get
			{
				byte[] copyright = new byte[32];

				// Seek 80 bytes in (offset 0x50)
				gbs.Seek(0x50, SeekOrigin.Begin);
				// Read 32 bytes
				gbs.Read(copyright, 0, 32);

				// Convert bytes to string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(copyright);
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (gbs != null)
				{
					gbs.Dispose();
				}
			}
		}
	}
}
