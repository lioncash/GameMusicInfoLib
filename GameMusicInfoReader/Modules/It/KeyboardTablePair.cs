namespace GameMusicInfoReader.Modules.It
{
	/// <summary>
	/// Instrument Note/Sample keyboard table pair.
	/// </summary>
	public sealed class KeyboardTablePair
	{
		/// <summary>Note byte</summary>
		public byte Note { get; internal set; }

		/// <summary>Sample byte</summary>
		public byte Sample { get; internal set; }
	}
}
