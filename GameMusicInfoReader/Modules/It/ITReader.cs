using System.IO;

namespace GameMusicInfoReader.Modules.It
{
	/// <summary>
	/// A reader for Impulse Tracker modules
	/// </summary>
	public sealed class ITReader
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an IT module</param>
		public ITReader(string path)
		{
			using (var br = new BinaryReader(File.OpenRead(path)))
			{
				HeaderID = new string(br.ReadChars(4));
				SongName = new string(br.ReadChars(26));
				PatternHighlightInfo = br.ReadInt16();

				// Totals
				TotalOrders      = br.ReadInt16();
				TotalInstruments = br.ReadInt16();
				TotalSamples     = br.ReadInt16();
				TotalPatterns    = br.ReadInt16();

				// Tracker related
				CreatedWithTracker    = br.ReadInt16();
				CompatibleWithTracker = br.ReadInt16();

				// Flags
				short flag = br.ReadInt16();
				IsStereo                = ((flag &  1) != 0);
				HasVol0MixOptimizations = ((flag &  2) != 0);
				UsesInstruments         = ((flag &  4) != 0);
				SlideType               = ((flag &  8) != 0) ? SlideTypes.Linear : SlideTypes.Amiga;
				UsesOldEffects          = ((flag & 16) != 0);
				LinkEffectMemory        = ((flag & 32) != 0);
				UsesMidiPitchController = ((flag & 64) != 0);

				// Special flags
				short special = br.ReadInt16();
				HasSongMessage = ((special & 1) != 0);

				// Volume and Tempo related stuff
				GlobalVolume      = br.ReadByte();
				MixVolume         = br.ReadByte();
				InitialSpeed      = br.ReadByte();
				InitialTempo      = br.ReadByte();
				PanningSeparation = br.ReadByte();
				PitchWheelDepth   = br.ReadByte();

				// Song message
				short messageLen = br.ReadInt16();
				int messageOffset = br.ReadInt32();
				br.BaseStream.Position = messageOffset;
				if (HasSongMessage)
					SongMessage = new string(br.ReadChars(messageLen)).Replace((char) 0xD, '\n');
				else
					SongMessage = "N/A";

				// Channel related
				TotalUsedChannels = GetTotalUsedChannels(br);
				ChannelPanning    = GetChannelPanning(br);
				ChannelVolumes    = GetChannelVolumes(br);

				// Parsing
				ParseInstruments(br);
				ParseSamples(br);
			}
		}

		#endregion

		#region Generic Info

		/// <summary>
		/// The header magic of the IT module
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the module's track
		/// </summary>
		public string SongName
		{
			get;
			private set;
		}

		/// <summary>
		/// Pattern row hilight information.
		/// Only relevant for pattern editing situations.
		/// </summary>
		public short PatternHighlightInfo
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of orders in the song
		/// </summary>
		public int TotalOrders
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of instruments used in the song
		/// </summary>
		public int TotalInstruments
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of samples used in the song
		/// </summary>
		public int TotalSamples
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of patterns used in the song
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// Version of ImpulseTracker that created the module.
		/// </summary>
		public short CreatedWithTracker
		{
			get;
			private set;
		}

		/// <summary>
		/// The major release version of impulse tracker that this module is compatible with
		/// </summary>
		public int CompatibleWithTracker
		{
			get;
			private set;
		}

		/// <summary>
		/// True if the module has stereo output
		/// </summary>
		public bool IsStereo
		{
			get;
			private set;
		}

		/// <summary>
		/// True if the no mixing occurs if no mixing
		/// occurs when the volume at mixing time is 0.
		/// </summary>
		/// <remarks>
		/// Is considered a redundant flag since IT 1.04+
		/// </remarks>
		public bool HasVol0MixOptimizations
		{
			get;
			private set;
		}

		/// <summary>
		/// <para>True if the module uses instruments. </para>
		/// <para>False if the module uses samples.    </para>
		/// </summary>
		public bool UsesInstruments
		{
			get;
			private set;
		}

		/// <summary>
		/// Slide type used by this module.
		/// </summary>
		public SlideTypes SlideType
		{
			get;
			private set;
		}

		/// <summary>
		/// <para>True if the module uses old effects. </para>
		/// <para>False if the module uses IT effects. </para>
		/// </summary>
		public bool UsesOldEffects
		{
			get;
			private set;
		}

		/// <summary>
		/// True if the module Link Effect G's memory with Effect E/F.
		/// </summary>
		public bool LinkEffectMemory
		{
			get;
			private set;
		}

		/// <summary>
		/// True if the module uses a MIDI pitch controller.
		/// </summary>
		public bool UsesMidiPitchController
		{
			get;
			private set;
		}

		/// <summary>
		/// True if the module has a song message
		/// </summary>
		public bool HasSongMessage
		{
			get;
			private set;
		}

		/// <summary>
		/// The global volume of the module (can be from 0 - 128)
		/// </summary>
		public int GlobalVolume
		{
			get;
			private set;
		}

