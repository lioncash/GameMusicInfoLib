using System.Collections.ObjectModel;
using System.IO;

namespace GameMusicInfoReader.Modules.Mod
{
	/// <summary>
	/// Represents a note pattern for a ProTracker module.
	/// </summary>
	public sealed class Pattern
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reader">Reader to get the pattern data from.</param>
		public Pattern(BinaryReader reader)
		{
			Notes = GetNotes(reader);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Note data for this pattern.
		/// </summary>
		public ReadOnlyCollection<Note> Notes
		{
			get;
			private set;
		}

		#endregion

		#region Helper Functions

		private static ReadOnlyCollection<Note> GetNotes(BinaryReader reader)
		{
			var noteArray = new Note[256];

			for (int i = 0; i < noteArray.Length; i++)
				noteArray[i] = new Note(reader.ReadBytes(4));

			return new ReadOnlyCollection<Note>(noteArray);
		}

		#endregion
	}
}
