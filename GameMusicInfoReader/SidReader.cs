using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A reader for Commodore 64 SID files.
	/// </summary>
	public sealed class SidReader : IDisposable
	{
		// Filestream representing the SID file.
		private readonly FileStream sid;
		// Reader that helps with reading bytes.
		private readonly BinaryReader br;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">File path to the SID file.</param>
		public SidReader(string path)
		{
			sid = File.OpenRead(path);
			br = new BinaryReader(sid);
		}

		/// <summary>
		/// Gets the 4 byte header magic as a string.
		/// <br/>
		/// Can be either 'RSID' or 'PSID'
		/// </summary>
		public string HeaderID
		{
			get
			{
				byte[] magic = new byte[4];

				// Make sure we seek to the beginning of the file
				sid.Seek(0, SeekOrigin.Begin);
				// Read 4 bytes
				sid.Read(magic, 0, 4);

				// Convert bytes to string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(magic);
			}
		}

		/// <summary>
		/// Version number stored in the SID file.
		/// </summary>
		public int Version
		{
			get
			{
				// Seek 5 bytes in.
				br.BaseStream.Seek(5, SeekOrigin.Begin);
				return br.ReadByte();
			}
		}

		/// <summary>
		/// Offset (in decimal) from the start of the file to the C64 binary data.
		/// </summary>
		public int DataOffset
		{
			get
			{
				if (Version == 1)
					return 0x0076;
				else // Must be version 2 then.
					return 0x007C;
			}
		}

		/// <summary>
		/// The C64 memory location (in decimal) to put the C64 data
		/// </summary>
		public int LoadAddress
		{
			get
			{
				// Seek 8 bytes in
				br.BaseStream.Seek(8, SeekOrigin.Begin);

				short tempLoadAddr = br.ReadInt16();

				// Looks like the data is stored in the original C64 data format.
				// We have to read the first two bytes of data to get the address.
				if (tempLoadAddr == 0)
				{
					// Seek to the C64 data 
					br.BaseStream.Seek(DataOffset, SeekOrigin.Begin);
					return br.ReadInt16();
				}
				
				// Not stored in original C64 format
				return tempLoadAddr;
			}
		}

		/// <summary>
		/// Starting address (in decimal) of machine code that initializes a song.
		/// <br/>
		/// If the Init Address is 0, then it is the same as the load address.
		/// </summary>
		public int InitAddress
		{
			get
			{
				// Seek 10 bytes in
				br.BaseStream.Seek(0xA, SeekOrigin.Begin);
				short temp = br.ReadInt16();
				
				// Init address must be zero for RSID files with the BASIC flag set
				if (HeaderID.Contains("RSID") && BasicFlagIsSet())
					return 0;

				// If temp is zero, then InitAddress = LoadAddress
				if (temp == 0)
					return LoadAddress;
				else
					return temp;
			}
		}

		/// <summary>
		/// The number of songs (or sound-effects) that can be initialized
		/// by calling the init address. Minimum is 1, maximum is 256.
		/// </summary>
		public int Songs
		{
			get
			{
				// Seek 14 bytes
				br.BaseStream.Seek(0xE, SeekOrigin.Begin);
				
				// TODO: Do we shift here? Seems correct, but I'm not sure.
				// Get only the higher bits.
				return br.ReadInt16() >> 8;
			}
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
			get
			{
				// Seek 12 bytes
				br.BaseStream.Seek(0xC, SeekOrigin.Begin);
				return br.ReadInt16();
			}
		}

		/// <summary>
		/// The song title of the SID file.
		/// </summary>
		public string SongTitle
		{
			get
			{
				byte[] songTitle = new byte[32];

				// Seek 22 bytes
				sid.Seek(0x16, SeekOrigin.Begin);
				// Read the song title.
				sid.Read(songTitle, 0, 32);

				// Convert bytes to a string.
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songTitle);
			}
		}

		/// <summary>
		/// The track copyright. 
		/// </summary>
		public string Copyright
		{
			get
			{
				byte[] copyright = new byte[32];

				// Seek 86 bytes
				sid.Seek(0x86, SeekOrigin.Begin);

				// Read 32 bytes.
				sid.Read(copyright, 0, 32);

				// Convert bytes to a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(copyright);
			}
		}

		/// <summary>
		/// The video standard that the SID file uses.
		/// <br/>
		/// Ex. NTSC, PAL, or both.
		/// </summary>
		public string VideoStandard
		{
			get
			{
				// Seek 118 bytes
				br.BaseStream.Seek(0x76, SeekOrigin.Begin);

				// Read a short, but only get the upper 8 bytes
				int value = br.ReadInt16() >> 8;

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
		}

		/// <summary>
		/// The chip model that the SID file was made for.
		/// </summary>
		public string ChipModel
		{
			get
			{
				// Seek 118 bytes in.
				br.BaseStream.Seek(0x76, SeekOrigin.Begin);

				// Read short, but only get upper 8 bytes.
				int value = br.ReadInt16() >> 8;

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

		// Checks if the BASIC flag is set for RSID files.
		private bool BasicFlagIsSet()
		{
			// Seek 118 bytes in.
			br.BaseStream.Seek(0x76, SeekOrigin.Begin);

			// Read short, but only get upper 8 bytes.
			int value = br.ReadInt16() >> 8;

			// If the flag is set.
			if ((value & 2) != 0)
			{
				return true;
			}
			else // Flag isn't set
			{
				return false;
			}
		}

		// Make sure the binary reader cleans up properly
		public void Dispose()
		{
			br.Close();
			br.Dispose();
		}
    }
}