		/// <summary>
		/// The mixing volume of the module (can be 0 - 128). <para/>
		/// During mixing this specifies how large the magnitude of the wave being mixed will be.
		/// </summary>
		public int MixVolume
		{
			get;
			private set;
		}

		/// <summary>
		/// The initial speed of the module
		/// </summary>
		public int InitialSpeed
		{
			get;
			private set;
		}

		/// <summary>
		/// The initial tempo of the module
		/// </summary>
		public int InitialTempo 
		{
			get;
			private set;
		}

		/// <summary>
		/// The panning separation between channels (can be within 0 - 128, with 128 being max separation)
		/// </summary>
		public int PanningSeparation
		{
			get;
			private set;
		}

		/// <summary>
		/// Pitch wheel depth for MIDI controllers.
		/// </summary>
		public int PitchWheelDepth
		{
			get;
			private set;
		}

		/// <summary>
		/// The song message stored in the module
		/// </summary>
		public string SongMessage
		{
			get;
			private set;
		}

		/// <summary>
		/// The number of used channels.
		/// <para></para>
		/// Has a max of 64 channels
		/// </summary>
		public int TotalUsedChannels
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets all the channel panning values for each channel as an array.
		/// <para></para>
		/// Note that since arrays are 0-based, the total indexes for the channels are 'totalChannels - 1'
		/// <para></para>
		/// ie). A 22 channel IT module will have an index of (0 - 21).
		/// So, channel one is channel[0], etc
		/// </summary>
		///
		/// <remarks>
		/// Panning values for each channel are within the range (0 - 64), where:
		/// <para>0 = Absolute left   </para>
		/// <para>32 = Central pan    </para>
		/// <para>64 = Absolute right </para>
		/// </remarks>
		public int[] ChannelPanning
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the volumes of all channels and stores them in an array.
		/// </summary>
		/// <remarks>
		/// Note that since arrays are 0-based, the total indexes for the channel volumes are 'totalChannels - 1'
		/// ie). A 22 channel IT module will have an index of (0 - 21).
		/// So, channel one is channel[0], etc
		/// Channel volumes can be from (0 - 64)
		/// </remarks>
		public int[] ChannelVolumes
		{
			get;
			private set;
		}

		private static int GetTotalUsedChannels(BinaryReader br)
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

		private int[] GetChannelPanning(BinaryReader br)
		{
			int[] channels = new int[TotalUsedChannels];

			// Seek to the channel panning data
			br.BaseStream.Seek(0x40, SeekOrigin.Begin);

			// Read data into each channel
			for (int i = 0; i < TotalUsedChannels; i++)
			{
				channels[i] = br.ReadByte();
			}

			return channels;
		}

		private int[] GetChannelVolumes(BinaryReader br)
		{
			int[] channelVolumes = new int[TotalUsedChannels];

			// Seek to channel volume data
			br.BaseStream.Seek(0x80, SeekOrigin.Begin);

			// Get all the channel volumes
			for (int i = 0; i < TotalUsedChannels; i++)
			{
				channelVolumes[i] = br.ReadByte();
			}

			return channelVolumes;
		}

		/// <summary>
		/// Slide type used by this module.
		/// </summary>
		public enum SlideTypes
		{
			Amiga,
			Linear
		}

		#endregion

		#region Instruments

		/// <summary>
		/// Instruments within this module.
		/// </summary>
		public Instrument[] Instruments
		{
			get;
			private set;
		}

