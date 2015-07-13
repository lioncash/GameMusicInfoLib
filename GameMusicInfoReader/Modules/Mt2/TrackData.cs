namespace GameMusicInfoReader.Modules.Mt2
{
	/// <summary>
	/// Track data chunk (TRKS)
	/// </summary>
	public sealed class TrackData
	{
		/// <summary>
		/// Master volume for the track.
		/// </summary>
		public int MasterVolume { get; internal set; }

		/// <summary>
		/// Track volume
		/// </summary>
		public int TrackVolume { get; internal set; }

		/// <summary>
		/// Whether or not an effect buffer is being used for the track.
		/// </summary>
		public bool UsingEffectBuffer { get; internal set; }

		/// <summary>
		/// Whether or not this track is outputted.
		/// <para>false = Self</para>
		/// <para>true = Output track</para>
		/// </summary>
		public bool OutputTrack { get; internal set; }

		/// <summary>
		/// The effect ID for this track.
		/// </summary>
		public int EffectID { get; internal set; }

		/// <summary>
		/// Track parameters
		/// </summary>
		public short[] Parameters { get; internal set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public TrackData()
		{
			Parameters = new short[8];
		}
	}
}
