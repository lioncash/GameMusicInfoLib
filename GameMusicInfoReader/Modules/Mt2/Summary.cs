namespace GameMusicInfoReader.Modules.Mt2
{
	/// <summary>
	/// Summary (SUM) chunk
	/// </summary>
	public sealed class Summary
	{
		/// <summary>
		/// Build summary mask
		/// </summary>
		public long SummaryMask { get; internal set; }

		/// <summary>
		/// Build summary content.
		/// </summary>
		public string Content { get; internal set; }

		public override string ToString()
		{
			return Content;
		}
	}
}