		private void ParseInstruments(BinaryReader br)
		{
			// Initialize instruments array.
			Instruments = new Instrument[TotalInstruments];

			// Get offset to the beginning of the instrument data.
			int offsetLocation = 0xC0 + TotalOrders;
			br.BaseStream.Position = offsetLocation;
			int instrumentDataOffset = br.ReadInt32();
			br.BaseStream.Position = instrumentDataOffset; // Set position to the first instrument.

			// Go over the instruments
			for (int i = 0; i < TotalInstruments; i++)
			{
				Instruments[i].InstrumentID           = new string(br.ReadChars(4));
				Instruments[i].DOSFilename            = new string(br.ReadChars(13));
				Instruments[i].NewNoteAction          = br.ReadByte();
				Instruments[i].DuplicateCheckType     = br.ReadByte();
				Instruments[i].DuplicateCheckAction   = br.ReadByte();
				Instruments[i].FadeOut                = br.ReadInt16();
				Instruments[i].PitchPanSeparation     = br.ReadSByte();
				Instruments[i].PitchPanCenter         = br.ReadByte();
				Instruments[i].GlobalVolume           = br.ReadByte();
				Instruments[i].DefaultPan             = br.ReadByte();
				Instruments[i].RandomVolumeVariation  = br.ReadByte();
				Instruments[i].RandomPanningVariation = br.ReadByte();
				Instruments[i].TrackerVersion         = br.ReadInt16();
				Instruments[i].NumAssociatedSamples   = br.ReadByte();
				br.BaseStream.Position += 1; // Skip 1 byte (unused)
				Instruments[i].InstrumentName         = new string(br.ReadChars(26));
				Instruments[i].InitialFilterCutoff    = br.ReadByte();
				Instruments[i].InitialFilterResonance = br.ReadByte();
				Instruments[i].MidiChannel            = br.ReadByte();
				Instruments[i].MidiProgram            = br.ReadByte();
				Instruments[i].MidiBank               = br.ReadInt16();

				// Set up the note/sample keyboard table.
				// These are read in pairs.
				Instruments[i].KeyboardTable = new KeyboardTablePair[120];
				for (int j = 0; j < Instruments[i].KeyboardTable.Length; j++)
				{
					Instruments[i].KeyboardTable[j].Note   = br.ReadByte();
					Instruments[i].KeyboardTable[j].Sample = br.ReadByte();
				}

				// Now finally read the envelopes.
				// There are always three of these per instrument.
				Instruments[i].Envelopes = new Envelope[3];
				for (int j = 0; j < Instruments[i].Envelopes.Length; j++)
				{
					// Envelope flags
					byte flags = br.ReadByte();
					Instruments[i].Envelopes[j].EnvelopeEnabled = ((flags & 1) != 0);
					Instruments[i].Envelopes[j].Looping         = ((flags & 2) != 0);
					Instruments[i].Envelopes[j].SustainLoop     = ((flags & 4) != 0);
					if (j == 2) // Pitch envelope
						Instruments[i].Envelopes[j].UseAsFilter = ((flags & 128) != 0);

					// Total node points
					Instruments[i].Envelopes[j].NumNodePoints = br.ReadByte();

					// Looping values
					Instruments[i].Envelopes[j].LoopBeginning        = br.ReadByte();
					Instruments[i].Envelopes[j].LoopEnd              = br.ReadByte();
					Instruments[i].Envelopes[j].SustainLoopBeginning = br.ReadByte();
					Instruments[i].Envelopes[j].SustainLoopEnd       = br.ReadByte();

					// Set up the node points.
					const int totalNodeSets = 25;
					Instruments[i].Envelopes[j].NodePoints = new EnvelopeNodePoint[totalNodeSets];
					for (int k = 0; k < totalNodeSets; k++)
					{
						Instruments[i].Envelopes[j].NodePoints[k].YValue     = br.ReadSByte();
						Instruments[i].Envelopes[j].NodePoints[k].TickNumber = br.ReadInt16();
					}
				}
				
				// 7 bytes are wasted per instrument.
				br.BaseStream.Position += 7;
			}
		}

		#endregion

		#region Samples

		/// <summary>
		/// Samples within this module file.
		/// </summary>
		public Sample[] Samples
		{
			get;
			private set;
		}

		private void ParseSamples(BinaryReader br)
		{
			// Initialize sample array
			Samples = new Sample[TotalSamples];

			// Locate start of the sample data.
			int initialLoc = 0xC0 + TotalOrders + (TotalInstruments*4);
			br.BaseStream.Position = initialLoc;
			int offset = br.ReadInt16();
			br.BaseStream.Position = offset; // Start of the first sample.

			// Read in all of the sample data.
			for (int i = 0; i < TotalSamples; i++)
			{
				Samples[i].SampleID = new string(br.ReadChars(4));
				Samples[i].DOSFilename = new string(br.ReadChars(13));
				Samples[i].GlobalVolume = br.ReadByte();

				// Flags
				byte flags = br.ReadByte();
				Samples[i].Is16Bit                   = ((flags &   2) != 0);
				Samples[i].IsStereo                  = ((flags &   4) != 0);
				Samples[i].CompressedSamples         = ((flags &   8) != 0);
				Samples[i].IsLooped                  = ((flags &  16) != 0);
				Samples[i].UsesSustainedLoop         = ((flags &  32) != 0);
				Samples[i].UsesPingPongLoop          = ((flags &  64) != 0);
				Samples[i].UsesSustainedPingPongLoop = ((flags & 128) != 0);

				Samples[i].DefaultVolume = br.ReadByte();
				Samples[i].SampleName    = new string(br.ReadChars(26));

				// Convert flag
				byte convert = br.ReadByte();
				Samples[i].UsesUnsignedSamples = ((convert & 1) != 0);

				// Default panning bits
				Samples[i].DefaultPanBits = br.ReadByte();

				// Length and loop related
				Samples[i].Length             = br.ReadInt32();
				Samples[i].LoopBegin          = br.ReadInt32();
				Samples[i].LoopEnd            = br.ReadInt32();
				Samples[i].C5Seconds          = br.ReadInt32();
				Samples[i].SustainedLoopBegin = br.ReadInt32();
				Samples[i].SustainedLoopEnd   = br.ReadInt32();
				Samples[i].SamplePointer      = br.ReadInt32();

				// Vibrato
				Samples[i].VibratoSpeed = br.ReadByte();
				Samples[i].VibratoDepth = br.ReadByte();
				Samples[i].VibratoRate  = br.ReadByte();
				Samples[i].VibratoType  = br.ReadByte();
			}
		}

		#endregion
	}
}
