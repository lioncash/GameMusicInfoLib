using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	// TODO: Individual instrument info.

	/// <summary>
	/// A reader for Impulse Tracker modules
	/// </summary>
	public class ITReader
	{
		// Filestream representing an IT module
		private readonly FileStream it;
		private readonly BinaryReader br;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an IT module</param>
		public ITReader(string path)
		{
			it = File.OpenRead(path);
			br = new BinaryReader(it, Encoding.Default, true);
		}

		/// <summary>
		/// The eader magic of the IT module
		/// </summary>
		public string HeaderID
		{
			get
			{
				byte[] magic = new byte[4];

				// Ensure we start at the beginning of the file
				it.Seek(0, SeekOrigin.Begin);

				// Read 4 bytes
				it.Read(magic, 0, 4);

				// Convert bytes to string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(magic);
			}
		}

		/// <summary>
		/// The name of the module's track
		/// </summary>
		public string SongName
		{
			get
			{
				byte[] songName = new byte[26];

				// Seek 4 bytes in
				it.Seek(4, SeekOrigin.Begin);

				// Read 26 bytes
				it.Read(songName, 0, 26);

				// Convert bytes to string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songName);
			}
		}

		/// <summary>
		/// The total number of orders in the song
		/// </summary>
		public int TotalOrders
		{
			get
			{
				// Seek 32 bytes in
				br.BaseStream.Seek(0x20, SeekOrigin.Begin);
				return br.ReadInt16();
			}
		}

		/// <summary>
		/// The total number of instruments used in the song
		/// </summary>
		public int TotalInstruments
		{
			get
			{
				// Seek 34 bytes in
				br.BaseStream.Seek(0x22, SeekOrigin.Begin);
				return br.ReadInt16();
			}
		}

		/// <summary>
		/// The total number of samples used in the song
		/// </summary>
		public int TotalSamples
		{
			get
			{
				// Seek 36 bytes in
				br.BaseStream.Seek(0x24, SeekOrigin.Begin);
				return br.ReadInt16();
			}
		}

		/// <summary>
		/// The total number of patterns used in the song
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 38 bytes in
				br.BaseStream.Seek(0x26, SeekOrigin.Begin);
				return br.ReadInt16();
			}
		}

		/// <summary>
		/// True if the module has stereo output
		/// </summary>
		public bool IsStereo
		{
			get
			{
				// Seek 44 bytes in
				it.Seek(0x2C, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 0 is set, it's stereo
				if ((value & 1) != 0)
					return true;

				// Not set, mono.
				return false;
			}
		}

		/// <summary>
		/// True if the no mixing occurs if no mixing
		/// occurs when the volume at mixing time is 0.
		/// <remarks>Is considered a redundant flag since IT 1.04+</remarks>
		/// </summary>
		public bool HasVol0MixOptimizations
		{
			get
			{
				// Seek 44 bytes in
				it.Seek(0x2C, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 1 is set
				if ((value & 2) != 0)
					return true;

				// Not set.
				return false;
			}
		}

		/// <summary>
		/// True if the module uses instruments. <para/>
		/// False if the module uses samples.
		/// </summary>
		public bool UsesInstruments
		{
			get
			{
				// Seek 44 bytes in
				it.Seek(0x2C, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 2 is set, uses instruments
				if ((value & 4) != 0)
					return true;

				// Not set, uses samples.
				return false;
			}
		}

		/// <summary>
		/// Returns 1 if the module uses linear slides. <para/>
		/// Returns 0 if the modules uses Amiga slides.
		/// </summary>
		public int SlideType
		{
			get
			{
				// Seek 44 bytes in
				it.Seek(0x2C, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 3 is set, uses linear slides
				if ((value & 8) != 0)
					return 1;

				// Not set, Amiga slides.
				return 0;
			}
		}

		/// <summary>
		/// True if the module uses old effects. <para/>
		/// False if the module uses IT effects.
		/// </summary>
		public bool UsesOldEffects
		{
			get
			{
				// Seek 44 bytes in
				it.Seek(0x2C, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 4 is set, uses old effects
				if ((value & 16) != 0)
					return true;

				// Not set, uses IT effects
				return false;
			}
		}

		/// <summary>
		/// True if the module Link Effect G's memory with Effect E/F.
		/// </summary>
		public bool LinkEffectMemory
		{
			get
			{
				// Seek 44 bytes in
				it.Seek(0x2C, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 5 is set, links memory
				if ((value & 32) != 0)
					return true;

				// Not set, doesn't link memory
				return false;
			}
		}

		/// <summary>
		/// True if the module uses a MIDI pitch controller.
		/// </summary>
		public bool UsesMidiPitchController
		{
			get
			{
				// Seek 44 bytes in
				it.Seek(0x2C, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 6 is set, uses pitch controller.
				if ((value & 64) != 0)
					return true;

				// Not set, doesn't use pitch controller.
				return false;
			}
		}

		/// <summary>
		/// True if the module has a song message
		/// </summary>
		public bool HasSongMessage
		{
			get
			{
				// Seek 46 bytes in
				it.Seek(0x2E, SeekOrigin.Begin);

				byte value = (byte)it.ReadByte();

				// If bit 0 is set, has a song message
				if ((value & 1) != 0)
					return true;

				// Not set, doesn't have a song message
				return false;
			}
		}

		/// <summary>
		/// The global volume of the module (can be from 0 - 128)
		/// </summary>
		public int GlobalVolume
		{
			get
			{
				// Seek 48 bytes in
				it.Seek(0x30, SeekOrigin.Begin);
				return it.ReadByte();
			}
		}

		/// <summary>
		/// The mixing volume of the module (can be 0 - 128). <para/>
		/// During mixing this specifies how large the magnitude of the wave being mixed will be.
		/// </summary>
		public int MixVolume
		{
			get
			{
				// Seek 49 bytes in
				it.Seek(0x31, SeekOrigin.Begin);
				return it.ReadByte();
			}
		}

		/// <summary>
		/// The initial speed of the module
		/// </summary>
		public int InitialSpeed
		{
			get
			{
				// Seek 50 bytes in
				it.Seek(0x32, SeekOrigin.Begin);
				return it.ReadByte();
			}
		}

		/// <summary>
		/// The initial tempo of the module
		/// </summary>
		public int InitialTempo 
		{
			get
			{
				// Seek 51 bytes in
				it.Seek(0x33, SeekOrigin.Begin);
				return it.ReadByte();
			}
		}

		/// <summary>
		/// The panning separation between channels (can be within 0 - 128, with 128 being max separation)
		/// </summary>
		public int PanningSeparation
		{
			get
			{
				// Seek 52 bytes in
				it.Seek(0x34, SeekOrigin.Begin);
				return it.ReadByte();
			}
		}

		/// <summary>
		/// Pitch wheel depth for MIDI controllers.
		/// </summary>
		public int PitchWheelDepth
		{
			get
			{
				if (UsesMidiPitchController)
				{
					// Seek 53 bytes in
					it.Seek(0x35, SeekOrigin.Begin);
					return it.ReadByte();
				}

				// No MIDI pitch controller being used
				return 0;
			}
		}

		/// <summary>
		/// The song message stored in the module
		/// </summary>
		public string SongMessage
		{
			get
			{
				if (HasSongMessage)
				{
					int length = SongMessageLength;
					int offset = SongMessageOffset;

					// Seek to the song message
					br.BaseStream.Seek(offset, SeekOrigin.Begin);
					// Read the message length
					byte[] messageBytes = br.ReadBytes(length);

					// Convert bytes to a string
					UTF8Encoding encoding = new UTF8Encoding();
					string msg = encoding.GetString(messageBytes);

					// Correctly replace 0xD with the newline.
					return msg.Replace((char)0xD, '\n');
				}

				// No song message
				return "<No Message>";
			}
		}

		// Offset to the song message in the IT module
		private int SongMessageOffset
		{
			get
			{
				if (HasSongMessage)
				{
					// Seek 56 bytes in
					br.BaseStream.Seek(0x38, SeekOrigin.Begin);
					return br.ReadInt32();
				}

				// No song message
				return 0;
			}
		}

		// The length of the message embedded in the IT module
		private int SongMessageLength
		{
			get
			{
				if (HasSongMessage)
				{
					// Seek 54 bytes in
					br.BaseStream.Seek(0x36, SeekOrigin.Begin);
					return br.ReadInt16();
				}

				// No song message
				return 0;
			}
		}
	}
}
