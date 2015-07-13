namespace GameMusicInfoReader.Modules.It
{
	/// <summary>
	/// Node point for an instrument envelope.
	/// </summary>
	public sealed class EnvelopeNodePoint
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
}
