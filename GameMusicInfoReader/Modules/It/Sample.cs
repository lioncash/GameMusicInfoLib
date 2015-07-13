namespace GameMusicInfoReader.Modules.It
{
	/// <summary>
	/// Represents an audio sample.
	/// </summary>
	public sealed class Sample
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
}
