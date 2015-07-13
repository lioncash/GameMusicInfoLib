using System.IO;

namespace GameMusicInfoReader
{
	/// <summary>
	/// Reader for Nintendo NSF Files
	/// </summary>
	public sealed class NsfReader
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to the NSF file.</param>
		public NsfReader(string path)
		{
			using (var nsf = new BinaryReader(File.OpenRead(path)))
			{
				// Header + Version
				HeaderID = new string(nsf.ReadChars(5));
				Version = nsf.ReadByte();

				// Total songs & starting song
				TotalSongs = nsf.ReadByte();
				StartingSong = nsf.ReadByte();

				// Song name/Author/Copyright
				SongName  = new string(nsf.ReadChars(32));
				Artist    = new string(nsf.ReadChars(32));
				Copyright = new string(nsf.ReadChars(32));

				// Check for NTSC.
				nsf.BaseStream.Position = 0x7A;
				byte playbackByte = nsf.ReadByte();
				IsNTSC            = ((playbackByte & 1) == 0);
				IsDualSupportive  = ((playbackByte & 2) != 0);

				// Speed ticks
				if (IsNTSC)
					nsf.BaseStream.Position = 0x6E;
				else
					nsf.BaseStream.Position = 0x78;
				SpeedTicks = (nsf.ReadByte() | (nsf.ReadByte() << 8));

				// Chip support
				nsf.BaseStream.Position = 0x7B;
				byte chipByte = nsf.ReadByte();
				IsUsingVrc6SoundChip = ((chipByte & 1)  != 0);
				IsUsingVrc7SoundChip = ((chipByte & 2)  != 0);
				IsUsingFdsAudio      = ((chipByte & 4)  != 0);
				IsUsingMMC5Audio     = ((chipByte & 8)  != 0);
				IsUsingNamco         = ((chipByte & 16) != 0);
				IsUsingSunsoft       = ((chipByte & 32) != 0);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Reads the header identifier
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// The version number byte of the NSF file.
		/// </summary>
		public int Version
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of songs
		/// </summary>
		public int TotalSongs
		{
			get;
			private set;
		}

		/// <summary>
		/// The very first song of the entire set contained in the NSF file
		/// </summary>
		public int StartingSong
		{
			get;
			private set;
		}

		/// <summary>
		/// The song name of the NSF file
		/// </summary>
		public string SongName
		{
			get;
			private set;
		}

		/// <summary>
		/// The artists of the game music in the NSF file.
		/// <br/>
		/// Can return &lt;?&gt; if no artist is specified.
		/// </summary>
		public string Artist
		{
			get;
			private set;
		}

		/// <summary>
		/// The copyright string within the NSF file.
		/// </summary>
		public string Copyright
		{
			get;
			private set;
		}

		/// <summary>
		/// The playback speed in ticks for the NSF file.
		/// </summary>
		public int SpeedTicks
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file is NTSC clock based
		/// </summary>
		public bool IsNTSC
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file is both PAL and NTSC supported
		/// </summary>
		public bool IsDualSupportive
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file requires the use of the VRC6 sound chip
		/// </summary>
		public bool IsUsingVrc6SoundChip
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file requires the use of the VRC7 sound chip
		/// </summary>
		public bool IsUsingVrc7SoundChip
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file required the use of FDS (Famicom Disk System) audio
		/// </summary>
		public bool IsUsingFdsAudio
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file requires the use of the MMC5 audio mapper
		/// </summary>
		public bool IsUsingMMC5Audio
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file requires the use of Namco 163 audio
		/// </summary>
		public bool IsUsingNamco
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the NSF file requires the use of Sunsoft 5B audio
		/// </summary>
		public bool IsUsingSunsoft
		{
			get;
			private set;
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

		#endregion
	}
}
