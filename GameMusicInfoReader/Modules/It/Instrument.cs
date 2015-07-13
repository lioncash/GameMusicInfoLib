namespace GameMusicInfoReader.Modules.It
{
	/// <summary>
	/// Represents a single instrument in this module.
	/// </summary>
	public sealed class Instrument
	{
		/// <summary>Instrument header magic</summary>
		public string InstrumentID { get; internal set; }

		/// <summary>DOS filename</summary>
		public string DOSFilename { get; internal set; }

		/// <summary>
		/// The specified instrument's new note action
		/// </summary>
		/// <value>
		/// <para>0 = Cut note </para>
		/// <para>1 = Continue </para>
		/// <para>2 = Note off </para>
		/// <para>3 = Note fade.</para>
		/// </value>
		public int NewNoteAction { get; internal set; }

		/// <summary>
		/// The Duplicate Check Type of an instrument
		/// </summary>
		/// <value>
		/// <para>0 = Off</para>
		/// <para>1 = Note</para>
		/// <para>2 = Sample</para>
		/// <para>3 = Instrument</para>
		/// </value>
		public int DuplicateCheckType { get; internal set; }

		/// <summary>
		/// The Duplicate Check Action (DCA) of the specified instrument
		/// </summary>
		/// <value>
		/// <para>0 = Cut </para>
		/// <para>1 = Note Off </para>
		/// <para>2 = Note Fade </para>
		/// </value>
		public int DuplicateCheckAction { get; internal set; }

		/// <summary>
		/// Fadeout of an instrument.
		/// Ranges between (0 - 128)
		/// </summary>
		/// <remarks>
		/// Fade applied when:
		/// <para>1) Note fade NNA is selected and triggered (by another note)</para>
		/// <para>2) Note off NNA is selected with no volume envelope or volume envelope loop</para>
		/// <para>3) Volume envelope end is reached</para>
		/// </remarks>
		/// <value>The fadeout length (0 - 128)</value>
		public int FadeOut { get; internal set; }

		/// <summary>
		/// The Pitch-Pan separation of a specified instrument
		/// </summary>
		/// <returns>
		/// The PPS of the instrument (-32 - 32)
		/// </returns>
		public sbyte PitchPanSeparation { get; internal set; }

		/// <summary>
		/// Pitch-pan center
		/// </summary>
		public byte PitchPanCenter { get; internal set; }

		/// <summary>
		/// Global volume
		/// </summary>
		/// <value>From 0 to 128</value>
		public byte GlobalVolume { get; internal set; }

		/// <summary>
		/// Default panning value
		/// </summary>
		/// <value>From 0 to 64. 128 means it's unused.</value>
		public byte DefaultPan { get; internal set; }

		/// <summary>
		/// Random volume variation (percentage)
		/// </summary>
		public byte RandomVolumeVariation { get; internal set; }

		/// <summary>
		/// Random panning variation (panning change - not implemented yet)
		/// </summary>
		public byte RandomPanningVariation { get; internal set; }

		/// <summary>
		/// Tracker version used to save the instrument.
		/// This is only used in the instrument files.
		/// </summary>
		public short TrackerVersion { get; internal set; }

		/// <summary>
		/// Number of samples associated with instrument.
		/// This is only used in the instrument files.
		/// </summary>
		public byte NumAssociatedSamples { get; internal set; }

		/// <summary>
		/// Instrument name
		/// </summary>
		public string InstrumentName { get; internal set; }

		/// <summary>
		/// Initial filter cutoff
		/// </summary>
		public byte InitialFilterCutoff { get; internal set; }

		/// <summary>
		/// Initial filter resonance
		/// </summary>
		public byte InitialFilterResonance { get; internal set; }

		/// <summary>
		/// MIDI channel number
		/// </summary>
		public int MidiChannel { get; internal set; }

		/// <summary>
		/// MIDI Program (instrument)
		/// </summary>
		public short MidiProgram { get; internal set; }

		/// <summary>
		/// MIDI Bank? Purpose not explained in the format documentation. (only referred to as MidiBnk)
		/// TODO: Figure out wtf this is used for and document it.
		/// </summary>
		public short MidiBank { get; internal set; }

		/// <summary>
		/// Each note of the instrument is first converted to a sample number
		/// and a note (C-0 -> B-9). These are stored as note/sample byte pairs
		/// (note first, range 0->119 for C-0 to B-9, sample ranges from 1-99, 0=no sample)
		/// </summary>
		/// <remarks>
		/// Overall, this table is 240 bytes in length if it were to be read entirely
		/// in one pass, instead of being broken into struct pairs.
		/// </remarks>
		public KeyboardTablePair[] KeyboardTable { get; internal set; }

		/// <summary>
		/// Instrument envelopes
		/// </summary>
		public Envelope[] Envelopes { get; internal set; }
	}
}
