using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader
{
	/// <summary>
	/// Reader for Nintendo NSF Files
	/// </summary>
	public sealed class NsfReader : IDisposable
	{
		// Filestream representing an NSF file
		private readonly FileStream nsf;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to the NSF file.</param>
		public NsfReader(string path)
		{
			nsf = File.OpenRead(path);
		}

		/// <summary>
		/// Reads the header identifier
		/// </summary>
		public string HeaderID
		{
			get
			{
				byte[] magic = new byte[5];

				// Ensure start of file.
				nsf.Seek(0, SeekOrigin.Begin);

				// Read 5 bytes
				nsf.Read(magic, 0, 5);

				// Convert bytes to string
				return Encoding.UTF8.GetString(magic);

			}
		}

		/// <summary>
		/// The version number byte of the NSF file.
		/// </summary>
		public int Version
		{
			get
			{
				// Seek 4 bytes in
				nsf.Seek(4, SeekOrigin.Begin);
				return nsf.ReadByte();
			}
		}

		/// <summary>
		/// The total number of songs
		/// </summary>
		public int TotalSongs
		{
			get
			{	
				// Seek 6 bytes in
				nsf.Seek(6, SeekOrigin.Begin);
				return nsf.ReadByte();
			}
		}

		/// <summary>
		/// The very first song of the entire set contained in the NSF file
		/// </summary>
		public int StartingSong
		{
			get
			{
				// Seek 7 bytes in
				nsf.Seek(7, SeekOrigin.Begin);
				return nsf.ReadByte();
			}
		}

		/// <summary>
		/// The song name of the NSF file
		/// </summary>
		public string SongName
		{
			get
			{
				byte[] songName = new byte[32];

				// Seek 14 bytes in
				nsf.Seek(14, SeekOrigin.Begin);

				// Read 32 bytes
				nsf.Read(songName, 0, 32);

				// Convert it to a string 
				return Encoding.UTF8.GetString(songName);
			}
		}

		/// <summary>
		/// The artists of the game music in the NSF file.
		/// <br/>
		/// Can return &lt;?&gt; if no artist is specified.
		/// </summary>
		public string Artist
		{
			get
			{
				byte[] artist = new byte[32];

				// Seek 46 bytes in
				nsf.Seek(0x2E, SeekOrigin.Begin);

				// Read 32 bytes
				nsf.Read(artist, 0, 32);

				// Convert bytes to string
				return Encoding.UTF8.GetString(artist);
			}
		}

		/// <summary>
		/// The copyright string within the NSF file.
		/// </summary>
		public string Copyright
		{
			get
			{
				byte[] copyright = new byte[32];

				// Seek 78 bytes in
				nsf.Seek(0x4E, SeekOrigin.Begin);

				// Read 32 bytes
				nsf.Read(copyright, 0, 32);

				// Convert it to a string
				return Encoding.UTF8.GetString(copyright);
			}
		}

		/// <summary>
		/// The playback speed in ticks for the NSF file.
		/// </summary>
		public int SpeedTicks
		{
			get
			{
				byte[] playbackBytes = new byte[2];

				if (IsNTSC)
				{
					nsf.Seek(0x6E, SeekOrigin.Begin);
					nsf.Read(playbackBytes, 0, 2);
				}
				else // PAL
				{
					nsf.Seek(0x78, SeekOrigin.Begin);
					nsf.Read(playbackBytes, 0, 2);
				}

				// Return ticks (for NTSC)
				return playbackBytes[0] | (playbackBytes[1] << 8);
			}
		}

		public int SpeedTicksInHertz
		{
			get
			{
				// Seek 110 bytes in
				nsf.Seek(0x6E, SeekOrigin.Begin);

				byte[] playbackBytes = new byte[2];

				// Read 2 bytes
				nsf.Read(playbackBytes, 0, 2);

				int v = playbackBytes[0] | (playbackBytes[1] << 8);

				// If its a PAL NSF file
				if (!IsNTSC)
				{
					// Clear playbackBytes contents
					Array.Clear(playbackBytes, 0, playbackBytes.Length);

					// Seek 120 (0x78) bytes in
					nsf.Seek(0x78, SeekOrigin.Begin);

					// Read 2 bytes
					nsf.Read(playbackBytes, 0, 2);

					v = playbackBytes[0] | (playbackBytes[1] << 8);

					// Return playback rate in Hertz (for PAL)
					return (1000000 / v);
				}

				// Return playback rate in Hertz (for NTSC)
				return (1000000 / v);
			}
		}

		/// <summary>
		/// Checks if the NSF file is NTSC clock based
		/// </summary>
		public bool IsNTSC
		{
			get
			{
				// Seek 122 (0x7A) bytes in
				nsf.Seek(0x7A, SeekOrigin.Begin);

				// Read one byte
				byte playbackByte = (byte)nsf.ReadByte();

				// If the zeroth bit is not set, then it's an NTSC file.
				if ((playbackByte & 1) == 0)
				{
					return true;
				}

				// If it's set, it's a PAL file
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file is both PAL and NTSC supported
		/// </summary>
		public bool IsDualSupportive
		{
			get
			{
				// Seek 122 (0x7A) bytes in
				nsf.Seek(0x7A, SeekOrigin.Begin);

				byte playbackByte = (byte)nsf.ReadByte();

				// If the first bit is set (not zero), then it's dual supportive
				if ((playbackByte & 2) != 0)
				{
					return true;
				}

				// If it's not set, then it's not dual supportive
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file requires the use of the VRC6 sound chip
		/// </summary>
		public bool IsUsingVrc6SoundChip
		{
			get
			{
				// Seek 123 (0x7B) bytes in
				nsf.Seek(0x7B, SeekOrigin.Begin);

				// Read 1 byte
				byte chipByte = (byte)nsf.ReadByte();

				// If bit 0 is set, then it's using the VRC6 chip
				if ((chipByte & 1) != 0)
					return true;

				// Not using VRC6
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file requires the use of the VRC7 sound chip
		/// </summary>
		public bool IsUsingVrc7SoundChip
		{
			get
			{
				// Seek 123 (0x7B) bytes in
				nsf.Seek(0x7B, SeekOrigin.Begin);

				// Read 1 byte
				byte chipByte = (byte)nsf.ReadByte();

				// If bit 1 is set, it's using the VRC7 chip
				if ((chipByte & 2) != 0)
					return true;

				// Not using the sound chip
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file required the use of FDS (Famicom Disk System) audio
		/// </summary>
		public bool IsUsingFdsAudio
		{
			get
			{
				// Seek 123 (0x7B) bytes in
				nsf.Seek(0x7B, SeekOrigin.Begin);

				// Read 1 byte
				byte chipByte = (byte)nsf.ReadByte();

				// If bit 2 is set, it's using FDS audio
				if ((chipByte & 4) != 0)
					return true;

				// Not using FDS audio
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file requires the use of the MMC5 audio mapper
		/// </summary>
		public bool IsUsingMMC5Audio
		{
			get
			{
				// Seek 123 (0x7B) bytes in
				nsf.Seek(0x7B, SeekOrigin.Begin);

				byte chipByte = (byte)nsf.ReadByte();

				// If bit 3 is set, it's using the MMC5 audio mapper
				if ((chipByte & 8) != 0)
					return true;

				// Not using the MMC5 audio mapper
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file requires the use of Namco 163 audio
		/// </summary>
		public bool IsUsingNamco
		{
			get
			{
				// Seek 123 (0x7B) bytes in
				nsf.Seek(0x7B, SeekOrigin.Begin);

				// Read 1 byte
				byte chipByte = (byte)nsf.ReadByte();

				// If bit 4 is set, it's using Namco 163 audio
				if ((chipByte & 16) != 0)
					return true;

				// Not using Namco 163 audio
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file requires the use of Sunsoft 5B audio
		/// </summary>
		public bool IsUsingSunsoft
		{
			get
			{
				// Seek 123 (0x7B) bytes in
				nsf.Seek(0x7B, SeekOrigin.Begin);

				// Read 1 byte
				byte chipByte = (byte)nsf.ReadByte();

				// If bit 5 is set, it's using Sunsoft 5B audio
				if ((chipByte & 32) != 0)
					return true;

				// Not using Sunsoft 5B audio
				return false;
			}
		}

		/// <summary>
		/// Checks if the NSF file uses any type of extension sound chip
		/// </summary>
		public bool IsUsingAnyExtensionChips
		{
			get
			{
				return (IsUsingVrc6SoundChip || IsUsingVrc7SoundChip || IsUsingFdsAudio || IsUsingMMC5Audio || IsUsingNamco || IsUsingSunsoft);
			}
		}


		#region IDisposable methods

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (nsf != null)
					nsf.Dispose();
			}
		}

		#endregion
	}
}
