using System.IO;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for Impulse Tracker modules
	/// </summary>
	public sealed class ITReader
	{
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

		private int GetTotalUsedChannels(BinaryReader br)
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

		#region Instrument Structure

		/// <summary>
		/// Represents a single instrument in this module.
		/// </summary>
		public struct Instrument
		{
			/// <summary>Instrument header magic</summary>
			public string InstrumentID { get; internal set; }

			/// <summary>DOS filename</summary>
			public string DOSFilename { get; internal set; }

			/// <summary>
			/// The specified instrument's new note action
			/// </summary>
			/// <value>
			/// <para>0 = Cut note </para>
			/// <para>1 = Continue </para>
			/// <para>2 = Note off </para>
			/// <para>3 = Note fade.</para>
			/// </value>
			public int NewNoteAction { get; internal set; }

			/// <summary>
			/// The Duplicate Check Type of an instrument
			/// </summary>
			/// <value>
			/// <para>0 = Off</para>
			/// <para>1 = Note</para>
			/// <para>2 = Sample</para>
			/// <para>3 = Instrument</para>
			/// </value>
			public int DuplicateCheckType { get; internal set; }

			/// <summary>
			/// The Duplicate Check Action (DCA) of the specified instrument
			/// </summary>
			/// <value>
			/// <para>0 = Cut </para>
			/// <para>1 = Note Off </para>
			/// <para>2 = Note Fade </para>
			/// </value>
			public int DuplicateCheckAction { get; internal set; }

			/// <summary>
			/// Fadeout of an instrument.
			/// Ranges between (0 - 128)
			/// </summary>
			/// <remarks>
			/// Fade applied when:
			/// <para>1) Note fade NNA is selected and triggered (by another note)</para>
			/// <para>2) Note off NNA is selected with no volume envelope or volume envelope loop</para>
			/// <para>3) Volume envelope end is reached</para>
			/// </remarks>
			/// <value>The fadeout length (0 - 128)</value>
			public int FadeOut { get; internal set; }

			/// <summary>
			/// The Pitch-Pan separation of a specified instrument
			/// </summary>
			/// <returns>
			/// The PPS of the instrument (-32 - 32)
			/// </returns>
			public sbyte PitchPanSeparation { get; internal set; }

			/// <summary>
			/// Pitch-pan center
			/// </summary>
			public byte PitchPanCenter { get; internal set; }

			/// <summary>
			/// Global volume
			/// </summary>
			/// <value>From 0 to 128</value>
			public byte GlobalVolume { get; internal set; }

			/// <summary>
			/// Default panning value
			/// </summary>
			/// <value>From 0 to 64. 128 means it's unused.</value>
			public byte DefaultPan { get; internal set; }

			/// <summary>
			/// Random volume variation (percentage)
			/// </summary>
			public byte RandomVolumeVariation { get; internal set; }

			/// <summary>
			/// Random panning variation (panning change - not implemented yet)
			/// </summary>
			public byte RandomPanningVariation { get; internal set; }

			/// <summary>
			/// Tracker version used to save the instrument.
			/// This is only used in the instrument files.
			/// </summary>
			public short TrackerVersion { get; internal set; }

			/// <summary>
			/// Number of samples associated with instrument.
			/// This is only used in the instrument files.
			/// </summary>
			public byte NumAssociatedSamples { get; internal set; }

			/// <summary>
			/// Instrument name
			/// </summary>
			public string InstrumentName { get; internal set; }

			/// <summary>
			/// Initial filter cutoff
			/// </summary>
			public byte InitialFilterCutoff { get; internal set; }

			/// <summary>
			/// Initial filter resonance
			/// </summary>
			public byte InitialFilterResonance { get; internal set; }

			/// <summary>
			/// MIDI channel number
			/// </summary>
			public int MidiChannel { get; internal set; }

			/// <summary>
			/// MIDI Program (instrument)
			/// </summary>
			public short MidiProgram { get; internal set; }

			/// <summary>
			/// MIDI Bank? Purpose not explained in the format documentation. (only referred to as MidiBnk)
			/// TODO: Figure out wtf this is used for and document it.
			/// </summary>
			public short MidiBank { get; internal set; }

			/// <summary>
			/// Each note of the instrument is first converted to a sample number
			/// and a note (C-0 -> B-9). These are stored as note/sample byte pairs
			/// (note first, range 0->119 for C-0 to B-9, sample ranges from 1-99, 0=no sample)
			/// </summary>
			/// <remarks>
			/// Overall, this table is 240 bytes in length if it were to be read entirely
			/// in one pass, instead of being broken into struct pairs.
			/// </remarks>
			public KeyboardTablePair[] KeyboardTable { get; internal set; }

			/// <summary>
			/// Instrument envelopes
			/// </summary>
			public Envelope[] Envelopes { get; internal set; }
		}

		/// <summary>
		/// Instrument Note/Sample keyboard table pair.
		/// </summary>
		public struct KeyboardTablePair
		{
			/// <summary>Note byte</summary>
			public byte Note { get; internal set; }

			/// <summary>Sample byte</summary>
			public byte Sample { get; internal set; }
		}

		/// <summary>
		/// Represents an instrument envelope.
		/// </summary>
		/// <remarks>
		/// Within an instrument, three envelopes are present:
		/// <para>One for volume</para>
		/// <para>One for panning</para>
		/// <para>One for pitch</para>
		/// </remarks>
		public struct Envelope
		{
			/// <summary>Whether or not this envelope is enabled</summary>
			public bool EnvelopeEnabled { get; internal set; }

			/// <summary>Whether or not to loop this envelope</summary>
			public bool Looping { get; internal set; }

			/// <summary>Whether or not to sustain the loop</summary>
			public bool SustainLoop { get; internal set; }

			/// <summary>Whether or not to use this envelope as a filter envelope (pitch envelope only)</summary>
			public bool UseAsFilter { get; internal set; }

			/// <summary>Number of node points in this envelope</summary>
			public int NumNodePoints { get; internal set; }

			/// <summary>Loop beginning</summary>
			public int LoopBeginning { get; internal set; }

			/// <summary>Loop end</summary>
			public int LoopEnd { get; internal set; }

			/// <summary>Sustain loop beginning</summary>
			public int SustainLoopBeginning { get; internal set; }

			/// <summary>Sustain loop end</summary>
			public int SustainLoopEnd { get; internal set; }

			/// <summary>
			/// Envelope node points
			/// </summary>
			public EnvelopeNodePoint[] NodePoints { get; internal set; }
		}

		/// <summary>
		/// Node point for an instrument envelope.
		/// </summary>
		public struct EnvelopeNodePoint
		{
			/// <summary>
			/// Y Value.
			/// </summary>
			/// <remarks>
			/// (0->64 for vol, -32->+32 for panning or pitch)
			/// </remarks>
			public sbyte YValue { get; internal set; }

			/// <summary>
			/// Tick number
			/// </summary>
			public short TickNumber { get; internal set; }
		}

		#endregion
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

		#region Sample Structure

		/// <summary>
		/// Represents an audio sample.
		/// </summary>
		public struct Sample
		{
			/// <summary>
			/// Sample header ID
			/// </summary>
			public string SampleID { get; internal set; }

			/// <summary>
			/// DOS filename
			/// </summary>
			public string DOSFilename { get; internal set; }

			/// <summary>
			/// Global volume for instrument
			/// <para>Ranges from 0-64</para>
			/// </summary>
			public int GlobalVolume { get; internal set; }

			/// <summary>
			/// Whether or not this sample is associated with the header.
			/// </summary>
			public bool SampleAssocWithHeader { get; internal set; }

			/// <summary>
			/// Whether or not this sample is 16-bits.
			/// <para>If false, then this sample is 8-bit.</para>
			/// </summary>
			public bool Is16Bit { get; internal set; }

			/// <summary>
			/// Whether or not this sample is stereo.
			/// <para>If false, then this sample is mono.</para>
			/// </summary>
			public bool IsStereo { get; internal set; }

			/// <summary>
			/// Whether or not this sample is compressed.
			/// </summary>
			public bool CompressedSamples { get; internal set; }

			/// <summary>
			/// Whether or not this sample is looped.
			/// </summary>
			public bool IsLooped { get; internal set; }

			/// <summary>
			/// Whether or not this sample uses a sustained loop.
			/// </summary>
			public bool UsesSustainedLoop { get; internal set; }

			/// <summary>
			/// Whether or not this sample uses a ping-pong loop.
			/// <para>If false, then this sample uses a forwards loop.</para>
			/// </summary>
			public bool UsesPingPongLoop { get; internal set; }

			/// <summary>
			/// Whether or not this sample uses a sustained ping-pong loop.
			/// <para>If false, then this sample uses a sustained forwards loop.</para>
			/// </summary>
			public bool UsesSustainedPingPongLoop { get; internal set; }

			/// <summary>
			/// The default volume for this instrument.
			/// </summary>
			public int DefaultVolume { get; internal set; }

			/// <summary>
			/// Name of this sample.
			/// </summary>
			public string SampleName { get; internal set; }

			/// <summary>
			/// Whether or not this sample uses unsigned samples.
			/// <para>If false, then this sample uses signed samples.</para>
			/// </summary>
			public bool UsesUnsignedSamples { get; internal set; }

			/// <summary>
			/// Default Panning Bits 0->6 = Pan value, Bit 7 ON to USE (opposite of inst)
			/// </summary>
			public byte DefaultPanBits { get; internal set; }

			/// <summary>
			/// Length of the sample instrument in number of samples.
			/// </summary>
			public int Length { get; internal set; }

			/// <summary>
			/// Start of loop (number of samples in)
			/// </summary>
			public int LoopBegin { get; internal set; }

			/// <summary>
			/// Sample number after the end of the loop.
			/// </summary>
			public int LoopEnd { get; internal set; }

			/// <summary>
			/// Number of bytes a second for C-5 (ranges from 0->9999999)
			/// </summary>
			public int C5Seconds { get; internal set; }

			/// <summary>
			/// Start of sustain loop (number of samples in)
			/// </summary>
			public int SustainedLoopBegin { get; internal set; }

			/// <summary>
			/// Sample number after the end of the sustain loop.
			/// </summary>
			public int SustainedLoopEnd { get; internal set; }

			/// <summary>
			/// Offset of the sample in the file.
			/// </summary>
			public int SamplePointer { get; internal set; }

			/// <summary>
			/// Vibrato Speed, ranges from 0->64
			/// </summary>
			public int VibratoSpeed { get; internal set; }

			/// <summary>
			/// Vibrato Depth, ranges from 0->64
			/// </summary>
			public int VibratoDepth { get; internal set; }

			/// <summary>
			/// The vibrato waveform type of the sample.
			/// </summary>
			/// <value>
			/// <para>0 = Sine wave </para>
			/// <para>1 = Ramp down </para>
			/// <para>2 = Square wave </para>
			/// <para>3 = Random (speed is irrelevant) </para>
			/// </value>
			public int VibratoType { get; internal set; }

			/// <summary>
			/// Vibrato Rate, rate at which vibrato is applied (0->64)
			/// </summary>
			public int VibratoRate { get; internal set; }
		}

		#endregion

		#endregion
	}
}
