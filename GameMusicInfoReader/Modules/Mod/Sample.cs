using System;
using GameMusicInfoReader.Util;

namespace GameMusicInfoReader.Modules.Mod
{
	/// <summary>
	/// Represents a single sample data
	/// entry for a ProTracker module.
	/// </summary>
	public sealed class Sample
	{
		/// <summary>
		/// Internal name for this Sample.
		/// </summary>
		public string SampleName { get; private set; }

		/// <summary>
		/// Sample length in 2-byte words.
		/// </summary>
		public int SampleLengthInWords { get; private set; }

		/// <summary>
		/// Sample length in bytes.
		/// </summary>
		public int SampleLengthInBytes { get; private set; }

		/// <summary>
		/// Fine tune value.
		/// </summary>
		public int FineTuneValue { get; private set; }

		/// <summary>
		/// Volume for this sample.
		/// </summary>
		public int Volume { get; private set; }

		/// <summary>
		/// Repeat point offset in 2-byte words.
		/// </summary>
		public int RepeatPointInWords { get; private set; }

		/// <summary>
		/// Repeat point offset in bytes.
		/// </summary>
		public int RepeatPointInBytes { get; private set; }

		/// <summary>
		/// Repeat length in 2-byte words.
		/// </summary>
		public int RepeatLengthInWords { get; private set; }

		/// <summary>
		/// Repeat length in bytes.
		/// </summary>
		public int RepeatLengthInBytes { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reader">Reader to retrieve sample data with.</param>
		/// <exception cref="ArgumentNullException">If the given reader is null.</exception>
		public Sample(EndianBinaryReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			SampleName = new string(reader.ReadChars(22));
			SampleLengthInWords = reader.ReadUInt16();
			SampleLengthInBytes = SampleLengthInWords * 2;
			FineTuneValue       = (reader.ReadSByte() & 0xF);
			Volume              = reader.ReadByte();
			RepeatPointInWords  = reader.ReadUInt16();
			RepeatPointInBytes  = RepeatLengthInWords * 2;
			RepeatLengthInWords = reader.ReadInt16();
			RepeatLengthInBytes = RepeatLengthInWords * 2;
		}

		public override string ToString()
		{
			return SampleName;
		}
	}
}
