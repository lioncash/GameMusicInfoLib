namespace GameMusicInfoReader.Modules.Mt2
{
	/// <summary>
	/// Message chunk (MSG) for storing a comment.
	/// </summary>
	public sealed class Message
	{
		/// <summary>
		/// Whether or not to display the message (can be ignored, doesn't matter).
		/// </summary>
		public bool ShowComment { get; internal set; }

		/// <summary>
		/// The actual comment.
		/// </summary>
		public string Comment { get; internal set; }

		public override string ToString()
		{
			return Comment;
		}
	}
}
