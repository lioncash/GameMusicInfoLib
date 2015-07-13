namespace GameMusicInfoReader.Modules.It
{
	/// <summary>
	/// Represents an instrument envelope.
	/// </summary>
	/// <remarks>
	/// Within an instrument, three envelopes are present:
	/// <para>One for volume</para>
	/// <para>One for panning</para>
	/// <para>One for pitch</para>
	/// </remarks>
	public sealed class Envelope
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
}
