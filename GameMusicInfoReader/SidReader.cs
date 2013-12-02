using System.IO;
using System.Text;
using GameMusicInfoReader.Util;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A reader for Commodore 64 SID files (assumes V2+).
	/// </summary>
	public sealed class SidReader
	{
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">File path to the SID file.</param>
		public SidReader(string path)
		{
			using (BinaryReader sid = new BinaryReader(File.OpenRead(path)))
			{
				// Get some flag values beforehand
				// Video standard/chip model/BASIC flag
				sid.BaseStream.Seek(0x76, SeekOrigin.Begin);
				short flag = Endian.SwapInt16(sid.ReadInt16());
				VideoStandard = GetVideoStandard(flag);
				ChipModel = GetChipModel(flag);
				IsBasicFlagSet = BasicFlagIsSet(flag);
				sid.BaseStream.Seek(0x00, SeekOrigin.Begin);

				// Header
				byte[] header = new byte[4];
				sid.Read(header, 0, header.Length);
				HeaderID = Encoding.UTF8.GetString(header);

				// Version number (Shift because big-endian)
				Version = Endian.SwapInt16(sid.ReadInt16());

				// Offset to C64 binary data
				DataOffset = Endian.SwapInt16(sid.ReadInt16());

				// Addresses
				LoadAddress = GetLoadAddress(sid);
				InitAddress = GetInitAddress(sid);
				PlayAddress = GetPlayAddress(sid);

				// Songs
				Songs = Endian.SwapInt16(sid.ReadInt16());
				StartSong = Endian.SwapInt16(sid.ReadInt16());

				// Speed integer
				Speed = Endian.SwapInt32(sid.ReadInt32());

				// Song title
				char[] songTitle = new char[32];
				sid.Read(songTitle, 0, songTitle.Length);
				SongTitle = new string(songTitle);

				// Artist
				char[] artist = new char[32];
				sid.Read(artist, 0, artist.Length);
				Artist = new string(artist);

				// Copyright
				char[] copyright = new char[32];
				sid.Read(copyright, 0, copyright.Length);
				Copyright = new string(copyright);
			}
		}

		/// <summary>
		/// Gets the 4 byte header magic as a string.
		/// <br/>
		/// Can be either 'RSID' or 'PSID'
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// Version number stored in the SID file.
		/// </summary>
		public int Version
		{
			get;
			private set;
		}

		/// <summary>
		/// Offset (in decimal) from the start of the file to the C64 binary data.
		/// </summary>
		public int DataOffset
		{
			get;
			private set;
		}

		/// <summary>
		/// The C64 memory location (in decimal) to put the C64 data
		/// </summary>
		public short LoadAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// Starting address (in decimal) of machine code that initializes a song.
		/// <br/>
		/// If the Init Address is 0, then it is the same as the load address.
		/// </summary>
		public short InitAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// Starting address (in decimal) of the machine code subroutine that is called
		/// frequently to produce a continuous sound.
		/// <br/>
		/// A value of 0 means the initialization subroutine is expected to install an
		/// interrupt handler, which then calls the music player at some place.
		/// </summary>
		public int PlayAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// The number of songs (or sound-effects) that can be initialized
		/// by calling the init address. Minimum is 1, maximum is 256.
		/// </summary>
		public int Songs
		{
			get;
			private set;
		}

		/// <summary>
		/// The song number to be played by default. This value is optional. It often
		/// specifies the first song you would hear upon starting the program it has
		/// been taken from. It has a default of 1.
		/// </summary>
		public int StartSong
		{
			get;
			private set;
		}

		/// <summary>
		/// Each bit in 'speed' specifies the speed
		/// for the corresponding tune number, i.e. bit 0 specifies the speed for tune 1.
		/// If there are more than 32 tunes, the speed specified for tune 32 is also used
		/// for all higher numbered tunes.
		/// </summary>
		/// <remarks>
		/// <para>Surplus bits in 'speed' should be set to 0.</para>
		/// <para>For RSID files 'speed' must always be set to 0</para>
		/// </remarks>
		public int Speed
		{
			get;
			private set;
		}

		/// <summary>
		/// The song title of the SID file.
		/// </summary>
		public string SongTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// Artist of the track
		/// </summary>
		public string Artist
		{
			get;
			private set;
		}

		/// <summary>
		/// The track copyright. 
		/// </summary>
		public string Copyright
		{
			get;
			private set;
		}

		/// <summary>
		/// The video standard that the SID file uses.
		/// <br/>
		/// Ex. NTSC, PAL, or both.
		/// </summary>
		public string VideoStandard
		{
			get;
			private set;
		}

		/// <summary>
		/// The chip model that the SID file was made for.
		/// </summary>
		public string ChipModel
		{
			get;
			private set;
		}

		/// <summary>
		/// Whether or not the BASIC flag is set.
		/// </summary>
		public bool IsBasicFlagSet
		{
			get;
			set;
		}

		// Checks if the BASIC flag is set for RSID files.
		private static bool BasicFlagIsSet(short value)
		{
			return ((value & 2) != 0);
		}

		private short GetLoadAddress(BinaryReader sid)
		{
			short tempLoadAddr = Endian.SwapInt16(sid.ReadInt16());

			// Looks like the data is stored in the original C64 data format.
			// We have to read the first two bytes of data to get the address.
			if (tempLoadAddr == 0)
			{
				long tempPos = sid.BaseStream.Position;

				// Seek to the C64 data 
				sid.BaseStream.Seek(DataOffset, SeekOrigin.Begin);
				short res = sid.ReadInt16();
				sid.BaseStream.Seek(tempPos, SeekOrigin.Begin);

				return res;
			}

			// Not stored in original C64 format
			return tempLoadAddr;
		}

		private short GetInitAddress(BinaryReader sid)
		{
			short temp = Endian.SwapInt16(sid.ReadInt16());

			// Init address must be zero for RSID files with the BASIC flag set
			if (HeaderID.Contains("RSID") && IsBasicFlagSet)
				return 0;

			// If temp is zero, then InitAddress = LoadAddress
			if (temp == 0)
				return LoadAddress;
			else
				return temp;
		}

		private static short GetPlayAddress(BinaryReader sid)
		{
			return Endian.SwapInt16(sid.ReadInt16());
		}

		private static string GetVideoStandard(short value)
		{
			// If bit 2 is set, but bit 3 isn't
			if ((value & 4) != 0 && (value & 8) == 0)
			{
				return "PAL";
			}
			// if bit 2 isn't set, but bit 3 is
			else if ((value & 4) == 0 && (value & 8) != 0)
			{
				return "NTSC";
			}
			// if both bit 2 and 3 aren't set
			else if ((value & 4) != 0 && (value & 8) != 0)
			{
				return "PAL and NTSC";
			}
			else
			{
				return "Unknown";
			}
		}

		private static string GetChipModel(short value)
		{
			// If bit 4 is set, but bit 5 isn't
			if ((value & 16) != 0 && (value & 32) == 0)
			{
				return "MOS6581";
			}
			// If bit 4 isn't set, but bit 5 is
			else if ((value & 16) == 0 && (value & 32) != 0)
			{
				return "MOS8580";
			}
			// If both bit 4 and 5 are set
			else if ((value & 16) != 0 && (value & 32) != 0)
			{
				return "MOS6581 and MOS8580";
			}
			else
			{
				return "Unknown";
			}
		}
	}
}
