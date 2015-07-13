namespace GameMusicInfoReader.Modules.Mt2
{
	/// <summary>
	/// MT2 Drum Data
	/// </summary>
	public sealed class DrumData
	{
		/// <summary>
		/// Number of drum patterns.
		/// </summary>
		public int NumPatterns { get; internal set; }

		/// <summary>
		/// Drum samples
		/// </summary>
		public short[] DrumSamples { get; internal set; }

		/// <summary>
		/// Drum orders
		/// </summary>
		public byte[] PatternOrders { get; internal set; }

		/// <summary>
		/// Whether or not any drum data exists.
		/// </summary>
		public bool IsEmpty { get; internal set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public MT2DrumData()
		{
			DrumSamples = new short[8];
			PatternOrders = new byte[256];
		}
	}
}
