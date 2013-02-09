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
		/// The major release version of impulse tracker that this module is compatible with
		/// </summary>
		public int CompatibleWithTracker
		{
			get
			{
				// Seek 42 bytes in
				it.Seek(0x2A, SeekOrigin.Begin);
				return it.ReadByte();
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

		/// <summary>
		/// The number of used channels.
		/// <para/>
		/// Has a max of 64 channels
		/// </summary>
		public int TotalChannels
		{
			get
			{
				int count = 0;
				br.BaseStream.Seek(0x40, SeekOrigin.Begin);

				// -1 signifies channels that haven't been set.
				while (br.ReadSByte() != -1)
				{
					// 0x7F = end of all channels
					// So, kill the loop if we hit it
					if (br.BaseStream.Position == 0x7F)
					{
						count++;
						break;
					}

					// Continue counting channels
					count++;
				}

				return count;
			}
		}

		/// <summary>
		/// Gets all the channel panning values for each channel as an array.
		/// <para/>
		/// Note that since arrays are 0-based, the total indexes for the channels are 'totalChannels - 1'
		/// <para/>
		/// ie). A 22 channel IT module will have an index of (0 - 21). <para/>
		/// So, channel one is channel[0], etc
		/// </summary>
		///
		/// <remarks>
		/// Panning values for each channel are within the range (0 - 64), where: <para/>
		/// 0 = Absolute left <para/>
		/// 32 = Central pan <para/>
		/// 64 = Absolute right
		/// </remarks>
		public int[] ChannelPanning
		{
			get
			{
				int totalChannels = TotalChannels;
				int[] channels = new int[totalChannels];

				// Seek to the channel panning data
				it.Seek(0x40, SeekOrigin.Begin);

				// Read data into each channel
				for (int i = 0; i < totalChannels; i++)
				{
					channels[i] = it.ReadByte();
				}

				return channels;
			}
		}

		/// <summary>
		/// Gets the volumes of all channels and stores them in an array.
		/// </summary>
		/// <remarks>
		/// Note that since arrays are 0-based, the total indexes for the channel volumes are 'totalChannels - 1' <para/>
		/// ie). A 22 channel IT module will have an index of (0 - 21). <para/>
		/// So, channel one is channel[0], etc <para/><para/>
		/// Channel volumes can be from (0 - 64)
		/// </remarks>
		public int[] ChannelVolumes
		{
			get
			{
				int totalChannels = TotalChannels;
				int[] channelVolumes = new int[totalChannels];

				// Seek to channel volume data
				it.Seek(0x80, SeekOrigin.Begin);

				// Get all the channel volumes
				for (int i = 0; i < totalChannels; i++)
				{
					channelVolumes[i] = it.ReadByte();
				}

				return channelVolumes;
			}
		}

		//////////////////////////////
		//--- INSTRUMENT READING ---//
		//////////////////////////////

		// NOTE: The magic number 557 is the length of an individual instrument header

		// The offset to the instrument data
		private int InstrumentHeaderOffset
		{
			get
			{
				// Location of the offset within the IT module
				int offsetLocation = 0xC0 + TotalOrders;
				
				// Seek to the location
				br.BaseStream.Seek(offsetLocation, SeekOrigin.Begin);

				// Read the offset to the instrument data
				return br.ReadInt32();
			}
		}

		/// <summary>
		/// The specified instrument's new note action
		/// </summary>
		/// <param name="instrument">The instrument to get the new note action of</param>
		/// <returns>
		/// -1 = When entering a value larger than TotalInstruments, or less than 0.<para/>
		/// 0 = Cut note <para/>
		/// 1 = Continue <para/>
		/// 2 = Note off <para/>
		/// 3 = Note fade.
		/// </returns>
		/// <remarks>
		/// This method is ZERO-BASED. ie) channel 1 = NewNoteAction(0)
		/// </remarks>
		public int NewNoteAction(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -1;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557*instrument); // The instrument number we're visiting
			
			// Seek to the new note action byte of the instrument
			it.Seek(instrumentNum + 0x11, SeekOrigin.Begin);

			return it.ReadByte();
		}

		/// <summary>
		/// The Duplicate Check Type of an instrument
		/// </summary>
		/// <param name="instrument">The instrument to get the duplicate check type information from.</param>
		/// <returns>
		/// -1 = Trying to pass a non-existant instrument <para/>
		/// 0 = Off <para/>
		/// 1 = Note <para/>
		/// 2 = Sample <para/>
		/// 3 = Instrument
		/// </returns>
		/// <remarks>
		/// This method is ZERO-BASED. ie) channel 1 = DuplicateCheckType(0)
		/// </remarks>
		public int DuplicateCheckType(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -1;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557*instrument);

			// Seek to the duplicate check type byte of the instrument
			it.Seek(instrumentNum + 0x12, SeekOrigin.Begin);

			return it.ReadByte();
		}

		/// <summary>
		/// The Duplicate Check Action (DCA) of the specified instrument
		/// </summary>
		/// <param name="instrument">The instrument to get the DCA value from</param>
		/// <returns>
		/// -1 = Trying to pass a non-existant instrument <para/>
		/// 0 = Cut <para/>
		/// 1 = Note Off <para/>
		/// 2 = Note Fade
		/// </returns>
		/// <remarks>
		/// This method is ZERO-BASED. ie) channel 1 = DuplicateCheckAction(0)
		/// </remarks>
		public int DuplicateCheckAction(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -1;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557 * instrument);

			// Seek to the duplicate check action byte of the instrument
			it.Seek(instrumentNum + 0x13, SeekOrigin.Begin);

			return it.ReadByte();
		}

		/// <summary>
		/// Fadeout of an instrument. <para/>
		/// Ranges between (0 - 128)
		/// </summary>
		/// <param name="instrument">The instrument to get the fadeout length of</param>
		/// <returns>The fadeout length (0 - 128)</returns>
		/// <remarks>
		/// This method is ZERO-BASED. ie) channel 1 = FadeOut(0)
		/// </remarks>
		public int FadeOut(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -1;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557 * instrument);

			// Seek to the fadeout byte of the instrument
			it.Seek(instrumentNum + 0x14, SeekOrigin.Begin);

			return it.ReadByte();
		}

		/// <summary>
		/// The Pitch-Pan separation of a specified instrument
		/// </summary>
		/// <param name="instrument">The instrument to get the PPS of</param>
		/// 
		/// <returns>
		/// The PPS of the instrument (-32 - 32)<para/>
		/// Returns -33 when trying to pass a non-existant instrument
		/// </returns>
		/// 
		/// <remarks>
		/// This method is ZERO-BASED. ie) channel 1 = PitchPanSeparation(0)
		/// </remarks>
		public int PitchPanSeparation(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -33;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557 * instrument);

			// Seek to the PPS byte of the instrument
			br.BaseStream.Seek(instrumentNum + 0x16, SeekOrigin.Begin);

			return br.ReadSByte();
		}

		/// <summary>
		/// Default panning for the instrument
		/// </summary>
		/// <param name="instrument">The instrument to get the default panning value from</param>
		/// <returns>
		/// -1 = Trying to pass a non-existant instrument <para/>
		/// Returns the default panning value (0 - 64)
		/// </returns>
		public int DefaultPan(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -1;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557 * instrument);

			// Seek to the DefaultPan byte of the instrument
			it.Seek(instrumentNum + 0x19, SeekOrigin.Begin);

			return it.ReadByte();
		}

		/// <summary>
		/// The random volume variation of the instrument
		/// </summary>
		/// <param name="instrument">The instrument to get the RVV of.</param>
		/// <returns>
		/// -1 = Trying to pass a non-existant instrument<para/>
		/// Returns the random volume variation as a percentage between (0 - 100)
		/// </returns>
		public int RandomVolumeVariation(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -1;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557 * instrument);

			// Seek to the random volume variation byte
			it.Seek(instrumentNum + 0x1A, SeekOrigin.Begin);

			return it.ReadByte();
		}

		/// <summary>
		/// The number of samples associated with the specified instrument
		/// </summary>
		/// <param name="instrument">The instrument to get the number of associated samples from</param>
		/// <returns>
		/// -1 = Trying to pass a non-existant instrument
		/// The number of samples associated with the specified instrument
		/// </returns>
		public int AssociatedSamples(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return -1;

			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557 * instrument);

			// Seek to the number of samples byte
			it.Seek(instrumentNum + 0x1E, SeekOrigin.Begin);

			return it.ReadByte();
		}

		/// <summary>
		/// The name of the instrument
		/// </summary>
		/// <param name="instrument">The instrument to get the name of</param>
		/// <returns>
		/// The name of the instrument. <para/>
		/// Null, if an invalid instrument is passed
		/// </returns>
		public string InstrumentName(int instrument)
		{
			// Trying to get an instrument that doesn't exist
			if (instrument < 0 || instrument > TotalInstruments)
				return null;

			byte[] instrumentName = new byte[26];
			int instrumentData = InstrumentHeaderOffset;
			int instrumentNum = instrumentData + (557 * instrument);

			// Seek to the instrument name
			it.Seek(instrumentNum + 0x20, SeekOrigin.Begin);
			it.Read(instrumentName, 0, 26);

			// Convert bytes to string
			UTF8Encoding encoding = new UTF8Encoding();
			return encoding.GetString(instrumentName);
		}

		///////////////////////////
		// Sample Header Reading //
		///////////////////////////
		
		// Offset to the sample headers
		private int SampleHeaderOffset
		{
			get
			{
				// Location of the offset within the IT module
				int initialLocation = 0xC0 + TotalOrders + (TotalInstruments * 4);
				// Seek to the location of the offset
				br.BaseStream.Seek(initialLocation, SeekOrigin.Begin);
				// Get the offset
				return br.ReadInt16();
			}
		}

		// TODO: Sample reading, Pattern reading
	}
}
